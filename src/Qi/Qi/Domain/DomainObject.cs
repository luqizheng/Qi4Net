namespace Qi.Domain
{
    /// <summary>
    /// For a discussion of this object, see 
    /// http://devlicio.us/blogs/billy_mccafferty/archive/2007/04/25/using-equals-gethashcode-effectively.aspx
    /// Code from http://ayende.com/blog/2500/generic-entity-equality.
    /// </summary>
    public abstract class DomainObject<TId> : IDomainObject
    {
        private int? _oldHashcode;
       
        /// <summary>
        /// Id may be of type string, int, custom type, etc.
        /// Setter is protected to allow unit tests to set this property via reflection and to allow 
        /// domain objects more flexibility in setting this for those objects with assigned IDs.
        /// </summary>
        public virtual TId Id { get; protected set; }

        #region IDomainObject Members

        object IDomainObject.Id
        {
            get { return Id; }
        }

        #endregion

        /// <summary>
        /// Equality operator so we can have == semantics
        /// </summary>
        public static bool operator ==(DomainObject<TId> x, DomainObject<TId> y)
        {
            return Equals(x, y);
        }

        /// <summary>
        /// Inequality operator so we can have != semantics
        /// </summary>
        public static bool operator !=(DomainObject<TId> x, DomainObject<TId> y)
        {
            return !(x == y);
        }

        public override bool Equals(object obj)
        {
            var other = obj as DomainObject<TId>;
            if (other == null)
                return false;
            //to handle the case of comparing two new objects
            bool otherIsTransient = Equals(other.Id, default(TId));
            bool thisIsTransient = Equals(Id, default(TId));
            if (otherIsTransient && thisIsTransient)
                return ReferenceEquals(other, this);
            return other.Id.Equals(Id);
        }

        /// <summary>
        /// Transient objects are not associated with an item already in storage.  
        /// </summary>
        public virtual bool IsTransient()
        {
            return Id.Equals(default(TId));
        }

        /// <summary>
        /// Must be provided to properly compare two objects
        /// </summary>
        public override int GetHashCode()
        {
            if (_oldHashcode.HasValue)
                return _oldHashcode.Value;

            if (IsTransient())
            {
                _oldHashcode = base.GetHashCode();
                return _oldHashcode.Value;
            }

            return Id.GetHashCode();
        }
    }
}