using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SitecoreOwinFederator.sitecore_modules.shell.federation
{
    public partial class login : System.Web.UI.Page
    {
        private const string StartUrl = "/sitecore/shell/default.aspx";
        protected void Page_Load(object sender, EventArgs e)
        {                        
            var ctx = HttpContext.Current.Request;
            if(HttpContext.Current.User== null)
                HttpContext.Current.GetOwinContext().Authentication.Challenge();
            else
            {
                var returnUrl = HttpUtility.ParseQueryString(ctx.QueryString.ToString()).Get("returnUrl");
                if (returnUrl.Contains("sitecore/shell"))
                    returnUrl = StartUrl;
                //WriteCookie("sitecore_starturl", StartUrl);
                //WriteCookie("sitecore_starttab", "advanced");
                Response.Redirect(returnUrl);
            }            
        }
    }
}