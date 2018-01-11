using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace WebApi.Helper
{
    public class JWTHelper
    {
        private const string Secret = "401b09eab3c013d4ca54922bb802bec8fd5318192b0a75f201dPai";

        internal static string GenerateToken(string username, int expireTime = 30)
        {
            var symmetricKey = Encoding.Default.GetBytes(Secret);
            //var symmetricKey = Convert.FromBase64String(Secret);
            var tokenHandler = new JwtSecurityTokenHandler();

            var now = DateTime.UtcNow.AddHours(8);
            //設定Token的內容
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                        {
                        new Claim(ClaimTypes.Name, username)
                    }),

                Expires = now.AddMinutes(Convert.ToInt32(expireTime)),
                NotBefore = now,
                Audience = username,
                Issuer = "RMQ.WebAPI",
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(symmetricKey), SecurityAlgorithms.HmacSha256Signature)
            };

            var stoken = tokenHandler.CreateToken(tokenDescriptor);
            var token = tokenHandler.WriteToken(stoken);

            return token;
        }

        internal static ClaimsPrincipal GetPrincipal(string token)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var jwtToken = tokenHandler.ReadToken(token) as JwtSecurityToken;

                if (jwtToken == null)
                    return null;

                //var symmetricKey = Convert.FromBase64String(Secret);
                var symmetricKey = Encoding.Default.GetBytes(Secret);
                //比對Token
                var validationParameters = new TokenValidationParameters()
                {
                    RequireExpirationTime = true,
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    IssuerSigningKey = new SymmetricSecurityKey(symmetricKey),
                    LifetimeValidator = CustomLifetimeValidator,
                    //ClockSkew = DateTime.UtcNow.AddDays(8).TimeOfDay
                };

                SecurityToken securityToken;
                var principal = tokenHandler.ValidateToken(token, validationParameters, out securityToken);

                return principal;
            }

            catch (Exception ex )
            {
                //should write log
                return null;
            }
        }

        private static bool CustomLifetimeValidator(DateTime? notBefore, DateTime? expires, SecurityToken securityToken, TokenValidationParameters validationParameters)
        {
            if (expires != null)
            {
                return expires > DateTime.UtcNow.AddHours(8);
            }
            return false;
        }
    }
}