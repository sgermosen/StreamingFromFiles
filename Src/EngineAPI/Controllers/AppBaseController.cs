using AutoMapper;
using Domain;
using Domain.Entities;
using EngineAPI.Services;
using EngineAPI.Utils;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace EngineAPI.Controllers
{
    public class AppBaseController : ControllerBase
    {
        public readonly UserManager<ApplicationUser> UserManager;
        public readonly ApplicationDataContext Context;
        public readonly IMapper Mapper;
        public readonly IStorageSaver StorageSaver;

        public readonly SignInManager<ApplicationUser> SignInManager;
        public readonly IMailHelper MailHelper;
        public readonly IAccountService UserService;

        public AppBaseController(UserManager<ApplicationUser> userManager,
      IMapper mapper, ApplicationDataContext context,
      SignInManager<ApplicationUser> signInManager, IMailHelper mailHelper, IAccountService userService)
        {
            UserService = userService;
            UserManager = userManager;
            Mapper = mapper;
            Context = context;
            SignInManager = signInManager;
            MailHelper = mailHelper;
        }


        public AppBaseController(UserManager<ApplicationUser> userManager,
         IMapper mapper,
         IStorageSaver storageSaver,
           ApplicationDataContext context)
        {
            this.UserManager = userManager;
            this.Mapper = mapper;
            this.StorageSaver = storageSaver;
            this.Context = context;
        }

        public AppBaseController(UserManager<ApplicationUser> userManager,
         IMapper mapper,
           ApplicationDataContext context)
        {
            this.UserManager = userManager;
            this.Mapper = mapper;
            this.Context = context;
        }

        internal async Task<ApplicationUser> GetConectedUser()
        {
            var email = HttpContext.User.Claims.FirstOrDefault(x => x.Type == "email")?.Value;
            if (string.IsNullOrEmpty(email))
                return new ApplicationUser();
            var user = await UserManager.FindByEmailAsync(email);
            return user;
        }

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
