using Newtonsoft.Json;
using System.Collections.Generic;

namespace InterviewSiddhant_Gauchan.Model
{
    public class UserModel
    {
        [JsonProperty("full_name")]
        public string FullName { get; set; }

        [JsonProperty("addresses")]
        public List<AddressModel> Addresses { get; set; }

        [JsonProperty("emails")]
        public List<string> Emails { get; set; }

        [JsonProperty("phones")]
        public List<string> Phones { get; set; }
    }
}
