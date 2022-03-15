using System;

namespace BazaarBot
{
	/// <summary>
	/// Base class that represents something that can be bought and sold.
	/// </summary>
    public class Good : ICloneable, IEquatable<Good>
    {
		/// <summary>
		/// Cached hash property to speed up dictionary hits.
		/// </summary>
		private readonly int hashCode;

		/// <summary>
		/// Human-readable identifier.
		/// </summary>
	    public string ID { get; protected set; }

	    public Good (string id)
	    {
		    ID = id;
			hashCode = ID.GetHashCode();
	    }

	    public object Clone() => new Good(ID);

		public Good Copy() => Clone() as Good;

		public bool Equals(Good other)
			=> hashCode == other.hashCode;

		public override bool Equals(object obj)
			=> obj is Good other && Equals(other);

		public override int GetHashCode() => hashCode;
	}
}
