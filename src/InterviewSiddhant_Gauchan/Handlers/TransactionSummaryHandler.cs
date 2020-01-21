using TrulayerApiTest.Helpers;
using TrulayerApiTest.Model;
using MediatR;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TrulayerApiTest.Handlers.Query;

namespace TrulayerApiTest.Handlers
{
    public interface ITransactionSummaryHandler : IRequestHandler<GetTransactionSummaryQuery, TransactionSummaryResponse>
    {        

    }

    public class TransactionSummaryHandler : ITransactionSummaryHandler
    {
        private readonly IStorage storage;

        public TransactionSummaryHandler(IStorage storage)
        {
            this.storage = storage;
        }
       

        public async Task<TransactionSummaryResponse> Handle(GetTransactionSummaryQuery request, CancellationToken cancellationToken)
        {
            
            var transactionSummaryViewModel = new List<TransactionSummaryViewModel>();
            var transactionSummaryRes = new TransactionSummaryResponse {  Response= transactionSummaryViewModel};
            var transactions = storage.Get<List<TransactionViewModel>>("transactionViewModel");

            if (transactions != null && transactions.Count > 0)
            {
                var transactionSummary = new List<TransactionSummaryModel>();

                foreach (var item in transactions)
                {
                    if (item.Transaction != null && item.Transaction.Count > 0)
                    {
                        transactionSummary = item.Transaction.GroupBy(p => new { p.TransactionCategory }).Select(grp => new TransactionSummaryModel
                        {
                            AverageAmount = grp.Average(p => p.Amount).ToString("0.##"),
                            MaxAmount = grp.Max(p => p.Amount).ToString("0.##"),
                            MinAmount = grp.Min(p => p.Amount).ToString("0.##"),
                            TransactionCategory = grp.Key.TransactionCategory
                        }).ToList();
                    }

                    transactionSummaryViewModel.Add(
                        new TransactionSummaryViewModel { Account = item.Account, TransactionSummary = transactionSummary });
                    transactionSummaryRes.Response = transactionSummaryViewModel;

                }

            }
            return transactionSummaryRes;
        }
    }
}
