using System;
using System.Buffers;
using Xunit;
using EdjCase.ICP.Agent.Responses;
using System.Collections.Generic;
using System.Formats.Cbor;
using EdjCase.ICP.Candid;
using EdjCase.ICP.Candid.Mapping;
using EdjCase.ICP.Candid.Models;
using EdjCase.ICP.Candid.Models.Types;
using EdjCase.ICP.Candid.Models.Values;
using EdjCase.ICP.Candid.Utilities;

public class QueryResponseTests
{
	[Fact]
	public void ReadCbor_ReturnsQueryResponse_WhenStatusIsReplied()
	{
		// query_blocks : (GetBlocksArgs) -> (QueryBlocksResponse) query;
		// get icp ledger by query_blocks with
		//	  const int start = 15_174_119;
		//    var getBlockReq = new GetBlocksArgs(start, 1);
		//    var res = await client.QueryBlocks(getBlockReq);

		var bytes = new List<byte>
		{
			217,217,247,191,102,115,116,97,116,117,115,103,114,101,112,108,105,101,100,101,114,101,112,108,121,161,99,97,114,103,89,3,148,68,73,68,76,25,108,5,151,146,139,218,1,1,134,221,168,191,10,3,164,199,168,206,12,120,145,251,149,146,13,120,131,244,244,196,15,16,110,2,109,123,109,4,108,3,222,195,137,220,4,5,214,169,187,174,10,12,195,156,153,173,15,1,108,4,186,137,229,194,4,120,147,166,198,144,9,1,167,136,130,130,10,6,130,243,243,145,12,12,110,7,107,4,173,250,237,251,1,8,239,128,229,223,2,13,194,245,213,153,3,14,203,214,253,160,11,15,108,7,198,252,182,2,9,234,202,138,158,4,2,185,135,146,234,7,124,216,187,178,132,12,9,145,156,156,191,13,10,222,167,247,218,13,11,203,150,220,180,14,2,108,1,224,169,179,2,120,110,9,110,12,108,1,214,246,142,128,1,120,108,3,234,202,138,158,4,2,216,163,140,168,13,9,203,150,220,180,14,1,108,2,251,202,1,2,216,163,140,168,13,9,108,5,251,202,1,2,198,252,182,2,9,234,202,138,158,4,2,216,163,140,168,13,9,203,150,220,180,14,1,109,17,108,3,197,179,154,248,7,18,226,232,173,160,8,120,230,169,158,248,9,120,106,1,19,1,20,1,1,108,2,226,232,173,160,8,120,230,169,158,248,9,120,107,2,188,138,1,21,197,254,210,1,22,108,1,134,221,168,191,10,3,107,2,176,173,140,212,4,23,176,173,143,205,12,24,108,2,193,164,130,166,5,120,128,213,219,221,5,120,108,2,144,198,193,150,5,113,196,152,177,181,13,120,1,0,1,253,3,217,217,247,162,100,116,114,101,101,131,1,131,1,131,1,130,4,88,32,166,90,250,56,226,36,34,136,99,168,158,187,115,207,229,92,249,192,181,230,200,248,117,108,57,136,205,120,57,118,94,139,131,2,72,99,97,110,105,115,116,101,114,131,1,131,1,131,1,130,4,88,32,3,214,4,176,239,29,179,161,208,159,237,126,183,24,159,209,28,63,27,130,116,127,82,52,15,89,159,103,194,131,199,137,131,1,131,2,74,0,0,0,0,0,0,0,2,1,1,131,1,131,1,131,2,78,99,101,114,116,105,102,105,101,100,95,100,97,116,97,130,3,88,32,211,156,41,215,78,210,166,152,149,183,15,239,50,110,130,20,49,65,22,225,255,31,81,252,221,77,40,68,187,180,196,72,130,4,88,32,216,246,79,122,252,166,165,93,78,230,222,217,176,32,11,172,102,81,202,244,199,161,146,2,18,181,160,60,155,241,223,54,130,4,88,32,4,101,1,112,217,117,249,236,67,88,222,202,196,1,177,63,14,59,28,2,87,222,40,200,242,136,68,102,92,114,161,193,130,4,88,32,164,56,239,93,55,255,211,64,51,236,121,236,198,205,177,37,133,255,56,20,76,44,228,186,21,56,71,125,40,134,67,157,130,4,88,32,196,174,101,56,202,172,134,15,107,0,41,146,208,142,84,42,45,38,201,114,201,225,155,166,178,16,56,133,144,102,78,119,130,4,88,32,101,204,143,63,31,231,106,85,221,226,38,134,75,14,198,126,167,25,57,229,245,66,210,180,3,251,127,144,127,131,87,69,130,4,88,32,133,31,223,199,84,144,149,54,251,164,30,93,84,169,89,139,11,105,225,6,80,25,42,164,64,47,177,124,75,42,153,165,131,1,130,4,88,32,218,116,26,70,4,82,174,110,69,52,131,19,16,141,71,229,65,21,84,50,140,39,145,5,126,152,31,189,78,128,137,133,131,2,68,116,105,109,101,130,3,73,165,208,218,163,182,128,219,128,24,105,115,105,103,110,97,116,117,114,101,88,48,163,174,166,22,47,205,8,7,16,120,94,19,49,208,193,135,81,103,95,202,97,32,58,50,19,31,112,22,53,58,111,249,211,252,93,47,164,175,187,208,231,100,236,207,226,242,93,89,0,194,137,233,0,0,0,0,0,64,133,233,0,0,0,0,0,1,1,1,10,0,0,0,0,0,0,0,14,1,1,10,103,101,116,95,98,108,111,99,107,115,231,137,231,0,0,0,0,0,1,0,0,0,0,0,0,0,106,115,105,103,110,97,116,117,114,101,115,129,163,105,116,105,109,101,115,116,97,109,112,27,24,1,108,3,100,118,168,37,105,115,105,103,110,97,116,117,114,101,88,64,113,175,198,158,114,37,165,169,100,234,226,133,212,63,44,68,159,191,159,213,117,166,28,201,252,223,141,33,138,218,183,74,190,238,60,9,33,21,211,220,156,149,243,250,107,163,33,70,8,37,51,109,185,99,68,223,74,185,251,249,48,35,142,5,104,105,100,101,110,116,105,116,121,88,29,228,57,177,175,17,90,164,61,199,170,33,63,139,22,67,144,250,203,211,3,0,231,33,199,215,177,238,69,2,255
		};


		var reader = new CborReader(bytes.ToArray(), CborConformanceMode.Strict, false);

		// // Ensure no exception is thrown
		var response = QueryResponse.ReadCbor(reader);

		Assert.Equal(QueryResponseType.Replied, response.Type);

		CandidArg reply = response.ThrowOrGetReply();
		var obj=  reply.ToObjects<QueryBlocksResponse>( );

		Assert.NotNull(obj);
	}
}

