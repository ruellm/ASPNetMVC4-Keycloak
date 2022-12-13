using Microsoft.Owin;
using Owin;
using System;
using System.Threading.Tasks;

using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Owin.Security.Keycloak;
using Microsoft.Owin.Security.OpenIdConnect;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using System.IdentityModel.Tokens;
using Microsoft.Owin.Host.SystemWeb;
using Microsoft.Owin.Security.Notifications;
using System.IO;
using System.Net;
using System.Text;
using System.Security.Cryptography;
using Newtonsoft.Json;
using System.Collections.Generic;

[assembly: OwinStartup(typeof(AspNetMVC4.Startup))]

namespace AspNetMVC4
{
    public class Startup
    {
        // Run keycloak from docker like this
        // follow this: https://www.keycloak.org/getting-started/getting-started-docker
        // docker run --name keycloak -p 8080:8080 -e KEYCLOAK_USER=admin -e KEYCLOAK_PASSWORD=admin jboss/keycloak
        public void Configuration(IAppBuilder app)
        {
            app.UseKentorOwinCookieSaver();
            // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=316888
            const string persistentAuthType = "keycloak_auth";
            app.SetDefaultSignInAsAuthenticationType(persistentAuthType);

            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                CookieSameSite = SameSiteMode.None,
                CookieSecure = CookieSecureOption.Always,
            });

            var desc = new AuthenticationDescription();
            desc.AuthenticationType = "keycloak_auth";
            desc.Caption = "keycloak_auth";
           

            app.UseOpenIdConnectAuthentication(new OpenIdConnectAuthenticationOptions
            {
                AuthenticationType = "Auth0",

                Authority = "http://localhost:8080/auth/realms/ScopicSoftware",

                ClientId = "keycloakdemo",
                ClientSecret = "Nr3XpY142xPxoxFfvivh2t7GvAbKx4z0",

                RedirectUri = "https://localhost:44337/home",
                
                ResponseType = OpenIdConnectResponseType.Code,
                ResponseMode = "query",
                UsePkce = false,

                RequireHttpsMetadata = false,

                CookieManager = new SameSiteCookieManager(new SystemWebCookieManager()),

                Notifications = new OpenIdConnectAuthenticationNotifications
                {
                    AuthorizationCodeReceived = (context) =>
                    {
                        var code = context.Code;
                        return Task.FromResult(0);
                    },

                    AuthenticationFailed = (context) =>
                    {
                        return Task.FromResult(0);
                    },

                    SecurityTokenReceived = (context) =>
                    {
                        return Task.FromResult(0);
                    },

                    MessageReceived = (context) =>
                    {
                        return Task.FromResult(0);
                    },

                    RedirectToIdentityProvider = notification =>
                    {
                        return Task.FromResult(0);
                    }
                }
            });
        }
    }
}
