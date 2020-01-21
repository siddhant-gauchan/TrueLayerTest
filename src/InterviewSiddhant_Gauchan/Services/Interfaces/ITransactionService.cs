using TrulayerApiTest.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace TrulayerApiTest.Services
{
    public interface ITransactionService
    {
        Task<List<TransactionModel>> GetTransactionByAccountId(string accountId);
    }
}
