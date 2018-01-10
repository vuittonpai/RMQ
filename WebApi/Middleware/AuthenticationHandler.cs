using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using WebApi.Helper;

namespace WebApi.Middleware
{
    public class AuthenticationHandler : DelegatingHandler
    {
        private string _token;
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            if (this.IsTokenValid(request))
            {
                return await base.SendAsync(request, cancellationToken);
            }

            return request.CreateErrorResponse(HttpStatusCode.Unauthorized, "Not Valid Token");
        }

        private bool IsTokenValid(HttpRequestMessage request)
        {
            //1.查看驗證參數
            IEnumerable<string> authzHeaders;
            if (!request.Headers.TryGetValues("Authorization", out authzHeaders) || authzHeaders.Count() > 1)
            {
                return false;
            }
            //2.驗證Scheme，(JWT所以是Bearer，一般是Basic)，認不得該驗證方式
            var bearerToken = authzHeaders.ElementAt(0);
            if (bearerToken == null || !bearerToken.StartsWith("bearer "))
                return false;
            _token = bearerToken.StartsWith("Bearer ") ? bearerToken.Substring(7) : bearerToken;
            //3.驗證Token，發IPrincipal
            var principal = AuthenticateJwtToken(_token);
            Thread.CurrentPrincipal = principal;
            HttpContext.Current.User = principal;
            //4.如果認證Token有問題處理
            if (principal == null)
                return false;
           
            return true;
        }
        /// <summary>
        /// 驗證Token，建立IPrincipal
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        protected IPrincipal AuthenticateJwtToken(string token)
        {
            string username;

            if (ValidateToken(token, out username))
            {
                // based on username to get more information from database in order to build local identity
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, username)
                    // Add more claims if needed: Roles, ...
                };

                var identity = new ClaimsIdentity(claims, "Jwt");
                IPrincipal user = new ClaimsPrincipal(identity);

                return user;
            }

            return null;
        }
        /// <summary>
        /// 驗證Token，解碼
        /// </summary>
        /// <param name="token"></param>
        /// <param name="username"></param>
        /// <returns></returns>
        private static bool ValidateToken(string token, out string username)
        {
            username = null;
            //判別token回傳ClaimsPrincipal
            var simplePrinciple = JWTHelper.GetPrincipal(token);
            var identity = simplePrinciple?.Identity as ClaimsIdentity;

            if (identity == null)
                return false;

            if (!identity.IsAuthenticated)
                return false;

            var usernameClaim = identity.FindFirst(ClaimTypes.Name);
            username = usernameClaim?.Value;

            if (string.IsNullOrEmpty(username))
                return false;

            // More validate to check whether username exists in system
            //判別他的IP位置是否合法，部門是否合法之類的

            return true;
        }

        
    }
}