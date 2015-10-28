using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading.Tasks;
using System.Web;
using Microsoft.IdentityModel.Protocols;
using Microsoft.Owin;
using Microsoft.Owin.Extensions;
using Microsoft.Owin.Infrastructure;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.DataHandler;
using Microsoft.Owin.Security.DataProtection;
using Microsoft.Owin.Security.Notifications;
using Microsoft.Owin.Security.WsFederation;
using Owin;
using Sitecore;
using Sitecore.Configuration;
using Sitecore.Security.Accounts;
using Sitecore.Security.Authentication;
using SitecoreOwinFederator.Authenticator;
using SitecoreOwinFederator.Pipelines.HttpRequest;

[assembly: OwinStartup(typeof(SitecoreOwinFederator.Startup))]
namespace SitecoreOwinFederator
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {            
            app.MapWhen(ctx => MapDomain(ctx, "multisite.local"), site =>
            {
                site.SetDefaultSignInAsAuthenticationType(CookieAuthenticationDefaults.AuthenticationType);
                site.UseCookieAuthentication(new CookieAuthenticationOptions
                {
                    SlidingExpiration = false,
                    SessionStore = new SqlAuthSessionStore(new TicketDataFormat(new MachineKeyProtector())),
                    TicketDataFormat = new TicketDataFormat(new MachineKeyProtector()),    
                    Provider = new CookieAuthenticationProvider
                    {
                        OnException = exception => HandleException(exception)
                    }                
                });

                site.UseWsFederationAuthentication(new WsFederationAuthenticationOptions
                {
                    UseTokenLifetime = true,                    
                    MetadataAddress = "http://claimsprovider.local/FederationMetadata",
                    Wtrealm = "urn:multisite",
                    Wreply = "http://multisite.local/login",
                });
            });

            app.MapWhen(ctx => MapDomain(ctx, "multisite2.local"), site =>
            {
                site.SetDefaultSignInAsAuthenticationType(CookieAuthenticationDefaults.AuthenticationType);
                site.UseCookieAuthentication(new CookieAuthenticationOptions
                {
                    SlidingExpiration = false,
                    SessionStore = new SqlAuthSessionStore(new TicketDataFormat(new MachineKeyProtector())),
                    TicketDataFormat = new TicketDataFormat(new MachineKeyProtector()),
                    Provider = new CookieAuthenticationProvider
                    {
                        OnException = exception => HandleException(exception)
                    }
                });

                site.UseWsFederationAuthentication(new WsFederationAuthenticationOptions
                {
                    MetadataAddress = "http://claimsprovider.local/FederationMetadata",
                    Wtrealm = "urn:multisite2",
                    Wreply = "http://multisite2.local/login",
                });
            });

            app.MapWhen(ctx => MapDomain(ctx, "multisite3.local"), site =>
            {
                site.SetDefaultSignInAsAuthenticationType(CookieAuthenticationDefaults.AuthenticationType);
                site.UseCookieAuthentication(new CookieAuthenticationOptions
                {
                    SlidingExpiration = false,
                    SessionStore = new SqlAuthSessionStore(new TicketDataFormat(new MachineKeyProtector())),
                    TicketDataFormat = new TicketDataFormat(new MachineKeyProtector()),
                    Provider = new CookieAuthenticationProvider
                    {
                        
                        OnException = exception => HandleException(exception)
                    }
                });

                site.UseWsFederationAuthentication(new WsFederationAuthenticationOptions
                {
                    MetadataAddress = "https://sitecore.accesscontrol.windows.net/FederationMetadata/2007-06/FederationMetadata.xml",
                    Wtrealm = "urn:multisite3:local",                    
                });
            });

            //configure Antariksh ADFS middleware
            //app.Map("/sitecore modules/shell/federation", config => {
                
            //    config.UseWsFederationAuthentication(new WsFederationAuthenticationOptions
            //    {
            //        UseTokenLifetime = true,
            //        MetadataAddress = "http://claimsprovider.local/FederationMetadata",
            //        Wtrealm = "urn:medewerker",
            //        Wreply = "http://multisite.local/sitecore/shell",
            //    });
            //});

        }

        private void HandleException(CookieExceptionContext exception)
        {
            var ex = exception;
        }

        

        //private WsFederationAuthenticationOptions GetAuthenticationOptions(KeyValuePair<string, FederationConfiguration> node)
        //{
        //    return new WsFederationAuthenticationOptions
        //    {
        //        MetadataAddress = "http://claimsprovider.local/FederationMetadata",
        //        Wtrealm = node.Value.Realm,
        //        Wreply = "http://" + node.Value.Hostname + "/sample item",
        //        Notifications = new WsFederationAuthenticationNotifications
        //        {
        //            SecurityTokenValidated = context => LoginSitecoreUser(context)
        //        }
        //    };
        //}

        private bool MapDomain(IOwinContext ctx, string hostname)
        {
            if (ctx.Request.Headers.Get("Host").Equals(hostname))
                return true;
            return false;
        }

        async Task<SecurityTokenValidatedNotification<WsFederationMessage, WsFederationAuthenticationOptions>> LoginSitecoreUser(SecurityTokenValidatedNotification<WsFederationMessage, WsFederationAuthenticationOptions> context)
        {

            //IPrincipal claimsPrincipal = new ClaimsPrincipal(context.AuthenticationTicket.Identity);
            //// store claims in session
            //IEnumerable<Claim> claims = context.AuthenticationTicket.Identity.Claims;
            //ClientContext.SetValue("bla", "diebla");
            //LoginHelper loginHelper = new LoginHelper();
            //loginHelper.Login(claimsPrincipal);


            return context;
        }

        //private void PrintCurrentIntegratedPipelineStage(IOwinContext context, string v)
        //{
        //    var currentIntegratedpipelineStage = HttpContext.Current.CurrentNotification;
        //    context.Get<TextWriter>("host.TraceOutput").WriteLine(
        //        "Current IIS event: " + currentIntegratedpipelineStage
        //        + " Msg: " + v);
        //}

        //private void OnResponseSignIn(CookieResponseSignInContext responseSignIn)
        //{
        //    var sitecoreCtx = Context.User;
        //}

        //private void SignedIn(CookieResponseSignedInContext ctx)
        //{
        //    var sitecoreCtx = Context.User;
        //}               
    }  

    //public class FederationConfiguration
    //{
    //    public string Hostname { get; set; }
    //    public string Realm { get; set; }
    //}
}
