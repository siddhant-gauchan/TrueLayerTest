namespace TrulayerApiTest.Helpers
{
    public class Config
    {
        public string OAuthServerUrl { get; set; }
        public string RedirectUri { get; set; }
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public string ResourceApi { get; set; }
        public string TokenKey => "token";
    }
}