using TrulayerApiTest.Helpers;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace TrulayerApiTest.Services
{
    public class AccountService : IAccountService
    {
        private readonly IHttpHelpers httpHelpers;
        private readonly IOptions<Config> config;
        private readonly IStorage tokenStorage;

        public AccountService(IHttpHelpers httpHelpers, IOptions<Config> config, IStorage tokenStorage)
        {
            this.httpHelpers = httpHelpers;
            this.config = config;
            this.tokenStorage = tokenStorage;
        }



        public async Task<List<AccountModel>> GetAllBankAccounts()
        {
            var token= tokenStorage.Get<TokenDetails>(config.Value.TokenKey);

            var client = httpHelpers.GetClient(config.Value.ResourceApi);
            var authValue = new AuthenticationHeaderValue("Bearer", token.AccessToken);
           
            client.DefaultRequestHeaders.Authorization = authValue;

            var response = await client.GetAsync("/data/v1/accounts");
            if(response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadAsStringAsync();
                var jObject = JObject.Parse(result);

               return jObject["results"].ToObject<List<AccountModel>>();
               
            }
            return new List<AccountModel>();
         
        }
    }
}
