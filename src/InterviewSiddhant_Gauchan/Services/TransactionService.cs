using TrulayerApiTest.Helpers;
using TrulayerApiTest.Model;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace TrulayerApiTest.Services
{
    public class TransactionService : ITransactionService
    {
        private readonly IHttpHelpers httpHelpers;
        private readonly IOptions<Config> config;
        private readonly IStorage tokenStorage;

        public TransactionService(IHttpHelpers httpHelpers, IOptions<Config> config, IStorage tokenStorage)
        {
            this.httpHelpers = httpHelpers;
            this.config = config;
            this.tokenStorage = tokenStorage;
        }
        public async Task<List<TransactionModel>> GetTransactionByAccountId(string accountId)
        {
            var token = tokenStorage.Get<TokenDetails>(config.Value.TokenKey);

            var client = httpHelpers.GetClient(config.Value.ResourceApi);            
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token.AccessToken);

            var path = $"/data/v1/accounts/{accountId}/transactions?from=''&to=''";

            var response = await client.GetAsync(path);
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadAsStringAsync();
                var jObject = JObject.Parse(result);

                return jObject["results"].ToObject<List<TransactionModel>>();

            }
            return new List<TransactionModel>();
        }
    }
}

