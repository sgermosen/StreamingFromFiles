using AutoMapper;
using Domain;
using Domain.Entities;
using EngineAPI.Models;
using EngineAPI.Resources;
using EngineAPI.Utils;
//using BC = BCrypt.Net.BCrypt;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace EngineAPI.Services
{
    public interface IAccountService
    {
        Task<UserManagerResponse> RegisterUserAsync(RegisterViewModel model);

        Task<UserManagerResponse> LoginUserAsync(LoginViewModel model);

        Task<UserManagerResponse> ConfirmEmailAsync(string userId, string token);

        Task<UserManagerResponse> ForgetPasswordAsync(string email);

        Task<UserManagerResponse> ResetPasswordAsync(ResetPasswordViewModel model);

    }

    public class AccountService : IAccountService
    {
        private readonly IStorageManager _storageSaver;
        private readonly ApplicationDataContext _context;
      //  private readonly IMapper _mapper;
        private readonly AppSettings _appSettings;
        private readonly IMailHelper _emailService;
        private IConfiguration _configuration;
        private UserManager<ApplicationUser> _userManger;
        public AccountService(
            ApplicationDataContext context,
           // IMapper mapper,
            IOptions<AppSettings> appSettings, IConfiguration configuration,
            IMailHelper emailService, UserManager<ApplicationUser> userManager, IStorageManager storageSaver)
        {
            _storageSaver = storageSaver;
            _configuration = configuration;
            _context = context;
         //   _mapper = mapper;
            _appSettings = appSettings.Value;
            _emailService = emailService;
            _userManger = userManager;
        }


        public async Task<UserManagerResponse> RegisterUserAsync(RegisterViewModel model)
        {
            if (model == null)
                throw new NullReferenceException(Resource.ModelNull);

            if (model.Password != model.ConfirmPassword)
                return new UserManagerResponse
                {
                    Message = Resource.ConfirmPassDosntMatch,
                    IsSuccess = false,
                };

            var identityUser = new ApplicationUser
            {
                EmployeeNumber = model.EmployeeNumber,
                StoreCode = model.StoreCode,
                Identification = model.Identification,
                FullName = model.FullName,
                Email = model.Email,
                UserName = model.Email
            };

            var result = await _userManger.CreateAsync(identityUser, model.Password);

            if (result.Succeeded)
            {
                if (model.ImageFile != null)
                {
                    var cardPicture = new Image { UserId = identityUser.Id, TypeId = 1 };
                    cardPicture.ImageUrl = await _storageSaver.SaveFile("personalidentification", model.ImageFile);
                    _context.Images.Add(cardPicture);
                    await _context.SaveChangesAsync();
                }
                var confirmEmailToken = await _userManger.GenerateEmailConfirmationTokenAsync(identityUser);

                var encodedEmailToken = Encoding.UTF8.GetBytes(confirmEmailToken);
                var validEmailToken = WebEncoders.Base64UrlEncode(encodedEmailToken);

                string url = $"{_configuration["AppUrl"]}/api/account/confirmemail?userid={identityUser.Id}&token={validEmailToken}";

                _emailService.SendMail(identityUser.Email, Resource.ConfirmEmail, $"<h1>{Resource.WelcomeTo} {StaticValues.AppName}</h1>" +
                  $"<p>{Resource.ConfirmEmailMsg} <a href='{url}'>{Resource.ClickHere}</a></p>");

                return new UserManagerResponse
                {
                    Message = Resource.UserCreated ,
                    IsSuccess = true,
                };
            }

            return new UserManagerResponse
            {
                Message = Resource.UserNotCreated,
                IsSuccess = false,
                Errors = result.Errors.Select(e => e.Description)
            };

        }

        public async Task<UserManagerResponse> LoginUserAsync(LoginViewModel model)
        {
            var user = await _userManger.FindByEmailAsync(model.Email);

            if (user == null)
            {
                return new UserManagerResponse
                {
                    Message = Resource.NoUserWithThatEmail ,
                    IsSuccess = false,
                };
            }

            var result = await _userManger.CheckPasswordAsync(user, model.Password);

            if (!result)
                return new UserManagerResponse
                {
                    Message = Resource.InvalidPassword,
                    IsSuccess = false,
                };

            var claims = new[]
            {
                new Claim("Email", model.Email),
                new Claim(ClaimTypes.NameIdentifier, user.Id),
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["AuthSettings:Key"]));

            var token = new JwtSecurityToken(
                issuer: _configuration["AuthSettings:Issuer"],
                audience: _configuration["AuthSettings:Audience"],
                claims: claims,
                expires: DateTime.Now.AddDays(30),
                signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256));

            string tokenAsString = new JwtSecurityTokenHandler().WriteToken(token);

            return new UserManagerResponse
            {
                Message = tokenAsString,
                IsSuccess = true,
                ExpireDate = token.ValidTo
            };
        }

        public async Task<UserManagerResponse> ConfirmEmailAsync(string userId, string token)
        {
            var user = await _userManger.FindByIdAsync(userId);
            if (user == null)
                return new UserManagerResponse
                {
                    IsSuccess = false,
                    Message = Resource.UserNotFound
                };

            var decodedToken = WebEncoders.Base64UrlDecode(token);
            string normalToken = Encoding.UTF8.GetString(decodedToken);

            var result = await _userManger.ConfirmEmailAsync(user, normalToken);

            if (result.Succeeded)
                return new UserManagerResponse
                {
                    Message = Resource.EmailConfirmed,
                    IsSuccess = true,
                };

            return new UserManagerResponse
            {
                IsSuccess = false,
                Message = Resource.EmailNotConfirmed,
                Errors = result.Errors.Select(e => e.Description)
            };
        }

        public async Task<UserManagerResponse> ForgetPasswordAsync(string email)
        {
            var user = await _userManger.FindByEmailAsync(email);
            if (user == null)
                return new UserManagerResponse
                {
                    IsSuccess = false,
                    Message = Resource.NoUserWithThatEmail,
                };

            var token = await _userManger.GeneratePasswordResetTokenAsync(user);
            var encodedToken = Encoding.UTF8.GetBytes(token);
            var validToken = WebEncoders.Base64UrlEncode(encodedToken);

            string url = $"{_configuration["AppUrl"]}/ResetPassword?email={email}&token={validToken}";

            _emailService.SendMail(email, Resource.ResetPass, $"<h1>{Resource.FallowToReset}</h1>" +
                $"<p> {Resource.ToResetMsg} <a href='{url}'>{Resource.ClickHere}</a></p>");

            return new UserManagerResponse
            {
                IsSuccess = true,
                Message = Resource.ResetPassUrlWasSend
            };
        }

        public async Task<UserManagerResponse> ResetPasswordAsync(ResetPasswordViewModel model)
        {
            var user = await _userManger.FindByEmailAsync(model.Email);
            if (user == null)
                return new UserManagerResponse
                {
                    IsSuccess = false,
                    Message = Resource.NoUserWithThatEmail,
                };

            if (model.NewPassword != model.ConfirmPassword)
                return new UserManagerResponse
                {
                    IsSuccess = false,
                    Message = Resource.ConfirmPassDosntMatch,
                };

            var decodedToken = WebEncoders.Base64UrlDecode(model.Token);
            string normalToken = Encoding.UTF8.GetString(decodedToken);

            var result = await _userManger.ResetPasswordAsync(user, normalToken, model.NewPassword);

            if (result.Succeeded)
                return new UserManagerResponse
                {
                    Message = Resource.PasswordWasReset,
                    IsSuccess = true,
                };

            return new UserManagerResponse
            {
                Message = Resource.SomethingWrong,
                IsSuccess = false,
                Errors = result.Errors.Select(e => e.Description),
            };
        }

        private string generateJwtToken(Account account)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_appSettings.Secret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[] { new Claim("id", account.Id.ToString()) }),
                Expires = DateTime.UtcNow.AddMinutes(15),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        private RefreshToken generateRefreshToken(string ipAddress)
        {
            return new RefreshToken
            {
                Token = randomTokenString(),
                Expires = DateTime.UtcNow.AddDays(7),
                Created = DateTime.UtcNow,
                CreatedByIp = ipAddress
            };
        }

        private void removeOldRefreshTokens(Account account)
        {
            account.RefreshTokens.RemoveAll(x =>
                !x.IsActive &&
                x.Created.AddDays(_appSettings.RefreshTokenTTL) <= DateTime.UtcNow);
        }

        private string randomTokenString()
        {
            using var rngCryptoServiceProvider = new RNGCryptoServiceProvider();
            var randomBytes = new byte[40];
            rngCryptoServiceProvider.GetBytes(randomBytes);
            // convert random bytes to hex string
            return BitConverter.ToString(randomBytes).Replace("-", "");
        }

        private void sendVerificationEmail(Account account, string origin)
        {
            string message;
            if (!string.IsNullOrEmpty(origin))
            {
                var verifyUrl = $"{origin}/account/verify-email?token={account.VerificationToken}";
                message = $@"<p>{Resource.BelowLink}</p>
                             <p><a href=""{verifyUrl}"">{verifyUrl}</a></p>";
            }
            else
            {
                message = $@"<p>{Resource.BelowToken} <code>/accounts/verify-email</code> api route:</p>
                             <p><code>{account.VerificationToken}</code></p>";
            }

            _emailService.SendMail(
                to: account.Email,
                subject: Resource.SignUpVerification,
                body: $@"<h4>{Resource.VerifyEmail}</h4>
                         <p>{Resource.ThanksForRegister}</p>
                         {message}"
            );
        }

        private void sendAlreadyRegisteredEmail(string email, string origin)
        {
            string message;
            if (!string.IsNullOrEmpty(origin))
                message = $@"<p>If you don't know your password please visit the <a href=""{origin}/account/forgot-password"">forgot password</a> page.</p>";
            else
                message = "<p>If you don't know your password you can reset it via the <code>/accounts/forgot-password</code> api route.</p>";

            _emailService.SendMail(
                to: email,
                subject: "Sign-up Verification API - Email Already Registered",
                body: $@"<h4>Email Already Registered</h4>
                         <p>Your email <strong>{email}</strong> is already registered.</p>
                         {message}"
            );
        }

        private void sendPasswordResetEmail(Account account, string origin)
        {
            string message;
            if (!string.IsNullOrEmpty(origin))
            {
                var resetUrl = $"{origin}/account/reset-password?token={account.ResetToken}";
                message = $@"<p>Please click the below link to reset your password, the link will be valid for 1 day:</p>
                             <p><a href=""{resetUrl}"">{resetUrl}</a></p>";
            }
            else
            {
                message = $@"<p>Please use the below token to reset your password with the <code>/accounts/reset-password</code> api route:</p>
                             <p><code>{account.ResetToken}</code></p>";
            }

            _emailService.SendMail(
                to: account.Email,
                subject: "Sign-up Verification API - Reset Password",
                body: $@"<h4>Reset Password Email</h4>
                         {message}"
            );
        }
    }
}
