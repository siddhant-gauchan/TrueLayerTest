using InterviewSiddhant_Gauchan.Helpers;
using InterviewSiddhant_Gauchan.Model;
using InterviewSiddhant_Gauchan.Services;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace InterviewSiddhant_Gauchan.Handlers
{
    public interface ITransactionHandler
    {
        Task<List<TransactionViewModel>> Get();
    }

    public class TransactionHandler : ITransactionHandler
    {
        private readonly IAccountService accountService;
        private readonly ITransactionService transactionService;
        private readonly IStorage storage;

        public TransactionHandler(IAccountService accountService, ITransactionService transactionService, IStorage storage)
        {
            this.accountService = accountService;
            this.transactionService = transactionService;
            this.storage = storage;
        }

        public async Task<List<TransactionViewModel>> Get()
        {
            var transactionViewModel = new List<TransactionViewModel>();
            var bankAccounts = await accountService.GetAllBankAccounts();
            foreach (var item in bankAccounts)
            {
                var result = await transactionService.GetTransactionByAccountId(item.AccountId);
                transactionViewModel.Add( new TransactionViewModel
                {
                    Account =
                    new AccountModel
                    {
                        AccountId = item.AccountId,
                        AccountName = item.AccountName,
                        AccountType = item.AccountType
                    },
                    Transaction = result
                });

            }
            storage.Store(transactionViewModel, "transactionViewModel");
            return transactionViewModel;
        }
    }
}
