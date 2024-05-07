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
    public class SignUp(ILogger<SignUp> logger, UserManager<UserAccountEntity> userManager)
    {
        private readonly ILogger<SignUp> _logger = logger;
        private readonly UserManager<UserAccountEntity> _userManager = userManager;

        [Function("SignUp")]
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
                SignUpRequest signUpRequest = null!;

                try
                {
                    signUpRequest = JsonConvert.DeserializeObject<SignUpRequest>(body)!;
                }
                catch (Exception ex)
                {
                    _logger.LogError($"JsonConvert.DeserializeObject<SignUpRequest> :: {ex.Message}");
                }



                if (signUpRequest != null && !string.IsNullOrEmpty(signUpRequest.FirstName) && !string.IsNullOrEmpty(signUpRequest.LastName) && !string.IsNullOrEmpty(signUpRequest.Email) && !string.IsNullOrEmpty(signUpRequest.Password))
                {
                    if (!await _userManager.Users.AnyAsync(x => x.Email == signUpRequest.Email))
                    {
                        var userAccount = new UserAccountEntity
                        {
                            FirstName = signUpRequest.FirstName,
                            LastName = signUpRequest.LastName,
                            Email = signUpRequest.Email,
                            UserName = signUpRequest.Email
                        };

                        try
                        {
                            var result = await _userManager.CreateAsync(userAccount, signUpRequest.Password);
                            if (result.Succeeded)
                            {
                                /*
                                http request, måste vänta på svar 
                                 
                                using var http = new HttpClient();
                                string content = new StringContent(JsonConvert.SerializeObject(new { Email = "userAccount.Email"}), Encoding.UTF8, "application/json" );
                                var response = await http.postasync("https://verificationprovider/api/generate", content);
                                 
                                 */

                                return new OkResult();
                            }
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError($"_userManager.CreateAsync :: {ex.Message}");
                        }
                    }
                    else
                    {
                        return new ConflictResult();
                    }
                }
            }
            return new BadRequestResult();
        }
    }
}
