using Microsoft.AspNetCore.Mvc;
using System.Globalization;
using System.Threading;

namespace EngineAPI.Controllers
{
    public class AppBaseController : ControllerBase
    {
        public CultureInfo GetCurrentCulture()
        { 
            //var rqf = Request.HttpContext.Features.Get<IRequestCultureFeature>();
            //var culture = rqf.RequestCulture.Culture;

            //CultureInfo uiCultureInfo = Thread.CurrentThread.CurrentUICulture;
            return Thread.CurrentThread.CurrentCulture;
        }

    }
}
