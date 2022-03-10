using System;

namespace EconomySim
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

		/// <summary>
		/// Inventory space taken up.
		/// </summary>
	    public double Size { get; protected set; }

	    public Good (string id, double size)
	    {
		    ID = id;
		    Size = size;
			hashCode = ID.GetHashCode();
	    }

	    public object Clone() => new Good(ID, Size);

		public Good Copy() => Clone() as Good;

		public bool Equals(Good other)
			=> Size == other.Size && ID == other.ID;

		public override bool Equals(object obj)
			=> obj is Good other && Equals(other);

		public override int GetHashCode() => hashCode;
	}
}
