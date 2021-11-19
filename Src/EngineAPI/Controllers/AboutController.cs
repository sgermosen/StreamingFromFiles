using EngineAPI.Localize;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace EngineAPI.Controllers
{

    [Route("{culture:culture}/[controller]")]
    [ApiController]
    public class AboutController : ControllerBase
    {
        private readonly IStringLocalizer<Resourcex> localizer;
        public AboutController(IStringLocalizer<Resourcex> localizer)
        {
            this.localizer = localizer;
        }

        [HttpGet]
        public string Get()
        {
            return localizer["About"];
        }
    }

}
