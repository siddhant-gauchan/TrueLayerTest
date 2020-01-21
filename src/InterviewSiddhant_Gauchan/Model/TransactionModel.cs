using Newtonsoft.Json;

namespace TrulayerApiTest.Model
{
    public class TransactionModel
    {
        [JsonProperty("transaction_type")]
        public string TransactionType { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("transaction_category")]
        public string TransactionCategory { get; set; }

        [JsonProperty("amount")]
        public double Amount { get; set; }
    }
}
