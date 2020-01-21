using TrulayerApiTest;
using TrulayerApiTest.Handlers;
using TrulayerApiTest.Helpers;
using TrulayerApiTest.Model;
using TrulayerApiTest.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using TrulayerApiTest.Handlers.Query;

namespace UnitTests.Handlers
{
    [TestClass]
    public class TransactionHandlerTests
    {
        [TestMethod]
        public async Task ShouldReturnListofTransactionViewModelWithDataWhenTrueLayerApiReturnsData()
        {
            var mockAccountService = new Mock<IAccountService>();
            var mockTransactionService = new Mock<ITransactionService>();
            var mockStorage = new Mock<IStorage>();
            var accounts = new List<AccountModel>
            {
                new AccountModel
                {
                    AccountId="testAccountId",
                    AccountName="TestAccount",
                    AccountType= "TestAccountType"
                }
            };

            var transactions = new List<TransactionModel> {
                new TransactionModel 
                { 
                    Amount = 1, 
                    Description = "testDesc", 
                    TransactionCategory = "testCategory", 
                    TransactionType = "testTransaction" 
                } 
            };
            mockAccountService.Setup(x => x.GetAllBankAccounts()).ReturnsAsync(accounts);
            mockTransactionService.Setup(x => x.GetTransactionByAccountId(accounts[0].AccountId)).ReturnsAsync(transactions);

            var handler = new TransactionHandler(mockAccountService.Object, mockTransactionService.Object, mockStorage.Object);

            var result = await handler.Handle(It.IsAny<GetTransactionQuery>(),It.IsAny<CancellationToken>());
            Assert.IsTrue(result.Response.Count>0);
            Assert.IsInstanceOfType(result.Response, typeof(List<TransactionViewModel>));
        }

        [TestMethod]
        public async Task ShouldReturnTransactionViewModelWithOutDataWhenTrueLayerAccountsApiDoesNotReturnsData()
        {
            var mockAccountService = new Mock<IAccountService>();
            var mockTransactionService = new Mock<ITransactionService>();
            var mockStorage = new Mock<IStorage>();
            

            mockAccountService.Setup(x => x.GetAllBankAccounts()).ReturnsAsync(new List<AccountModel>());
           
            var handler = new TransactionHandler(mockAccountService.Object, mockTransactionService.Object, mockStorage.Object);

            var result = await handler.Handle(It.IsAny<GetTransactionQuery>(), It.IsAny<CancellationToken>());
            Assert.IsTrue(result.Response.Count==0);
            Assert.IsInstanceOfType(result.Response, typeof(List<TransactionViewModel>));
        }

        [TestMethod]
        public async Task ShouldReturnTransactionViewModelWithOutDataWhenTrueLayerTransactionApiEndpointDoesNotReturnsData()
        {
            var mockAccountService = new Mock<IAccountService>();
            var mockTransactionService = new Mock<ITransactionService>();
            var mockStorage = new Mock<IStorage>();
            var accounts = new List<AccountModel>
            {
                new AccountModel
                {
                    AccountId="testAccountId",
                    AccountName="TestAccount",
                    AccountType= "TestAccountType"
                }
            };

            mockAccountService.Setup(x => x.GetAllBankAccounts()).ReturnsAsync(new List<AccountModel>());
            mockTransactionService.Setup(x => x.GetTransactionByAccountId(accounts[0].AccountId)).ReturnsAsync(new List<TransactionModel>());
            
            var handler = new TransactionHandler(mockAccountService.Object, mockTransactionService.Object, mockStorage.Object);

            var result = await handler.Handle(It.IsAny<GetTransactionQuery>(), It.IsAny<CancellationToken>());

            Assert.IsTrue(result.Response.Count == 0);
            Assert.IsInstanceOfType(result.Response, typeof(List<TransactionViewModel>));
        }
    }
}
