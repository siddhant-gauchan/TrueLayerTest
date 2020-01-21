using TrulayerApiTest.Model;
using MediatR;
using System.Collections.Generic;

namespace TrulayerApiTest.Handlers.Query
{
    public class GetTransactionQuery:IRequest<TransactionResponse>
    {

    }
    
    public class TransactionResponse { 

    public List<TransactionViewModel> Response { get; set; }

    }
}
