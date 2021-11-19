using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;
using System.Threading;

namespace EngineAPI.Controllers
{
    public class AppBaseController : ControllerBase
    {
        internal CultureInfo GetServerCulture()
        {
            // var dd =  HttpContext.Request.GetTypedHeaders().AcceptLanguage;
            //var rqf = Request.HttpContext.Features.Get<IRequestCultureFeature>();
            //var culture = rqf.RequestCulture.Culture;

            //CultureInfo uiCultureInfo = Thread.CurrentThread.CurrentUICulture; 
            return Thread.CurrentThread.CurrentCulture;
        }
        internal CultureInfo GetRequestCulture()
        {
            var userLanguages = HttpContext.Request.GetTypedHeaders().AcceptLanguage;
            var currentLanguage = userLanguages[0].Value;
            return new CultureInfo(currentLanguage.Value);
        }
    }
}
