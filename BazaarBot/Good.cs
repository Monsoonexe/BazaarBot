using System;

namespace EconomySim
{
    public class Good : ICloneable, IEquatable<Good>
    {
		/// <summary>
		/// Cached hash property to speed up dictionary hits.
		/// </summary>
		private int hashCode;

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

	    public object Clone()
	    {
		    return new Good(ID, Size);
	    }

		public bool Equals(Good other)
			=> Size == other.Size && ID == other.ID;

		public override int GetHashCode() => hashCode;
	}
}
