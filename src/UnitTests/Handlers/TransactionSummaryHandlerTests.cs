using InterviewSiddhant_Gauchan;
using InterviewSiddhant_Gauchan.Handlers;
using InterviewSiddhant_Gauchan.Helpers;
using InterviewSiddhant_Gauchan.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Collections.Generic;

namespace UnitTests.Handlers
{
    [TestClass]
    public class TransactionSummaryHandlerTests
    {
        [TestMethod]
        public void ShouldReturnListofTransactionSummaryViewModelWithDataWhenStorageHasData()
        {
                        
            var mockStorage = new Mock<IStorage>();
            var account = new AccountModel
            {
                AccountId = "testAccountId",
                AccountName = "TestAccount",
                AccountType = "TestAccountType"
            };
            var accounts = new List<AccountModel>{account};
            var transaction =
                new TransactionModel
                {
                    Amount = 1,
                    Description = "testDesc",
                    TransactionCategory = "testCategory",
                    TransactionType = "testTransaction"
                };

            var transactions = new List<TransactionModel> {transaction};
            var transactionViewModel = new List<TransactionViewModel> 
            { 
                new TransactionViewModel
                {
                    Account=account, 
                    Transaction= transactions
                } 
            };
            
            mockStorage.Setup(x => x.Get<List<TransactionViewModel>>("transactionViewModel")).Returns(transactionViewModel);
            var handler = new TransactionSummaryHandler(mockStorage.Object);

            var response = handler.Get();
            Assert.IsTrue(response.Count>0);
            Assert.IsInstanceOfType(response, typeof(List<TransactionSummaryViewModel>));
        }
        [TestMethod]
        public void ShouldReturnListofTransactionSummaryViewModelGroupedByCategoryWithMaxMinandAverageAmountWhenStorageHasData()
        {

            var mockStorage = new Mock<IStorage>();
            var account = new AccountModel
            {
                AccountId = "testAccountId",
                AccountName = "TestAccount",
                AccountType = "TestAccountType"
            };
            var accounts = new List<AccountModel> { account };
            var transaction1 =
                new TransactionModel
                {
                    Amount = 1,
                    Description = "testDesc",
                    TransactionCategory = "testCategory",
                    TransactionType = "testTransaction"
                };
            var transaction2 =
                new TransactionModel
                {
                    Amount = 10,
                    Description = "testDesc",
                    TransactionCategory = "testCategory",
                    TransactionType = "testTransaction"
                };
            var transaction3 =
                new TransactionModel
                {
                    Amount = 20,
                    Description = "testDesc",
                    TransactionCategory = "testCategory",
                    TransactionType = "testTransaction"
                };
            var transactions = new List<TransactionModel> { transaction1, transaction2, transaction3 };
            var transactionViewModel = new List<TransactionViewModel>
            {
                new TransactionViewModel
                {
                    Account=account,
                    Transaction= transactions
                }
            };

            mockStorage.Setup(x => x.Get<List<TransactionViewModel>>("transactionViewModel")).Returns(transactionViewModel);
            var handler = new TransactionSummaryHandler(mockStorage.Object);

            var response = handler.Get();
            Assert.IsTrue(response.Count > 0);
            Assert.IsTrue(response[0].TransactionSummary[0].AverageAmount == "10.33");
            Assert.IsTrue(response[0].TransactionSummary[0].MinAmount == "1");
            Assert.IsTrue(response[0].TransactionSummary[0].MaxAmount == "20");
            Assert.IsTrue(response[0].TransactionSummary[0].TransactionCategory == "testCategory");

            Assert.IsInstanceOfType(response, typeof(List<TransactionSummaryViewModel>));
        }
        

        [TestMethod]
        public void ShouldReturnTransactionSummaryViewModelWithOutDataWhenStorageHasNoData()
        {

            var mockStorage = new Mock<IStorage>();
            
            mockStorage.Setup(x => x.Get<List<TransactionViewModel>>("transactionViewModel")).Returns(new List<TransactionViewModel>());
            var handler = new TransactionSummaryHandler(mockStorage.Object);

            var response = handler.Get();
            Assert.IsTrue(response.Count == 0);
            Assert.IsInstanceOfType(response, typeof(List<TransactionSummaryViewModel>));
        }


    }
}
