using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using EdjCase.ICP.Candid.Mapping;
using EdjCase.ICP.Candid;
using UserNumber = System.UInt64;
using PublicKey = System.Collections.Generic.List<System.Byte>;
using CredentialId = System.Collections.Generic.List<System.Byte>;
using DeviceKey = System.Collections.Generic.List<System.Byte>;
using UserKey = System.Collections.Generic.List<System.Byte>;
using SessionKey = System.Collections.Generic.List<System.Byte>;
using FrontendHostname = System.String;
using Timestamp = System.UInt64;
using ChallengeKey = System.String;

namespace EdjCase.ICP.InternetIdentity.Models
{
	public class Challenge
	{
		[EdjCase.ICP.Candid.Mapping.CandidNameAttribute("png_base64")]
		public string PngBase64 { get; set; }
		
		[EdjCase.ICP.Candid.Mapping.CandidNameAttribute("challenge_key")]
		public ChallengeKey ChallengeKey { get; set; }
		
	}
}