public class QueryBlocksResponse
{
	[CandidName("certificate")] public OptionalValue<List<byte>> Certificate { get; set; }

	[CandidName("blocks")] public List<CandidBlock> Blocks { get; set; }

	[CandidName("chain_length")] public ulong ChainLength { get; set; }

	[CandidName("first_block_index")] public ulong FirstBlockIndex { get; set; }

	[CandidName("archived_blocks")] public List<ArchivedBlocksRange> ArchivedBlocks { get; set; }

	public QueryBlocksResponse(OptionalValue<List<byte>> certificate, List<CandidBlock> blocks, ulong chainLength,
		ulong firstBlockIndex, List<ArchivedBlocksRange> archivedBlocks)
	{
		this.Certificate = certificate;
		this.Blocks = blocks;
		this.ChainLength = chainLength;
		this.FirstBlockIndex = firstBlockIndex;
		this.ArchivedBlocks = archivedBlocks;
	}

	public QueryBlocksResponse()
	{
	}
}

public class CandidBlock
{
	[CandidName("transaction")]
	public CandidTransaction Transaction { get; set; }

	[CandidName("timestamp")]
	public TimeStamp Timestamp { get; set; }

	[CandidName("parent_hash")]
	public OptionalValue<List<byte>> ParentHash { get; set; }

