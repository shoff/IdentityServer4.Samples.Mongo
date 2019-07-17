namespace MongoDbIdentityServer.Models
{
    using System.Collections.Generic;
    using IdentityModel;

    /// <summary>
    /// Claim equality comparer
    /// </summary>
    public class UserClaimComparer : IEqualityComparer<UserClaim>
    {
        private readonly bool valueAndTypeOnly;

        /// <summary>
        /// Initializes a new instance of the <see cref="ClaimComparer"/> class.
        /// </summary>
        public UserClaimComparer()
        {
            this.valueAndTypeOnly = false;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ClaimComparer"/> class.
        /// </summary>
        /// <param name="compareValueAndTypeOnly">if set to <c>true</c> only type and value are being compared.</param>
        public UserClaimComparer(bool compareValueAndTypeOnly)
        {
            this.valueAndTypeOnly = compareValueAndTypeOnly;
        }

        /// <summary>
        /// Determines whether the specified objects are equal.
        /// </summary>
        /// <param name="x">The first object to compare.</param>
        /// <param name="y">The second object to compare.</param>
        /// <returns>
        /// true if the specified objects are equal; otherwise, false.
        /// </returns>
        public bool Equals(UserClaim x, UserClaim y)
        {
            if (x == null && y == null)
            {
                return true;
            }

            if (x == null)
            {
                return false;
            }

            if (y == null)
            {
                return false;
            }

            if (this.valueAndTypeOnly)
            {
                return (x.Type == y.Type &&
                    x.Value == y.Value);
            }

            return (x.Type == y.Type &&
                x.Value == y.Value &&
                x.Issuer == y.Issuer &&
                x.ValueType == y.ValueType);
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <param name="claim">The claim.</param>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
        /// </returns>
        public int GetHashCode(UserClaim claim)
        {
            if (ReferenceEquals(claim, null))
            {
                return 0;
            }

            int typeHash = claim.Type?.GetHashCode() ?? 0;
            int valueHash = claim.Value?.GetHashCode() ?? 0;

            return typeHash ^ valueHash;
        }
    }
}