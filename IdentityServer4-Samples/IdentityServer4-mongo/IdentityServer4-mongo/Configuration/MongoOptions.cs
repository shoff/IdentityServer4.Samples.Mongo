namespace MongoDbIdentityServer.Configuration
{
    public class MongoOptions
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string AuthMechanism { get; set; }
        public string MongoHost { get; set; }
        public string Port { get; set; }
        public string DefaultDb { get; set; }
    }
}