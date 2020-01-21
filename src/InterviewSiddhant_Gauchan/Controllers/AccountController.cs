using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using TrulayerApiTest.Handlers;
using TrulayerApiTest.Helpers;
using TrulayerApiTest.Model;
using MediatR;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using TrulayerApiTest.Handlers.Query;

namespace TrulayerApiTest.Controllers
{
    [ApiController]
    public class AccountController : ControllerBase
    {
        private ITokenHandler tokenHandler;
        private readonly IOptions<Config> config;
        private readonly IStorage storage;
        private readonly IMediator mediator;

        public AccountController(ITokenHandler tokenHandler, IOptions<Config> config,IStorage storage, IMediator mediator)
        {
            this.tokenHandler = tokenHandler;
            this.config = config;
            this.storage = storage;
            this.mediator = mediator;
        }

        [Route("Account/Login")]
        [HttpGet]
        [ApiExplorerSettings(IgnoreApi = true)]
        //used for manaual testing 
        public ActionResult Login()
        {
            try
            {
                var query = new QueryBuilder()
            {
                {"response_type","code"},
                //{"response_mode","form_post"},
                {"client_id",$"{config.Value.ClientId}"},
                {"scope", "transactions accounts offline_access info"},
                {"redirect_uri",$"{config.Value.RedirectUri}"},
                {"enable_mock","true"}
            };
                var url = $"{config.Value.OAuthServerUrl}/{query}";

                return Redirect(url);
            }
            catch (System.Exception ex)
            {

                return StatusCode(StatusCodes.Status500InternalServerError, ex);
            }

        }

        [Route("callback")]
        [HttpGet]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<IActionResult> Callback([FromQuery]string code)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(code))
                    return Ok("error");
                {
                    var result = await mediator.Send(new GetTokenQuery { AccessCode = code});                   
                    var user = storage.Get<List<UserModel>>("userInfo");
                    
                    var identity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme, ClaimTypes.Name, ClaimTypes.Role);
                    identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, user[0].FullName));
                    identity.AddClaim(new Claim(ClaimTypes.Name, user[0].FullName));
                    identity.AddClaim(new Claim(ClaimTypes.Email, string.Join(",", user[0].Emails)));
                    identity.AddClaim(new Claim(ClaimTypes.MobilePhone, string.Join(",",user[0].Phones)));
                    identity.AddClaim(new Claim(ClaimTypes.Expiration, string.Join(",", result.Response.ExpiryDate)));
                    identity.AddClaim(new Claim("RefreshToken", string.Join(",", result.Response.RefreshToken)));
                    var principal = new ClaimsPrincipal(identity);

                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal, 
                    new AuthenticationProperties 
                    { 
                        IsPersistent = true, 
                        ExpiresUtc = DateTime.UtcNow.AddMinutes(20) 
                    });
                    return Ok(result.Response);
                }

            }
            catch (System.Exception ex)
            {

                return StatusCode(StatusCodes.Status500InternalServerError, ex);
            }


        }

        [Route("refreshtoken")]
        [HttpGet]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<IActionResult> RefreshToken()
        {
            var accessToken = await tokenHandler.GetTokenByRefreshToken();
            return Ok(accessToken);

        }
    }
}