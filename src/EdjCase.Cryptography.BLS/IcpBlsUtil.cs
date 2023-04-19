using System;
using System.Runtime.InteropServices;

namespace EdjCase.Cryptography.BLS
{
	/// <summary>
	/// Class with functions around BLS signatures (ICP flavor only)
	/// </summary>
	public static class IcpBlsUtil
	{
		private const int PublicKeyLength = 96;
		private const int SignatureLength = 48;

		private static object intializeLock = new object();
		private static bool isInitialized = false;

		/// <summary>
		/// Verifies a BLS signature (ICP flavor only)
		/// </summary>
		/// <param name="publicKey">The signer public key</param>
		/// <param name="messageHash">The SHA256 hash of the message</param>
		/// <param name="signature">The signature of the message</param>
		/// <returns>True if the signature is valid, otherwise false</returns>
		public static bool VerifySignature(
			byte[] publicKey,
			byte[] messageHash,
			byte[] signature
		)
		{
			if (signature.Length != SignatureLength)
			{
				throw new ArgumentOutOfRangeException(nameof(signature), signature.Length, $"Signature must be {SignatureLength} bytes long.");
			}
			if (publicKey.Length != PublicKeyLength)
			{
				throw new ArgumentOutOfRangeException(nameof(publicKey), publicKey.Length, $"Public Key must be {PublicKeyLength} bytes long.");
			}

			Delegates.Init init;
			Delegates.SetEthSerialization setEthSerialization;
			Delegates.SetMapToMode setMapToMode;
			Delegates.SetGeneratorOfPublicKey setGeneratorOfPublicKey;
			Delegates.MclBnG1SetDst mclBnG1SetDst;
			Delegates.PublicKeyDeserialize publicKeyDeserialize;
			Delegates.SignatureDeserialize signatureDeserialize;
			Delegates.Verify verify;
			if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
			{
				string libraryName = "libbls384_256";
				IntPtr libraryHandle = NativeInterop.LoadNativeLibrary(libraryName);
				T Get<T>(string functionName)
				{
					IntPtr blsInitPtr = NativeInterop.GetFunctionPointer(libraryHandle, functionName);
					return Marshal.GetDelegateForFunctionPointer<T>(blsInitPtr);
				}
				init = Get<Delegates.Init>("blsInit");
				setEthSerialization = Get<Delegates.SetEthSerialization>("blsSetETHserialization");
				setMapToMode = Get<Delegates.SetMapToMode>("setMapToMode");
				setGeneratorOfPublicKey = Get<Delegates.SetGeneratorOfPublicKey>("blsSetGeneratorOfPublicKey");
				mclBnG1SetDst = Get<Delegates.MclBnG1SetDst>("mclBnG1_setDst");
				publicKeyDeserialize = Get<Delegates.PublicKeyDeserialize>("blsPublicKeyDeserialize");
				signatureDeserialize = Get<Delegates.SignatureDeserialize>("blsSignatureDeserialize");
				verify = Get<Delegates.Verify>("blsVerify");
			}
			else
			{
				init = Interop.blsInit;
				setEthSerialization = Interop.blsSetETHserialization;
				setMapToMode = Interop.blsSetMapToMode;
				setGeneratorOfPublicKey = Interop.blsSetGeneratorOfPublicKey;
				mclBnG1SetDst = Interop.mclBnG1_setDst;
				publicKeyDeserialize = Interop.blsPublicKeyDeserialize;
				signatureDeserialize = Interop.blsSignatureDeserialize;
				verify = Interop.blsVerify;
			}
			return VerifySignatureInternal(
				publicKey,
				messageHash,
				signature,
				init,
				setEthSerialization,
				setMapToMode,
				setGeneratorOfPublicKey,
				mclBnG1SetDst,
				publicKeyDeserialize,
				signatureDeserialize,
				verify
			);
		}

		private static bool VerifySignatureInternal(
			byte[] publicKey,
			byte[] messageHash,
			byte[] signature,
			Delegates.Init blsInit,
			Delegates.SetEthSerialization setEthSerialization,
			Delegates.SetMapToMode setMapToMode,
			Delegates.SetGeneratorOfPublicKey setGeneratorOfPublicKey,
			Delegates.MclBnG1SetDst mclBnG1SetDst,
			Delegates.PublicKeyDeserialize publicKeyDeserialize,
			Delegates.SignatureDeserialize signatureDeserialize,
			Delegates.Verify verify
		)
		{
			EnsureInitialized(
				blsInit,
				setEthSerialization,
				setMapToMode,
				setGeneratorOfPublicKey,
				mclBnG1SetDst
			);

			var blsPublicKey = default(Interop.PublicKey);
			ulong publicKeyBytesRead = publicKeyDeserialize(ref blsPublicKey, publicKey, (ulong)publicKey!.LongLength);

			if (publicKeyBytesRead != (ulong)publicKey.Length)
			{
				throw new Exception($"Error deserializing BLS public key");
			}

			var blsSignature = default(Interop.Signature);
			ulong signatureBytesRead = signatureDeserialize(ref blsSignature, signature, (ulong)signature.LongLength);
			if (signatureBytesRead != (ulong)signature.LongLength)
			{
				throw new Exception($"Error deserializing BLS signature, length: {signatureBytesRead}");
			}

			int result = verify(in blsSignature, in blsPublicKey, messageHash, (ulong)messageHash.Length);

			return result == 1;
		}


		private static void EnsureInitialized(
			Delegates.Init blsInit,
			Delegates.SetEthSerialization setEthSerialization,
			Delegates.SetMapToMode setMapToMode,
			Delegates.SetGeneratorOfPublicKey setGeneratorOfPublicKey,
			Delegates.MclBnG1SetDst mclBnG1SetDst
		)
		{
			lock (IcpBlsUtil.intializeLock)
			{
				if (!IcpBlsUtil.isInitialized)
				{
					if (!Environment.Is64BitProcess)
					{
						throw new PlatformNotSupportedException("not 64-bit system");
					}
					int err = blsInit(Interop.MCL_BLS12_381, Interop.COMPILED_TIME_VAR);
					if (err != 0)
					{
						throw new Exception("BLS failed to initialize");
					}
					setEthSerialization(1);
					if (setMapToMode((int)Interop.MapToMode.HashToCurve) != 0)
					{
						throw new Exception("Failed while invoking: SetMapToMode");
					}
					Interop.PublicKey gen = new Interop.PublicKey();
					gen.SetStr("1 0x24aa2b2f08f0a91260805272dc51051c6e47ad4fa403b02b4510b647ae3d1770bac0326a805bbefd48056c8c121bdb8 0x13e02b6052719f607dacd3a088274f65596bd0d09920b61ab5da61bbdc7f5049334cf11213945d57e5ac7d055d042b7e 0x0ce5d527727d6e118cc9cdc6da2e351aadfd9baa8cbdd3a76d429a695160d12c923ac9cc3baca289e193548608b82801 0x0606c4a02ea734cc32acd2b02bc28b99cb3e287e85a763af267492ab572e99ab3f370d275cec1da1aaa9075ff05f79be");

					if (setGeneratorOfPublicKey(in gen) != 0)
					{
						throw new Exception("Failed while invoking: SetGeneratorOfPublicKey");
					}
					string dst = "BLS_SIG_BLS12381G1_XMD:SHA-256_SSWU_RO_NUL_";
					if (mclBnG1SetDst(dst, (ulong)dst.Length) != 0)
					{
						throw new Exception("Failed while invoking: SetDstG1:" + dst);
					}
					IcpBlsUtil.isInitialized = true;
				}
			}
		}
	}
}
