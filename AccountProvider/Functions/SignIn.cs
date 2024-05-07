using AccountProvider.Models;
using Data.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace AccountProvider.Functions
{
    public class SignIn(ILogger<SignIn> logger, UserManager<UserAccountEntity> userManager, SignInManager<UserAccountEntity> signInManager)
    {
        private readonly ILogger<SignIn> _logger = logger;
        private readonly UserManager<UserAccountEntity> _userManager = userManager;
        private readonly SignInManager<UserAccountEntity> _signInManager = signInManager;

        [Function("SignIn")]
        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequest req)
        {
            string body = null!;
            try
            {
                body = await new StreamReader(req.Body).ReadToEndAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError($"StreamReader :: {ex.Message}");
            }

            if (body != null)
            {
                SignInRequest signInRequest = null!;

                try
                {
                    signInRequest = JsonConvert.DeserializeObject<SignInRequest>(body)!;
                }
                catch (Exception ex)
                {
                    _logger.LogError($"JsonConvert.DeserializeObject<SignInRequest> :: {ex.Message}");
                }

                if (signInRequest != null && !string.IsNullOrEmpty(signInRequest.Email) && !string.IsNullOrEmpty(signInRequest.Password))
                {
                    try
                    {
                        var result = await _signInManager.PasswordSignInAsync(signInRequest.Email, signInRequest.Password, signInRequest.IsPersistent, false);
                        if (result.Succeeded)
                        {
                            //get token from tokenprovider
                            return new OkObjectResult("accesstoken");
                        }

                        return new UnauthorizedResult();
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError($"_signInManager.PasswordSignInAsync :: {ex.Message}");
                    }
                }
            }


            return new BadRequestResult();
        }
    }
}
