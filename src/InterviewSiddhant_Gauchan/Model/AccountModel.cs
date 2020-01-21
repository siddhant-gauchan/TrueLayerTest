using Newtonsoft.Json;

namespace TrulayerApiTest
{
    public class AccountModel
    {
        [JsonProperty("account_id")]
        public string AccountId { get; set; }

        [JsonProperty("display_name")]
        public string AccountName { get; set; }
        
        [JsonProperty("account_type")]
        public string AccountType { get; set; }
    }
}
