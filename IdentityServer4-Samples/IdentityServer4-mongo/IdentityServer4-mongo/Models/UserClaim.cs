namespace MongoDbIdentityServer.Models
{
    using System.Security.Claims;
    using MongoDB.Bson.Serialization.Attributes;

    [BsonIgnoreExtraElements]
    public class UserClaim
    {
        public UserClaim() { }

        public UserClaim(string type, string value)
        {
            this.Type = type;
            this.Value = value;
        }

        /// <summary>Gets the claim type of the claim.</summary>
        /// <returns>The claim type.</returns>
        [BsonElement("type")]
        public string Type { get; set; }
        /// <summary>Gets the value of the claim.</summary>
        /// <returns>The claim value.</returns>
        [BsonElement("value")]
        public string Value { get; set; }
        /// <summary>Gets the value type of the claim.</summary>
        /// <returns>The claim value type.</returns>
        [BsonElement("value_type")]
        public string ValueType { get; set; }

        public string Issuer { get; set; }

        public static implicit operator Claim(UserClaim userClaim)
        {
            var claim = new Claim(userClaim.Type, userClaim.Value, userClaim.ValueType, userClaim.Issuer);
            return claim;
        }

        public static implicit operator UserClaim(Claim claim)
        {
            var userClaim = new UserClaim
            {
                Type = claim.Type,
                Value = claim.Value,
                ValueType = claim.ValueType,
                Issuer = claim.Issuer
            };
            return userClaim;
        }
    }
}