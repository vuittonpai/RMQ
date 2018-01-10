using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace WebApi.Middleware
{
    public class IPValidationHandler: DelegatingHandler
    {
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            if (this.IsIpValid(request))
            {
                return await base.SendAsync(request, cancellationToken);
            }
            return request.CreateErrorResponse(HttpStatusCode.Forbidden, "Not Valid IP Address");
        }

        private bool IsIpValid(HttpRequestMessage request)
        {
            if (!request.GetRequestContext().IsLocal)//local不擋
            {
                string ipaddress = string.Empty;
                if (request.Properties.ContainsKey("MS_HttpContext"))
                {
                    dynamic ctx = request.Properties["MS_HttpContext"];
                    if (ctx != null)
                    {
                        ipaddress = ctx.Request.UserHostAddress;
                    }
                }

                string ValidIpAddress = ConfigurationManager.AppSettings["ValidIpAddress"];
                
                if (!IsIpAddressValid(ipaddress, ValidIpAddress))
                {
                    return false;
                }                
            }
            return true;
        }

        private bool IsIpAddressValid(string ipAddress, string validAddresses)
        {
            var splitSingleIPs = validAddresses.Trim().Split(',');
            foreach (string singleIP in splitSingleIPs)
                return (singleIP == ipAddress);
            return false;
        }
    }
}