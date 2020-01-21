using InterviewSiddhant_Gauchan;
using MediatR;

namespace TrulayerApiTest.Handlers.Query
{
    public class GetTokenQuery:IRequest<TokenRespose>
    {
        public string AccessCode { get; set; }
    }
    
    public class TokenRespose
    {
        public TokenDetails Response { get; set; }
    }
}
