using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Http;
using System.Web.Http.Controllers;

namespace RMQ.WebApi.Attributes
{
    public class FilterIPAttribute: AuthorizeAttribute
    {

        protected override bool IsAuthorized(HttpActionContext actionContext)
        {
            if (actionContext == null)
                throw new ArgumentNullException("actionContext");

            string ipAddress = ((HttpContextWrapper)actionContext.Request.Properties["MS_HttpContext"]).Request.UserHostName;
            string validAddresses = ConfigurationManager.AppSettings["JOB_IP_InitalCategoryTree"];

            try
            {
                if (!IsIpAddressValid(ipAddress, validAddresses))
                {
                    //log-forbidden
                    return false;
                }
            }
            catch (Exception e)
            {
                // Log
            }

            return true; 
        }
       
        
        private bool IsIpAddressValid(string ip, string list)
        {
            var splitSingleIPs = list.Trim().Split(',');
            foreach (string singleIP in splitSingleIPs)
                return (singleIP == ip);
            return false;
        }

      
       
    }

}