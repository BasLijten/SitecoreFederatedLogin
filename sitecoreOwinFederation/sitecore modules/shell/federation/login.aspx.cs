using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Microsoft.Owin.Security;
using Sitecore;
using SitecoreOwinFederator.Authenticator;
using SitecoreOwinFederator.Pipelines.HttpRequest;

namespace SitecoreOwinFederator.sitecore_modules.shell.federation
{    
    public partial class login : System.Web.UI.Page
    {
        private const string StartUrl = "/sitecore/shell/default.aspx";
        protected void Page_Load(object sender, EventArgs e)
        {
            var domain = Sitecore.Context.Domain;

            //var properties = new AuthenticationProperties();
            HttpContext.Current.GetOwinContext().Authentication.Challenge();


            var principal = IdentityHelper.GetCurrentClaimsPrincipal();

            // Login the sitecore user with the claims identity that was provided by identity ticket
            LoginHelper loginHelper = new LoginHelper();
            loginHelper.Login(principal);
            //else
            //{
            //    var returnUrl = HttpUtility.ParseQueryString(ctx.QueryString.ToString()).Get("returnUrl");
            //    if (returnUrl.Contains("sitecore/shell"))
            //        returnUrl = StartUrl;
            //    //WriteCookie("sitecore_starturl", StartUrl);
            //    //WriteCookie("sitecore_starttab", "advanced");
            //    Response.Redirect(returnUrl);
            //}            
        }
    }
}