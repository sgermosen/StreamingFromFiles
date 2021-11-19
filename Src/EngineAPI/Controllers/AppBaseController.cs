using EngineAPI.Resources;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;
using System.Threading;

namespace EngineAPI.Controllers
{
    public class AppBaseController : ControllerBase
    {
        public AppBaseController()
        {
            //CultureInfo cultureInfo = Thread.CurrentThread.CurrentCulture;

            //var rqf = Request.HttpContext.Features.Get<IRequestCultureFeature>();
            //var culture = rqf.RequestCulture.Culture;

            //CultureInfo uiCultureInfo = Thread.CurrentThread.CurrentUICulture;
              
        }
        
        
    }
}
