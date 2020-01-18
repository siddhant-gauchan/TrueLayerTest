using InterviewSiddhant_Gauchan.Helpers;
using InterviewSiddhant_Gauchan.Model;
using System.Collections.Generic;
using System.Linq;

namespace InterviewSiddhant_Gauchan.Handlers
{
    public interface ITransactionSummaryHandler
    {
        List<TransactionSummaryViewModel> Get();

    }

    public class TransactionSummaryHandler : ITransactionSummaryHandler
    {
        private readonly IStorage storage;

        public TransactionSummaryHandler(IStorage storage)
        {
            this.storage = storage;
        }
        public List<TransactionSummaryViewModel> Get()
        {
            var transactionSummaryViewModel = new List<TransactionSummaryViewModel>();
            var transactions = storage.Get<List<TransactionViewModel>>("transactionViewModel");
            
            if (transactions != null && transactions.Count > 0)
            {
                var transactionSummary = new List<TransactionSummaryModel>();

                foreach (var item in transactions)
                {
                    if (item.Transaction != null && item.Transaction.Count > 0)
                    {
                        transactionSummary= item.Transaction.GroupBy(p => new { p.TransactionCategory }).Select(grp => new TransactionSummaryModel
                        {
                            AverageAmount = grp.Average(p => p.Amount).ToString("0.##"),
                            MaxAmount = grp.Max(p => p.Amount).ToString("0.##"),
                            MinAmount = grp.Min(p => p.Amount).ToString("0.##"),
                            TransactionCategory = grp.Key.TransactionCategory
                        }).ToList();                      
                    }

                    transactionSummaryViewModel.Add(
                        new TransactionSummaryViewModel { Account = item.Account, TransactionSummary = transactionSummary });
                }
                return  transactionSummaryViewModel;
            }
            return new List<TransactionSummaryViewModel>();
        }
    }
}
