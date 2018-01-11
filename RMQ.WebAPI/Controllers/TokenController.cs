using System;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Web.Http;
using WebApi.Helper;
using RMQ.WebApi.Models;

namespace RMQ.WebApi.Controllers
{
    public class TokenController : ApiController
    {
        [HttpPost]
        [AllowAnonymous]
        [Route("GetAccountToken")]
        public string GetAccountToken([FromBody] AccountArgs account)
        {
            if (CheckUser(account.Username, account.Password))
            {
                return JWTHelper.GenerateToken(account.Username);
            }

            throw new HttpResponseException(HttpStatusCode.Unauthorized);
        }
        [HttpPost]
        [Route("GetApiKeyToken")]
        [AllowAnonymous]
        public string GetApiKeyToken([FromBody] AccountArgs account)
        {
            var byteData = Encoding.UTF8.GetBytes("");
            var hmac = new HMACSHA256(byteData);
            var key = Convert.ToBase64String(hmac.Key);
            return JWTHelper.GenerateToken(key);

            throw new HttpResponseException(HttpStatusCode.Unauthorized);
        }
        [HttpPost]
        [AllowAnonymous]
        public string Post([FromBody] string ApiKey)
        {
            var byteData = Encoding.UTF8.GetBytes(ApiKey);
            //另一個方式建立
            var hmac = new HMACSHA256(byteData);
            var key = Convert.ToBase64String(hmac.Key);
            //原方式
            return JWTHelper.GenerateToken(key);


            throw new HttpResponseException(HttpStatusCode.Unauthorized);
        }

        public bool CheckUser(string username, string password)
        {
            // should check in the database
            return true;
        }
    }
}