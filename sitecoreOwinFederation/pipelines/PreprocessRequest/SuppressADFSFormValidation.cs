using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Sitecore.Diagnostics;
using Sitecore.Pipelines.PreprocessRequest;

namespace SitecoreOwinFederator.pipelines.PreprocessRequest
{
    public class SuppressAdfsFormValidation : PreprocessRequestProcessor
    {
        public override void Process(PreprocessRequestArgs args)
        {
            Assert.ArgumentNotNull(args, "args");
            try
            {
                new SuppressFormValidation().Process(args);
            }
            catch (HttpRequestValidationException)
            {
                string rawUrl = args.Context.Request.RawUrl;
                if (!rawUrl.Contains("sample item") && !rawUrl.Contains("secure") && !rawUrl.Contains("login"))
                {
                    throw;
                }
            }
        }
    }
}