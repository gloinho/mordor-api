using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Text;
using System.Text.Json;

namespace mordor_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MordorController : ControllerBase
    {
        [HttpGet]
        [Route("/auth")]
        public async Task<ActionResult<string>> Authenticate()
        {
            var client = new HttpClient();
            var request = new HttpRequestMessage(HttpMethod.Post, "http://localhost:8080/realms/mordor/protocol/openid-connect/token");
            var formurlencoded = new Dictionary<string, string>
            {
                { "grant_type", "password" },
                { "client_id", "mordor-api" },
                { "username", "froto" },
                { "password", "123" },
                { "client_secret", "VTWI3jx14YCcOF23sLaZ09IYTzKo8Wrw" },
            };
            request.Content = new FormUrlEncodedContent(formurlencoded);
            var response = await client.SendAsync(request).Result.Content.ReadAsStringAsync();
            var token = JsonSerializer.Deserialize<AccessToken>(response);
            
            return Ok(token);

        }

        [HttpPost]
        public async Task<ActionResult> CreateUser([FromBody] string token)
        {
            var user = new User()
            {
                attributes = new Attributes() { attribute_key = "test_value" },
                credentials = new Credential[] { new Credential() { temporary = false, type = "password", value = "123" } },
                username = "testando",
                firstName = "Anas",
                lastName = "Gomes",
                email = "testedotnet@teste.com",
                emailVerified = true,
                enabled = true,
            };
            var client = new HttpClient();
            var request = new HttpRequestMessage(HttpMethod.Post, "http://localhost:8080/admin/realms/mordor/users");
            request.Content = new StringContent(JsonSerializer.Serialize(user), Encoding.UTF8, "application/json");
            request.Headers.Add("Authorization", $"Bearer {token}");
            var response = await client.SendAsync(request);
            return Ok(response.IsSuccessStatusCode);
        }

        [HttpGet]
        [Authorize(Roles = "CreateRing")]
        public ActionResult CreateRing()    
        {
            return Ok("Ash nazg durbatulûk, ash nazg gimbatul, ash nazg thrakatulûk, agh burzum-ishi krimpatul");
        }

        [HttpDelete]
        [Authorize(Roles ="DestroyRing")]
        public ActionResult DestroyRing()
        {
            return NoContent();
        }
    }
}
