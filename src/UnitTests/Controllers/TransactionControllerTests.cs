using InterviewSiddhant_Gauchan;
using InterviewSiddhant_Gauchan.Controllers;
using InterviewSiddhant_Gauchan.Handlers;
using InterviewSiddhant_Gauchan.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace UnitTests
{
    [TestClass]
    public class TransactionControllerTests
    {
        [TestMethod]
        public async Task ShouldReturnTransactionViewModelWithDataForTransationEndpoint()
        {
            //arrange
            var mockTransactionHandler = new Mock<ITransactionHandler>();
            var mockTransactionSummaryHandler = new Mock<ITransactionSummaryHandler>();
            mockTransactionHandler.Setup(x => x.Get()).ReturnsAsync(
                new List<TransactionViewModel>
                            {
                              new TransactionViewModel
                                {
                                  Account = new AccountModel
                                    { AccountId = "testAccountId",
                                     AccountName = "testAccountName",
                                        AccountType = "testAccountType"
                                    },
                                  Transaction = new List<TransactionModel>
                                {
                                      new TransactionModel
                                       { Amount = 1,
                                         Description = "testDescription",
                                        TransactionCategory = "testCategory",
                                        TransactionType = "testTransactionType" }
                                        }
                                }
                });

            var controller = new TransactionController(mockTransactionHandler.Object, mockTransactionSummaryHandler.Object);
            //act
            var response = await controller.Get() as ObjectResult;
            var result = response.Value as List<TransactionViewModel>;
             //assert
            Assert.IsInstanceOfType(response.Value, typeof(List<TransactionViewModel>));
            Assert.IsTrue(result.Count>0);
            Assert.IsTrue(response.StatusCode == 200);

        }

        [TestMethod]
        public async Task ShouldReturnInternalServerErrorWhenExceptionOccursForTransactionEndPoint()
        {
            //arrange
            var mockTransactionHandler = new Mock<ITransactionHandler>();
            var mockTransactionSummaryHandler = new Mock<ITransactionSummaryHandler>();
            mockTransactionHandler.Setup(x => x.Get()).Throws(new System.Exception());

            var controller = new TransactionController(mockTransactionHandler.Object, mockTransactionSummaryHandler.Object);
            //act
            var response = await controller.Get() as ObjectResult;

            //assert
            Assert.AreEqual(StatusCodes.Status500InternalServerError, response.StatusCode);
            
            
        }

        [TestMethod]        
        public void ShouldReturnTransactionSummaryViewModelForTransactionSummaryEndPoint()
        {
            //arrange
            var mockTransactionHandler = new Mock<ITransactionHandler>();
            var mockTransactionSummaryHandler = new Mock<ITransactionSummaryHandler>();
            mockTransactionSummaryHandler.Setup(x => x.Get()).Returns(new List<TransactionSummaryViewModel> { });
            var controller = new TransactionController(mockTransactionHandler.Object, mockTransactionSummaryHandler.Object);

            //act
            var response = controller.GetSummary() as ObjectResult;

            //assert
            Assert.IsInstanceOfType(response.Value, typeof(List<TransactionSummaryViewModel>));
            Assert.IsTrue(response.StatusCode == 200);
            
        }

        [TestMethod]
        public void ShouldReturnInternalServerErrorWhenExceptionOccursForTransactionSummaryEndPoint()
        {
            //arrange
            var mockTransactionHandler = new Mock<ITransactionHandler>();
            var mockTransactionSummaryHandler = new Mock<ITransactionSummaryHandler>();
            mockTransactionSummaryHandler.Setup(x => x.Get()).Throws(new System.Exception());
            var controller = new TransactionController(mockTransactionHandler.Object, mockTransactionSummaryHandler.Object);

            //act
            var response = controller.GetSummary() as ObjectResult;

            //assert
            Assert.AreEqual(StatusCodes.Status500InternalServerError, response.StatusCode);
            

        }
        
    }
}
