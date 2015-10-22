using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Web;

namespace SitecoreOwinFederator.Models
{
    /// <summary>
    /// claims model to displau in rendering. Can be removed for production
    /// </summary>
    public class UserClaimsModel
    {
        public IEnumerable<Claim> Claims { get; set; }
    }    
}