using System;

namespace BazaarBot
{
    /// <summary>
    /// A string-based unique identifier.
    /// </summary>
    [Serializable]
    public struct UniqueId : IEquatable<UniqueId>, IEquatable<string>, IEquatable<int>
    {
        private const int MAX_LENGTH = 16;

        /// <summary>
        /// The factory method which is responsible for creating a
        /// new <see cref="UniqueId"/>.
        /// By default it implements the <see cref="Guid"/> class,
        /// but it can be overridden with your own implementation.
        /// </summary>
        public static Func<string> NewIDProvider { get; set; }
            = () => Guid.NewGuid().ToString().Replace("-", "").Substring(0, MAX_LENGTH);

        public static UniqueId New => FromString(NewIDProvider());

        public string ID { get; private set; }

        private int? cachedHash;
        public int Hash { get => cachedHash ?? (cachedHash = ID.GetHashCode()).Value; }

        #region Constructors

        public UniqueId(string id)
        {
            ID = id;
            cachedHash = null;
        }

        #endregion Constructors

        public void GenerateNewId() => ID = New;

        public override string ToString() => ID;

        public override int GetHashCode() => Hash;

        public static UniqueId FromString(string src) => new UniqueId(src);

        #region IEquatable

        public bool Equals(UniqueId other) => this.Hash == other.Hash;
        public bool Equals(string other) => ID.QuickEquals(other);
        public bool Equals(int other) => Hash == other;

        #endregion IEquatable

        #region Equality Operators

        public override bool Equals(object obj) => obj is UniqueId other && Equals(other);

        public static bool operator ==(UniqueId a, UniqueId b) => a.Equals(b);
        public static bool operator !=(UniqueId a, UniqueId b) => !(a == b);

        public static bool operator ==(UniqueId a, string b) => a.Equals(b);
        public static bool operator !=(UniqueId a, string b) => !(a == b);

        public static bool operator ==(string a, UniqueId b) => b.Equals(a);
        public static bool operator !=(string a, UniqueId b) => !(a == b);

        public static bool operator ==(int a, UniqueId b) => b.Equals(a);
        public static bool operator !=(int a, UniqueId b) => !(a == b);

        public static bool operator ==(UniqueId a, int b) => b.Equals(a);
        public static bool operator !=(UniqueId a, int b) => !(a == b);

        #endregion Equality Operators

        public static implicit operator string(UniqueId id) => id.ID;
        public static implicit operator int(UniqueId id) => id.Hash;
        public static implicit operator UniqueId(string id) => new UniqueId(id);
    }
}
