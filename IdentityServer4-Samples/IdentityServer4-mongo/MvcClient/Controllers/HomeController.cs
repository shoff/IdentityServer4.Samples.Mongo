namespace MvcClient.Controllers
{
    using System.Net.Http;
    using System.Threading.Tasks;
    using ChaosMonkey.Guards;
    using IdentityModel.Client;
    using Microsoft.AspNetCore.Authentication;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;
    using Newtonsoft.Json.Linq;

    public class HomeController : Controller
    {
        private readonly HttpClient httpClient;

        public HomeController(
            IHttpClientFactory httpClientFactory,
            ILogger<HomeController> logger
        )
        {
            Guard.IsNotNull(httpClientFactory, nameof(httpClientFactory));
            this.httpClient = httpClientFactory.CreateClient("MyCustomAPI");
        }

        public IActionResult Index()
        {
            return this.View();
        }

        [Authorize]
        public IActionResult Secure()
        {
            this.ViewData["Message"] = "Secure page.";

            return this.View();
        }

        public async Task Logout()
        {
            await this.HttpContext.SignOutAsync("Cookies");
            await this.HttpContext.SignOutAsync("oidc");
        }

        public IActionResult Error()
        {
            return this.View();
        }

        public async Task<IActionResult> CallApiUsingClientCredentials()
        {

            var tokenResponse = await this.httpClient.RequestClientCredentialsTokenAsync(new ClientCredentialsTokenRequest
            {
                Address = "http://localhost:5000/connect/token",
                ClientId = "mvc",
                ClientSecret = "secret",
                Scope = "api1"
            });

            //var tokenClient = new TokenClient("http://localhost:5000/connect/token", "mvc", "secret");
            //var tokenResponse = await tokenClient.RequestClientCredentialsAsync("api1");
            // var client = new HttpClient();
            this.httpClient.SetBearerToken(tokenResponse.AccessToken);
            var content = await this.httpClient.GetStringAsync("http://localhost:5001/identity");

            this.ViewBag.Json = JArray.Parse(content).ToString();
            return this.View("json");
        }

        public async Task<IActionResult> CallApiUsingUserAccessToken()
        {
            var accessToken = await this.HttpContext.GetTokenAsync("access_token");
            this.httpClient.SetBearerToken(accessToken);
            var content = await this.httpClient.GetStringAsync("http://localhost:5001/identity");

            this.ViewBag.Json = JArray.Parse(content).ToString();
            return this.View("json");
        }
    }
}