	public CandidBlock(CandidTransaction transaction, TimeStamp timestamp, OptionalValue<List<byte>> parentHash)
	{
		this.Transaction = transaction;
		this.Timestamp = timestamp;
		this.ParentHash = parentHash;
	}

	public CandidBlock()
	{
	}
}

public class CandidTransaction
{
	[CandidName("memo")]
	public ulong Memo { get; set; }

	[CandidName("icrc1_memo")]
	public OptionalValue<List<byte>> Icrc1Memo { get; set; }

	[CandidName("operation")]
	public OptionalValue<CandidOperation> Operation { get; set; }

	[CandidName("created_at_time")]
	public TimeStamp CreatedAtTime { get; set; }

	public CandidTransaction(ulong memo, OptionalValue<List<byte>> icrc1Memo, OptionalValue<CandidOperation> operation, TimeStamp createdAtTime)
	{
		this.Memo = memo;
		this.Icrc1Memo = icrc1Memo;
		this.Operation = operation;
		this.CreatedAtTime = createdAtTime;
	}

	public CandidTransaction()
	{
	}
}

[Variant]
	public class CandidOperation
	{
		[VariantTagProperty]
		public CandidOperationTag Tag { get; set; }

		[VariantValueProperty]
		public object? Value { get; set; }

		public CandidOperation(CandidOperationTag tag, object? value)
		{
			this.Tag = tag;
			this.Value = value;
		}

		protected CandidOperation()
		{
		}

		public static CandidOperation Approve(CandidOperation.ApproveInfo info)
		{
			return new CandidOperation(CandidOperationTag.Approve, info);
		}

		public static CandidOperation Burn(CandidOperation.BurnInfo info)
		{
			return new CandidOperation(CandidOperationTag.Burn, info);
		}

		public static CandidOperation Mint(CandidOperation.MintInfo info)
		{
			return new CandidOperation(CandidOperationTag.Mint, info);
		}

		public static CandidOperation Transfer(CandidOperation.TransferInfo info)
		{
			return new CandidOperation(CandidOperationTag.Transfer, info);
		}

		public CandidOperation.ApproveInfo AsApprove()
		{
			this.ValidateTag(CandidOperationTag.Approve);
			return (CandidOperation.ApproveInfo)this.Value!;
		}

		public CandidOperation.BurnInfo AsBurn()
		{
			this.ValidateTag(CandidOperationTag.Burn);
			return (CandidOperation.BurnInfo)this.Value!;
		}

		public CandidOperation.MintInfo AsMint()
		{
			this.ValidateTag(CandidOperationTag.Mint);
			return (CandidOperation.MintInfo)this.Value!;
		}

		public CandidOperation.TransferInfo AsTransfer()
		{
			this.ValidateTag(CandidOperationTag.Transfer);
			return (CandidOperation.TransferInfo)this.Value!;
		}

		private void ValidateTag(CandidOperationTag tag)
		{
			if (!this.Tag.Equals(tag))
			{
				throw new InvalidOperationException($"Cannot cast '{this.Tag}' to type '{tag}'");
			}
		}

		public class ApproveInfo
		{
			[CandidName("fee")]
			public Tokens Fee { get; set; }

			[CandidName("from")]
			public List<byte> From { get; set; }

			[CandidName("allowance_e8s")]
			public UnboundedInt AllowanceE8s { get; set; }

			[CandidName("allowance")]
			public Tokens Allowance { get; set; }

			[CandidName("expected_allowance")]
			public OptionalValue<Tokens> ExpectedAllowance { get; set; }

			[CandidName("expires_at")]
			public OptionalValue<TimeStamp> ExpiresAt { get; set; }

			[CandidName("spender")]
			public List<byte> Spender { get; set; }

			public ApproveInfo(Tokens fee, List<byte> from, UnboundedInt allowanceE8s, Tokens allowance, OptionalValue<Tokens> expectedAllowance, OptionalValue<TimeStamp> expiresAt, List<byte> spender)
			{
				this.Fee = fee;
				this.From = from;
				this.AllowanceE8s = allowanceE8s;
				this.Allowance = allowance;
				this.ExpectedAllowance = expectedAllowance;
				this.ExpiresAt = expiresAt;
				this.Spender = spender;
			}

			public ApproveInfo()
			{
			}
		}

		public class BurnInfo
		{
			[CandidName("from")]
			public List<byte> From { get; set; }

			[CandidName("amount")]
			public Tokens Amount { get; set; }

			[CandidName("spender")]
			public OptionalValue<List<byte>> Spender { get; set; }

			public BurnInfo(List<byte> from, Tokens amount, OptionalValue<List<byte>> spender)
			{
				this.From = from;
				this.Amount = amount;
				this.Spender = spender;
			}

			public BurnInfo()
			{
			}
		}

		public class MintInfo
		{
			[CandidName("to")]
			public List<byte> To { get; set; }

			[CandidName("amount")]
			public Tokens Amount { get; set; }

			public MintInfo(List<byte> to, Tokens amount)
			{
				this.To = to;
				this.Amount = amount;
			}

			public MintInfo()
			{
			}
		}

		public class TransferInfo
		{
			[CandidName("to")]
			public List<byte> To { get; set; }

			[CandidName("fee")]
			public Tokens Fee { get; set; }

			[CandidName("from")]
			public List<byte> From { get; set; }

			[CandidName("amount")]
			public Tokens Amount { get; set; }

			[CandidName("spender")]
			public OptionalValue<List<byte>> Spender { get; set; }

			public TransferInfo(List<byte> to, Tokens fee, List<byte> from, Tokens amount, OptionalValue<List<byte>> spender)
			{
				this.To = to;
				this.Fee = fee;
				this.From = from;
				this.Amount = amount;
				this.Spender = spender;
			}

			public TransferInfo()
			{
			}
		}
	}

	public enum CandidOperationTag
	{
		Approve,
		Burn,
		Mint,
		Transfer
	}

	public class Tokens
	{
		[CandidName("e8s")]
		public ulong E8s { get; set; }

		public Tokens(ulong e8s)
		{
			this.E8s = e8s;
		}

		public Tokens()
		{
		}
	}

	public class TimeStamp
	{
		[CandidName("timestamp_nanos")]
		public ulong TimestampNanos { get; set; }

		public TimeStamp(ulong timestampNanos)
		{
			this.TimestampNanos = timestampNanos;
		}

		public TimeStamp()
		{
		}
	}

	public class ArchivedBlocksRange
	{
		[CandidName("callback")]
		public CandidFunc Callback { get; set; }

		[CandidName("start")]
		public ulong Start { get; set; }

		[CandidName("length")]
		public ulong Length { get; set; }

		public ArchivedBlocksRange(CandidFunc callback, ulong start, ulong length)
		{
			this.Callback = callback;
			this.Start = start;
			this.Length = length;
		}

		public ArchivedBlocksRange()
		{
		}
	}

	public class CandidFunc : CandidValue
  {
    public override CandidValueType Type { get; } = CandidValueType.Func;

    public bool IsOpaqueReference { get; }

    public (CandidService Service, string Method)? ServiceInfo { get; }

    public CandidFunc(CandidService service, string name)
    {
      this.IsOpaqueReference = false;
      this.ServiceInfo = new (CandidService, string)?((service, name));
    }

    private CandidFunc()
    {
      this.IsOpaqueReference = true;
      this.ServiceInfo = new (CandidService, string)?();
    }

    internal override void EncodeValue(
      CandidType type,
      Func<CandidId, CandidCompoundType> getReferencedType,
      IBufferWriter<byte> destination)
    {
      if (this.IsOpaqueReference)
      {
        destination.WriteOne<byte>((byte) 0);
      }
      else
      {
        CandidFuncType type1 = CandidValue.DereferenceType<CandidFuncType>(type, getReferencedType);
        (CandidService Service, string str) = this.ServiceInfo.Value;
        destination.WriteOne<byte>((byte) 1);
        Service.EncodeValue((CandidType) type1, getReferencedType, destination);
        destination.WriteUtf8LebAndValue((ReadOnlySpan<char>) str);
      }
    }

    public override int GetHashCode()
    {
      return HashCode.Combine<bool, (CandidService, string)?>(this.IsOpaqueReference, this.ServiceInfo);
    }

    public override bool Equals(CandidValue? other)
    {
      if (!(other is CandidFunc candidFunc) || this.IsOpaqueReference != candidFunc.IsOpaqueReference || this.IsOpaqueReference)
        return false;
      (CandidService Service, string Method)? serviceInfo1 = this.ServiceInfo;
      (CandidService Service, string Method)? serviceInfo2 = candidFunc.ServiceInfo;
      bool hasValue = serviceInfo1.HasValue;
      if (hasValue != serviceInfo2.HasValue)
        return false;
      if (!hasValue)
        return true;
      (CandidService Service, string Method) valueOrDefault1 = serviceInfo1.GetValueOrDefault();
      (CandidService Service, string Method) valueOrDefault2 = serviceInfo2.GetValueOrDefault();
      return (CandidValue) valueOrDefault1.Service == (CandidValue) valueOrDefault2.Service && valueOrDefault1.Method == valueOrDefault2.Method;
    }

    public override string ToString()
    {
      if (this.IsOpaqueReference)
        return "(Opaque Reference)";
      (CandidService Service, string Method) tuple = this.ServiceInfo.Value;
      return string.Format("(Method: {0}, Service: {1})", (object) tuple.Method, (object) tuple.Service);
    }

    public static CandidFunc OpaqueReference() => new CandidFunc();
  }

	public class CandidService : CandidValue
	{
		private readonly EdjCase.ICP.Candid.Models.Principal? principalId;

		public override CandidValueType Type { get; } = CandidValueType.Service;

		public bool IsOpqaueReference { get; }

		public CandidService(EdjCase.ICP.Candid.Models.Principal principalId)
		{
			this.principalId = principalId ?? throw new ArgumentNullException(nameof (principalId));
			this.IsOpqaueReference = false;
		}

		private CandidService()
		{
			this.principalId = (EdjCase.ICP.Candid.Models.Principal) null;
			this.IsOpqaueReference = true;
		}

		public EdjCase.ICP.Candid.Models.Principal GetPrincipal()
		{
			if (this.IsOpqaueReference)
				throw new InvalidOperationException("Cannot get principal from opaque service reference.");
			return this.principalId;
		}

		internal override void EncodeValue(
			CandidType type,
			Func<CandidId, CandidCompoundType> getReferencedType,
			IBufferWriter<byte> destination)
		{
			if (this.IsOpqaueReference)
				destination.WriteOne<byte>((byte) 0);
			else
				CandidValue.Principal(this.principalId).EncodeValue((CandidType) CandidType.Principal(), getReferencedType, destination);
		}

		public override int GetHashCode()
		{
			return HashCode.Combine<bool, EdjCase.ICP.Candid.Models.Principal>(this.IsOpqaueReference, this.principalId);
		}

		public override bool Equals(CandidValue? other)
		{
			return other is CandidService candidService && this.IsOpqaueReference == candidService.IsOpqaueReference && !this.IsOpqaueReference && this.principalId == candidService.principalId;
		}

		public override string ToString()
		{
			return !this.IsOpqaueReference ? this.principalId.ToString() : "(Opaque Reference)";
		}

		public static CandidService OpaqueReference() => new CandidService();
	}
