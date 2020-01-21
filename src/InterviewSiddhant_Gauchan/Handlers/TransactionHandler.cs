using TrulayerApiTest.Helpers;
using TrulayerApiTest.Model;
using TrulayerApiTest.Services;
using MediatR;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using TrulayerApiTest.Handlers.Query;

namespace TrulayerApiTest.Handlers
{
    public interface ITransactionHandler: IRequestHandler<GetTransactionQuery,TransactionResponse>
    {
        
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
               

        public  async Task<TransactionResponse> Handle(GetTransactionQuery request, CancellationToken cancellationToken)
        {
            var transactionViewModel = new List<TransactionViewModel>();
            var bankAccounts = await accountService.GetAllBankAccounts();
            foreach (var item in bankAccounts)
            {
                var result = await transactionService.GetTransactionByAccountId(item.AccountId);
                transactionViewModel.Add(new TransactionViewModel
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
            return new TransactionResponse { Response= transactionViewModel};
        }
    }
}
