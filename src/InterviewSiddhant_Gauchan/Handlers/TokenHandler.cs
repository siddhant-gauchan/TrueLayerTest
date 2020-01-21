using TrulayerApiTest.Helpers;
using TrulayerApiTest.Services;
using MediatR;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using TrulayerApiTest.Handlers.Query;

namespace TrulayerApiTest.Handlers
{
    public interface ITokenHandler: IRequestHandler<GetTokenQuery, TokenRespose>
    {
       
        Task<TokenDetails> GetTokenByRefreshToken();
    }
    public class TokenHandler : ITokenHandler
    {
        private readonly IHttpHelpers httpHelpers;
        private readonly IOptions<Config> config;
        private readonly IStorage tokenStorage;
        private readonly IUserService userService;

        public TokenHandler(IHttpHelpers httpHelpers, IOptions<Config> config, IStorage tokenStorage,IUserService userService)
        {
            this.httpHelpers = httpHelpers;
            this.config = config;
            this.tokenStorage = tokenStorage;
            this.userService = userService;
        }
          public async Task<TokenDetails> GetTokenByRefreshToken()
        {
            //TODO : It returns badRequest. Need to fix it             
            var content = new FormUrlEncodedContent(new[]
            {
             new KeyValuePair<string, string>("client_id", config.Value.ClientId)
            ,new KeyValuePair<string, string>("client_secret", config.Value.ClientSecret)
            ,new KeyValuePair<string, string>("grant_type", "refresh_token")
            ,new KeyValuePair<string, string>("refresh_token", tokenStorage.Get<TokenDetails>(config.Value.TokenKey).RefreshToken)

            });


            var client = httpHelpers.GetClient(config.Value.OAuthServerUrl);
            var response = await client.PostAsync("/connect/token", content);
            if (response.IsSuccessStatusCode)
            {
                dynamic result = JsonConvert.DeserializeObject<dynamic>(await response.Content.ReadAsStringAsync());
                var jwtSecurityHandler = new JwtSecurityTokenHandler();
                var user = (JwtSecurityToken)jwtSecurityHandler.ReadJwtToken(result.access_token.ToString());

                var tokens = new TokenDetails
                {
                    AccessToken = result.access_token,
                    RefreshToken = result.refresh_token,
                    ExpiryDate = FromUnixTime(user.Claims.Where(x => x.Type == "exp").FirstOrDefault().Value)
                };

                tokenStorage.Clear();
                tokenStorage.Store(tokens, config.Value.TokenKey);
                return tokens;
            }
            return new TokenDetails();

        }

        public DateTime FromUnixTime(string unixTime)
        {
            var epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            return epoch.AddSeconds(Convert.ToDouble(unixTime));
        }

        public async Task<TokenRespose> Handle(GetTokenQuery request, CancellationToken cancellationToken)
        {
            var content = new FormUrlEncodedContent(new[]
           {
             new KeyValuePair<string, string>("grant_type", "authorization_code")
            ,new KeyValuePair<string, string>("code", request.AccessCode)
            ,new KeyValuePair<string, string>("redirect_uri", config.Value.RedirectUri)
            ,new KeyValuePair<string, string>("client_id", config.Value.ClientId)
            ,new KeyValuePair<string, string>("client_secret", config.Value.ClientSecret)
            });

            var client = httpHelpers.GetClient(config.Value.OAuthServerUrl);
            var response = await client.PostAsync("/connect/token", content);
            if (response.IsSuccessStatusCode)
            {
                dynamic result = JsonConvert.DeserializeObject<dynamic>(await response.Content.ReadAsStringAsync());
                var jwtSecurityHandler = new JwtSecurityTokenHandler();
                var token = (JwtSecurityToken)jwtSecurityHandler.ReadJwtToken(result.access_token.ToString());


                var tokens = new TokenDetails
                {
                    AccessToken = result.access_token,
                    RefreshToken = request.AccessCode,
                    ExpiryDate = FromUnixTime(token.Claims.Where(x => x.Type == "exp").FirstOrDefault().Value)
                };

                tokenStorage.Clear();
                tokenStorage.Store(tokens, config.Value.TokenKey);

                var user = await userService.GetInfo();
                tokenStorage.Store(user, "userInfo");

                return new TokenRespose { Response = tokens };
            }
            return new TokenRespose();
        }
    }
}
