using EngineAPI.Localize;
using EngineAPI.Resources;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using System.Globalization;

namespace EngineAPI.Controllers
{

    //[Route("{culture:culture}/[controller]")]
    [Route("[controller]")]
    [ApiController]
    public class AboutController : ControllerBase
    {
        private readonly IStringLocalizer<Resourcex> localizer;
        public AboutController(IStringLocalizer<Resourcex> localizer)
        {
            this.localizer = localizer;
        }

        [HttpGet]
        public string Get(string requestCultureInfo)
        {
            CultureInfo cultureInfo = new CultureInfo(requestCultureInfo); // (request.CultureInfo);
            Resource.Culture = cultureInfo;

            return Resource.About;// localizer["About"];
        }
    }

}
