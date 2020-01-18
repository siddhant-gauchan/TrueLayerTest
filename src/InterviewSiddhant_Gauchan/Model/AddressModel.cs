using Newtonsoft.Json;

namespace InterviewSiddhant_Gauchan.Model
{
    public class AddressModel
    {
        [JsonProperty("address")]
        public string Address { get; set; }

        [JsonProperty("city")]
        public string City { get; set; }

        [JsonProperty("zip")]
        public string Zip { get; set; }

        [JsonProperty("country")]
        public string Country { get; set; }
    }
}