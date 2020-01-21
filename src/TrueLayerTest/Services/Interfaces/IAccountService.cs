using System.Collections.Generic;
using System.Threading.Tasks;

namespace InterviewSiddhant_Gauchan.Services
{
    public interface IAccountService
    {
       Task<List<AccountModel>> GetAllBankAccounts();
    }
}
