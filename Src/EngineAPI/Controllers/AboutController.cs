using AutoMapper;
using Domain;
using EngineAPI.Resources;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;

namespace EngineAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class AboutController : AppBaseController
    {
        public AboutController(IMapper mapper, ApplicationDataContext context) : base(mapper, context)
        {
        }

        [HttpPost]
        //[ValidateAntiForgeryToken]
        [HttpPost("setCulture")]
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
            var user = GetConectedUser();
            return Resource.About;
        }


    }

}
