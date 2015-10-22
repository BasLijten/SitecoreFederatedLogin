using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Web;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.DataHandler;
using Microsoft.Owin.Security.WsFederation;

namespace SitecoreOwinFederator.Authenticator
{
    /// <summary>
    /// Helper class which helps to retrieve the claimsprincipal from the .AspNet.Cookies cookie
    /// </summary>
    public static class IdentityHelper
    {
        public static string GetAuthTokenFromCookie()
        {
            string authKey = String.Empty;
            var ticket = GetAuthenticationKeyTicket();
            if(ticket!=null)
                authKey = ticket.Identity.Claims.Where(claim => claim.Type.Equals("Microsoft.Owin.Security.Cookies-SessionId")).FirstOrDefault().Value;        
            return authKey;
        }

        private static AuthenticationTicket GetAuthenticationTicketByKey(string key)
        {
            if (String.IsNullOrEmpty(key))
                throw new ArgumentNullException(key);
            
            SqlAuthSessionStore store = new SqlAuthSessionStore(new TicketDataFormat(new MachineKeyProtector()));
            return store.RetrieveAsync(key).Result;
        }

        private static AuthenticationTicket GetAuthenticationTicketForCurrentUser()
        {
            AuthenticationTicket ticket = null;
            string authKey = GetAuthTokenFromCookie();
            if(!String.IsNullOrEmpty(authKey))
                ticket = GetAuthenticationTicketByKey(authKey);
            return ticket;
        }

        public static IPrincipal GetCurrentClaimsPrincipal()
        {
            var ticket = GetAuthenticationTicketForCurrentUser();
            IPrincipal p = null;
            if (ticket != null && ticket.Identity != null)
                p = new ClaimsPrincipal(ticket.Identity);            
            return p;
        }

        public static IEnumerable<Claim> GetClaimsForCurrentUser()
        {
            var identity = GetAuthTokenForCurrentUser();
            if(identity!=null)
                return GetAuthTokenForCurrentUser().Claims;
            return null;
        }

        private static ClaimsIdentity GetAuthTokenForCurrentUser()
        {
            AuthenticationTicket ticket = GetAuthenticationKeyTicket();
            if(ticket!=null)
                return ticket.Identity;
            return null;
        }

        private static AuthenticationTicket GetAuthenticationKeyTicket()
        {
            AuthenticationTicket ticket = null;

            var ctx = HttpContext.Current.Request;
            if (ctx.Cookies != null && ctx.Cookies[".AspNet.Cookies"] != null)
            {
                var cookie = ctx.Cookies[".AspNet.Cookies"];
                var secureDataFormat = new TicketDataFormat(new MachineKeyProtector());
                ticket = secureDataFormat.Unprotect(cookie.Value);
            }
            return ticket;            
        }        
    }
}