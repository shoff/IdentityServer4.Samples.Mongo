namespace MongoDbIdentityServer.Models
{
    using System.Collections.Generic;
    using System.Security.Claims;
    using MongoDB.Bson.Serialization.Attributes;

    [BsonIgnoreExtraElements]
    public class User
    {
        [BsonElement("subject_id")]
        public string SubjectId { get; set; }

        [BsonElement("username")]
        public string Username { get; set; }

        [BsonElement("password")]
        public string Password { get; set; }

        [BsonElement("provider_name")]
        public string ProviderName { get; set; }

        [BsonElement("provider_subject_id")]
        public string ProviderSubjectId { get; set; }

        [BsonElement("is_active")]
        public bool IsActive { get; set; } = true;

        [BsonElement("claims")]
        public ICollection<UserClaim> Claims { get; set; } = new HashSet<UserClaim>(new UserClaimComparer());
    }
}