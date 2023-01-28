using EdjCase.ICP.Candid.Models.Types;
using System;
using System.Linq;

namespace EdjCase.ICP.Candid.Models.Values
{
	/// <summary>
	/// A model representing the value of a candid opt
	/// </summary>
	public class CandidOptional : CandidValue
	{
		/// <inheritdoc />
		public override CandidValueType Type { get; } = CandidValueType.Optional;

		/// <summary>
		/// The inner value of an opt. If not set, will be a candid null value
		/// </summary>
		public CandidValue Value { get; }

		/// <param name="value">The inner value of an opt. If not set, will be a candid null value</param>
		public CandidOptional(CandidValue? value = null)
		{
			this.Value = value ?? CandidPrimitive.Null();
		}

		/// <inheritdoc />
		internal override byte[] EncodeValue(CandidType type, Func<CandidId, CandidCompoundType> getReferencedType)
		{
			CandidOptionalType t;
			if (type is CandidReferenceType r)
			{
				t = (CandidOptionalType)getReferencedType(r.Id);
			}
			else
			{
				t = (CandidOptionalType)type;
			}
			if (this.Value.Type == CandidValueType.Primitive
				&& this.Value.AsPrimitive().ValueType == PrimitiveType.Null)
			{
				return new byte[] { 0 };
			}
			return new byte[] { 1 }
				.Concat(this.Value.EncodeValue(t.Value, getReferencedType))
				.ToArray();
		}

		/// <inheritdoc />
		public override int GetHashCode()
		{
			return HashCode.Combine(this.Value);
		}

		/// <inheritdoc />
		public override bool Equals(CandidValue? other)
		{
			if (other is CandidOptional o)
			{
				return this.Value.Equals(o.Value);
			}
			return false;
		}

		/// <inheritdoc />
		public override string ToString()
		{
			return $"opt {this.Value}";
		}
	}
}