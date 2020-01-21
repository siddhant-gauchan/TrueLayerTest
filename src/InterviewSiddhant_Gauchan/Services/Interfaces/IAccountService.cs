using System.Collections.Generic;
using System.Threading.Tasks;

namespace TrulayerApiTest.Services
{
    public interface IAccountService
    {
       Task<List<AccountModel>> GetAllBankAccounts();
    }
}
