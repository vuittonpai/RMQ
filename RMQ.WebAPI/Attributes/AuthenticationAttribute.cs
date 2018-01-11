using System;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http.Filters;
using RMQ.WebApi.Attributes.AuthenticateHelper;
using WebApi.Helper;

namespace RMQ.WebApi.Attributes
{
    public class AuthenticationAttribute : Attribute, IAuthenticationFilter
    {
        public bool AllowMultiple => throw new NotImplementedException();

        /// <summary>
        /// validate進來的Request
        /// 成功會回傳 Iprinciple ，然後attach 在request
        /// </summary>
        /// <param name="context"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task AuthenticateAsync(HttpAuthenticationContext context, CancellationToken cancellationToken)
        {
            //1.查看驗證參數
            var request = context.Request;
            var authorization = request.Headers.Authorization;
            //2.驗證Scheme，(JWT所以是Bearer，一般是Basic)
            if (authorization == null || authorization.Scheme != "bearer")
                return;
            //3.認不得該驗證方式
            if (string.IsNullOrEmpty(authorization.Parameter))
            {
                //處理無Token
                context.ErrorResult = new AuthenticationFailureResult("Missing Jwt Token", request);
                return;
            }
            //4.驗證Token，發IPrincipal
            var principal = await AuthenticateJwtToken(authorization.Parameter);

            //5.如果認證Token有問題處理
            if (principal == null)
            {
                context.ErrorResult = new AuthenticationFailureResult("Invalid token", request);
            }
            else { 
                context.Principal = principal;
            }
        }

        /// <summary>
        /// 驗證Token，建立IPrincipal
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        protected Task<IPrincipal> AuthenticateJwtToken(string token)
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

                return Task.FromResult(user);
            }

            return Task.FromResult<IPrincipal>(null);
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
            //判別他的權限是否合法，部門是否合法之類的

            return true;
        }

        /// <summary>
        /// 添加Authentication challenge到HTTP Response
        /// </summary>
        /// <param name="context"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task ChallengeAsync(HttpAuthenticationChallengeContext context, CancellationToken cancellationToken)
        {
            var challenge = new AuthenticationHeaderValue("bearer");
            context.Result = new AddChallengeOnUnauthorizedResult(challenge, context.Result);
            return Task.FromResult(0);
        }
    }
}