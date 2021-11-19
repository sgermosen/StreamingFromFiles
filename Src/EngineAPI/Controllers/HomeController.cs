using EngineAPI.Localize;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace EngineAPI.Controllers
{
    // [Route("{culture:culture}/[controller]")]
    [Route("[controller]")]
    [ApiController]
    public class HomeController : ControllerBase
    {
        private readonly IStringLocalizer<Resourcex> localizer;
        public HomeController(IStringLocalizer<Resourcex> localizer)
        {
            this.localizer = localizer;
        }
        [HttpGet]
        public string Get()
        {
            return localizer["Home"];
        }
    }
}
