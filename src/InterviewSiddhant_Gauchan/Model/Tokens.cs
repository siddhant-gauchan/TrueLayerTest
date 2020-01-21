using System;

namespace TrulayerApiTest
{
    public class TokenDetails
    {
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
        public DateTime ExpiryDate { get; set; }

    }
}