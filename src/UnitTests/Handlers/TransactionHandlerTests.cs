using InterviewSiddhant_Gauchan;
using InterviewSiddhant_Gauchan.Handlers;
using InterviewSiddhant_Gauchan.Helpers;
using InterviewSiddhant_Gauchan.Model;
using InterviewSiddhant_Gauchan.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Collections.Generic;
using System.Threading.Tasks;

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

            var response = await handler.Get();
            Assert.IsTrue(response.Count>0);
            Assert.IsInstanceOfType(response, typeof(List<TransactionViewModel>));
        }

        [TestMethod]
        public async Task ShouldReturnTransactionViewModelWithOutDataWhenTrueLayerAccountsApiDoesNotReturnsData()
        {
            var mockAccountService = new Mock<IAccountService>();
            var mockTransactionService = new Mock<ITransactionService>();
            var mockStorage = new Mock<IStorage>();
            

            mockAccountService.Setup(x => x.GetAllBankAccounts()).ReturnsAsync(new List<AccountModel>());
           
            var handler = new TransactionHandler(mockAccountService.Object, mockTransactionService.Object, mockStorage.Object);

            var response = await handler.Get();
            Assert.IsTrue(response.Count==0);
            Assert.IsInstanceOfType(response, typeof(List<TransactionViewModel>));
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

            var response = await handler.Get();
            
            Assert.IsTrue(response.Count == 0);
            Assert.IsInstanceOfType(response, typeof(List<TransactionViewModel>));
        }
    }
}
