using System;

namespace Qi.Domain
{
    public interface IDomainObject
    {
        object Id { get; }
    }


    /// <summary>
    /// For a discussion of this object, see 
    /// http://devlicio.us/blogs/billy_mccafferty/archive/2007/04/25/using-equals-gethashcode-effectively.aspx
    /// </summary>
    public abstract class DomainObject<IdT> : IDomainObject
    {
        protected DomainObject()
        {
            Id = default(IdT);
        }

        /// <summary>
        /// Id may be of type string, int, custom type, etc.
        /// Setter is protected to allow unit tests to set this property via reflection and to allow 
        /// domain objects more flexibility in setting this for those objects with assigned IDs.
        /// </summary>
        public virtual IdT Id { get; protected set; }

        #region IDomainObject Members

        object IDomainObject.Id
        {
            get { return Id; }
        }

        #endregion

        public override bool Equals(object obj)
        {
            var compareTo = obj as DomainObject<IdT>;

            return (compareTo != null) &&
                   (HasSameNonDefaultIdAs(compareTo) ||
                    // Since the IDs aren't the same, either of them must be transient to 
                    // compare business value signatures
                    (((IsTransient()) || compareTo.IsTransient()) &&
                     HasSameBusinessSignatureAs(compareTo)));
        }

        /// <summary>
        /// Transient objects are not associated with an item already in storage.  
        /// </summary>
        public virtual bool IsTransient()
        {
            return Id == null || Id.Equals(default(IdT));
        }

        /// <summary>
        /// Must be provided to properly compare two objects
        /// </summary>
        public abstract override int GetHashCode();

        private bool HasSameBusinessSignatureAs(DomainObject<IdT> compareTo)
        {
            if (compareTo == null)
                throw new ArgumentNullException("compareTo");
            //Check.Require(compareTo != null, "compareTo may not be null");

            return GetHashCode().Equals(compareTo.GetHashCode());
        }

        /// <summary>
        /// Returns true if self and the provided persistent object have the same Id values 
        /// and the IDs are not of the default Id value
        /// </summary>
        private bool HasSameNonDefaultIdAs(DomainObject<IdT> compareTo)
        {
            //Check.Require(compareTo != null, "compareTo may not be null");
            if (compareTo == null)
                throw new ArgumentNullException("compareTo");
            return (Id != null && !Id.Equals(default(IdT))) &&
                   (compareTo.Id != null && !compareTo.Id.Equals(default(IdT))) &&
                   Id.Equals(compareTo.Id);
        }
    }
}