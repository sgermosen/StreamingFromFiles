using EngineAPI.Resources;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;

namespace EngineAPI.Controllers
{

    //[Route("{culture:culture}/[controller]")]
    [Route("[controller]")]
    [ApiController]
    public class AboutController : AppBaseController
    {
        public AboutController()
        {
        }

        [HttpPost]
        //[ValidateAntiForgeryToken]
        public IActionResult SetCulture(string culture)//, string returnUrl)
        {
            HttpContext.Response.Cookies.Append(
                CookieRequestCultureProvider.DefaultCookieName,
                CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(culture)),
                new CookieOptions { Path = Url.Content("~/") });

            //if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
            //{
            //    return LocalRedirect(returnUrl);
            //} 
            //   return RedirectToAction("Index", "Home");
            return Ok();
        }

        [HttpGet]
        public string Get(string requestCultureInfo)
        {

            return Resource.About;// localizer["About"];
        }


    }

}
