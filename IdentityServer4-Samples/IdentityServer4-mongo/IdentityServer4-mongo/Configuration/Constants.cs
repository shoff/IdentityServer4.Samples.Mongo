// ReSharper disable InconsistentNaming
namespace MongoDbIdentityServer.Configuration
{
    public class Constants
    {
        public const string OIDC = "oidc";
        public const string OPEN_ID_CONNECT = "OpenID Connect";
        public const string IMPLICIT = "implicit";
        public const string AUTHORITY = "https://localhost:5000/"; // TODO optionize this

        // token
        public const string NAME = "name";
        public const string ROLE = "role";

        // mongo
        public const string ADMIN = "admin";
        public const string USERS_COLLECTION = "user";
    }
}