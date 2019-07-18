namespace MongoDbIdentityServer.Models
{
    using System.Collections.Generic;
    using MongoDB.Bson.Serialization.Attributes;

    [BsonIgnoreExtraElements]
    public class User
    {
        /// <summary>
        /// Gets or sets the subject identifier.
        /// </summary>
        public string SubjectId { get; set; }

        /// <summary>
        /// Gets or sets the username.
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        /// Gets or sets the password.
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// Gets or sets the provider name.
        /// </summary>
        public string ProviderName { get; set; }

        /// <summary>
        /// Gets or sets the provider subject identifier.
        /// </summary>
        public string ProviderSubjectId { get; set; }

        /// <summary>
        /// Gets or sets if the user is active.
        /// </summary>
        public bool IsActive { get; set; } = true;

        /// <summary>
        /// Gets or sets the claims.
        /// </summary>
        public ICollection<UserClaim> Claims { get; set; } = new HashSet<UserClaim>(new UserClaimComparer());
    }
}