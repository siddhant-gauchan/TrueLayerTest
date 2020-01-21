using System.Collections.Generic;

namespace TrulayerApiTest.Model
{
    public class TransactionViewModel
    {
        public AccountModel Account { get; set; }
        public List<TransactionModel> Transaction { get; set; }
        
    }
    
    public class TransactionSummaryViewModel
    {
        public AccountModel Account { get; set; }
        public List<TransactionSummaryModel> TransactionSummary { get; set; }
    }

    public class TransactionSummaryModel
    {
        public string TransactionCategory { get; set; }
        public string MaxAmount { get; set; }
        public string AverageAmount { get; set; }
        public string MinAmount { get; set; }
    }
}
