using EdjCase.ICP.Agent.Models;
using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Asn1.X9;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Crypto.Signers;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Math.EC;
using Org.BouncyCastle.Security;
using System;
using System.Collections.Generic;

namespace EdjCase.ICP.Agent.Identities
{
	/// <summary>
	/// An identity using a Ed25519 key
	/// </summary>
	public abstract class EcdsaIdentity : IIdentity
	{
		/// <summary>
		/// The public key of the identity, DER encoded
		/// </summary>
		public SubjectPublicKeyInfo PublicKey { get; }

		/// <summary>
		/// The private key of the identity
		/// </summary>
		public byte[] PrivateKey { get; }

		protected EcdsaIdentity(
			byte[] publicKey,
			byte[] privateKey,
			string curveOid
		)
		{
			this.PublicKey = SubjectPublicKeyInfo.Ecdsa(publicKey, curveOid);
			this.PrivateKey = privateKey;
		}

		/// <inheritdoc/>
		public SubjectPublicKeyInfo GetPublicKey()
		{
			return this.PublicKey;
		}


		/// <inheritdoc/>
		public byte[] Sign(byte[] message)
		{
			ECDomainParameters ecDomain = GetDomain(this.PublicKey.Algorithm.ParametersOid!);
			// Create the private key from the byte array
			ECPrivateKeyParameters privateKey = new(
				new BigInteger(1, this.PrivateKey),
				ecDomain
			);

			// Initialize the signer with the private key
			ECDsaSigner signer = new();
			signer.Init(true, privateKey);

			// Hash the message
			byte[] messageHash = DigestUtilities.CalculateDigest("SHA-256", message);

			// Sign the message hash
			BigInteger[] signature = signer.GenerateSignature(messageHash);

			// Concatenate the R and S components of the signature
			byte[] rBytes = signature[0].ToByteArrayUnsigned();
			byte[] sBytes = signature[1].ToByteArrayUnsigned();
			byte[] result = new byte[rBytes.Length + sBytes.Length];
			Buffer.BlockCopy(rBytes, 0, result, 0, rBytes.Length);
			Buffer.BlockCopy(sBytes, 0, result, rBytes.Length, sBytes.Length);

			return result;
		}

		/// <inheritdoc/>
		public List<SignedDelegation>? GetSenderDelegations()
		{
			return null;
		}


		internal static byte[] DeriveUncompressedPublicKey(byte[] privateKey, string curveOid)
		{
			ECDomainParameters ecDomain = GetDomain(curveOid);
			return DeriveUncompressedPublicKey(privateKey, ecDomain);
		}

		private static byte[] DeriveUncompressedPublicKey(
			byte[] privateKey,
			ECDomainParameters ecDomain
		)
		{
			var privateKeyBigInt = new BigInteger(1, privateKey);
			// Perform the scalar multiplication (privateKey * G) to get public key
			ECPoint publicKeyPoint = ecDomain.G.Multiply(privateKeyBigInt);

			// Encode the public key point as a non-compressed public key
			publicKeyPoint = publicKeyPoint.Normalize();
			return publicKeyPoint.GetEncoded(compressed: false);
			
		}


		protected static byte[] CreatePrivateKey(string curveOid)
		{
			// Get the curve parameters from the OID
			DerObjectIdentifier oid = new DerObjectIdentifier(curveOid);
			X9ECParameters curve = ECNamedCurveTable.GetByOid(oid);
			if(curve == null)
			{
				throw new Exception($"Curve with oid '{curveOid}' is not found or supported");
			}

			// Create a secure random generator
			SecureRandom random = new();

			// Generate the private key
			BigInteger privateKeyInt = new(curve.N.BitLength, random);
			while (privateKeyInt.CompareTo(BigInteger.Zero) <= 0
				|| privateKeyInt.CompareTo(curve.N) >= 0)
			{
				privateKeyInt = new BigInteger(curve.N.BitLength, random);
			}

			// Convert the private key to a byte array
			return privateKeyInt.ToByteArrayUnsigned();
		}

		private static ECDomainParameters GetDomain(string oid)
		{
			X9ECParameters curve = ECNamedCurveTable.GetByOid(new DerObjectIdentifier(oid));
			return new ECDomainParameters(
				curve.Curve,
				curve.G,
				curve.N,
				curve.H,
				curve.GetSeed()
			);
		}
	}
}
