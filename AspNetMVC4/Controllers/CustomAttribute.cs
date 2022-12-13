using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace AspNetMVC4.Controllers
{
   public class CustomAuthorizeAttribute : AuthorizeAttribute
    {
       // Entities context = new Entities(); // my entity  
       // private readonly string[] allowedroles;
        public CustomAuthorizeAttribute(params string[] roles)
        {
         //   this.allowedroles = roles;
        }
        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {           
            var codeObj = httpContext.Request.Params["code"];
            if(codeObj != null)
            {
                string code = codeObj.ToString();
                string query = httpContext.Request.Params["QUERY_STRING"].ToString();


                {
                    // Keycloak endpoint guide: https://www.baeldung.com/postman-keycloak-endpoints
                    var request = (HttpWebRequest)WebRequest.Create("http://localhost:8080/auth/realms/ScopicSoftware/protocol/openid-connect/token");

                    var postData = "grant_type=" + Uri.EscapeDataString("authorization_code");
                    postData += "&client_id=" + Uri.EscapeDataString("keycloakdemo");
                    postData += "&client_secret=" + Uri.EscapeDataString("Nr3XpY142xPxoxFfvivh2t7GvAbKx4z0");
                    postData += "&code=" + Uri.EscapeDataString(code);
                    postData += "&redirect_uri=" + Uri.EscapeDataString("https://localhost:44337/home");
                    var data = Encoding.ASCII.GetBytes(postData);

                    request.Method = "POST";
                    request.ContentType = "application/x-www-form-urlencoded";
                    request.ContentLength = data.Length;

                    using (var stream = request.GetRequestStream())
                    {
                        stream.Write(data, 0, data.Length);
                    }

                    var response = (HttpWebResponse)request.GetResponse();
                    var responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();

                    // now get user information
                    var values = JsonConvert.DeserializeObject<Dictionary<string, string>>(responseString);
                    request = (HttpWebRequest)WebRequest.Create("http://localhost:8080/auth/realms/ScopicSoftware/protocol/openid-connect/userinfo");

                    request.Method = "GET";
                    request.ContentType = "application/x-www-form-urlencoded";

                    var authHeader = "Bearer " + values["access_token"];
                    request.Headers.Add("Authorization", authHeader);

                    using (WebResponse response2 = request.GetResponse())
                    {
                        using (Stream stream = response2.GetResponseStream())
                        {
                            responseString = new StreamReader(response2.GetResponseStream()).ReadToEnd();
                        }
                    }

                    response = (HttpWebResponse)request.GetResponse();

                    var userInfo = JsonConvert.DeserializeObject<Dictionary<string, string>>(responseString);
                    var identity = new ClaimsIdentity(new List<Claim>
                        {
                            new Claim("UserName", userInfo["preferred_username"], ClaimValueTypes.String)
                        }, "Custom");

                    httpContext.User = new ClaimsPrincipal(identity);

                    return true;

                }
            }
           
            return false;
        }
        protected override void HandleUnauthorizedRequest(System.Web.Mvc.AuthorizationContext filterContext)
        {
            filterContext.Result = new HttpUnauthorizedResult();
        }
    }
}