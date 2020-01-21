using InterviewSiddhant_Gauchan.Model;
using MediatR;
using System.Collections.Generic;

namespace TrulayerApiTest.Handlers.Query
{

    public class GetTransactionSummaryQuery : IRequest<TransactionSummaryResponse>
    {

    }

    public class TransactionSummaryResponse
    {

        public List<TransactionSummaryViewModel> Response { get; set; }

    }
}
