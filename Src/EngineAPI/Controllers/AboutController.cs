using EngineAPI.Resources;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;

namespace EngineAPI.Controllers
{ 
    [Route("[controller]")]
    [ApiController]
    public class AboutController : AppBaseController
    {

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
        public string Get()
        { 
            return Resource.About;
        }


    }

}
