using System;

namespace BazaarBot
{
	/// <summary>
	/// Base class that represents something that can be bought and sold.
	/// </summary>
    public class Good : ICloneable, IEquatable<Good>
    {
		/// <summary>
		/// Human-readable identifier.
		/// </summary>
	    public UniqueId Id { get; protected set; }

	    public Good (UniqueId id)
	    {
		    Id = id;
	    }

	    public object Clone() => new Good(Id);

		public Good Copy() => Clone() as Good;

        public bool Equals(Good other)
			=> other != null && Id == other.Id;

        public override bool Equals(object obj)
			=> obj is Good other && Equals(other);

		public override int GetHashCode() => Id.GetHashCode();
	}
}
