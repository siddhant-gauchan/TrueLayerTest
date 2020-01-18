using InterviewSiddhant_Gauchan.Helpers;
using InterviewSiddhant_Gauchan.Model;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace InterviewSiddhant_Gauchan.Services
{
    public class UserService : IUserService
    {
        private readonly IHttpHelpers httpHelpers;
        private readonly IOptions<Config> config;
        private readonly IStorage tokenStorage;

        public UserService(IHttpHelpers httpHelpers, IOptions<Config> config, IStorage tokenStorage)
        {
            this.httpHelpers = httpHelpers;
            this.config = config;
            this.tokenStorage = tokenStorage;
        }
                
        public async Task<List<UserModel>> GetInfo()
        {
            var token= tokenStorage.Get<TokenDetails>(config.Value.TokenKey);

            var client = httpHelpers.GetClient(config.Value.ResourceApi);
            var authValue = new AuthenticationHeaderValue("Bearer", token.AccessToken);
           
            client.DefaultRequestHeaders.Authorization = authValue;

            var response = await client.GetAsync("/data/v1/info");
            if(response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadAsStringAsync();
                var jObject = JObject.Parse(result);

               return jObject["results"].ToObject<List<UserModel>>();
               
            }
            return new List<UserModel>();
         
        }
    }
}
