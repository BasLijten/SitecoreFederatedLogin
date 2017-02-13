using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Security.Principal;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.DataHandler;
using Microsoft.Owin.Security.Infrastructure;
using Microsoft.Owin.Security.WsFederation;
using Sitecore;
using Sitecore.Analytics;
using Sitecore.Configuration;
using Sitecore.Security.Authentication;
using SitecoreOwinFederator.Authenticator;
using SitecoreOwinFederator.Models;
using SitecoreOwinFederator.Pipelines.HttpRequest;

namespace SitecoreOwinFederator.Controllers
{
    /// <summary>
    /// Authentication controller, contains login and logout functionality.
    /// THe authorize attribute on the Index Action forces OWIN to trigger an ASP.Net authenticaiton challenge
    /// </summary>
    public class AuthController : Controller
    {
        // GET: Auth
        [Authorize]
        public ActionResult Index()
        {            
            // Get ID ticket from .ASP.Net cookie. This ticket doesnt contain an identity, 
            // but a reference to the identity in the Session Store                          
            var principal = IdentityHelper.GetCurrentClaimsPrincipal();

            var ctx = Tracker.Current.Session;
            // Login the sitecore user with the claims identity that was provided by identity ticket
            LoginHelper loginHelper = new LoginHelper();
            loginHelper.Login(principal);

            ctx = Tracker.Current.Session;

            // temporary code to show user claims, while there is a sitecore user object as
            UserClaimsModel ucm = new UserClaimsModel();
            ucm.Claims = ((ClaimsPrincipal)principal).Claims;
            return View(ucm);
        }
        
        /// <summary>
        /// Logs out user 
        /// </summary>
        /// <returns></returns>
        public ActionResult Logout()
        {
            AuthenticationProperties properties = new AuthenticationProperties();                        
            properties.RedirectUri = "/login";
            properties.AllowRefresh = false;
            AuthenticationManager.Logout();
            Request.GetOwinContext().Authentication.SignOut(properties);
            return View();         
        }                
    }

   
}