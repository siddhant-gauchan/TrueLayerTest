using InterviewSiddhant_Gauchan.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace InterviewSiddhant_Gauchan.Services
{
    public interface ITransactionService
    {
        Task<List<TransactionModel>> GetTransactionByAccountId(string accountId);
    }
}
