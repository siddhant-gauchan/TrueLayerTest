using TrulayerApiTest;
using TrulayerApiTest.Controllers;
using TrulayerApiTest.Handlers;
using TrulayerApiTest.Model;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using TrulayerApiTest.Handlers.Query;

namespace UnitTests
{
    [TestClass]
    public class TransactionControllerTests
    {
        [TestMethod]
        public async Task ShouldReturnTransactionViewModelWithDataForTransationEndpoint()
        {
            //arrange
            var mockMediator = new Mock<IMediator>();
            var res = new List<TransactionViewModel>
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
                };
            mockMediator.Setup(x => x.Send(It.IsAny<GetTransactionQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(new TransactionResponse { Response = res });

            var controller = new TransactionController(mockMediator.Object);
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
            var mockMediator = new Mock<IMediator>();
            mockMediator.Setup(x => x.Send(It.IsAny<GetTransactionSummaryQuery>(), It.IsAny<CancellationToken>())).ThrowsAsync(new System.Exception());

            var controller = new TransactionController(mockMediator.Object);
            //act
            var response = await controller.Get() as ObjectResult;

            //assert
            Assert.AreEqual(StatusCodes.Status500InternalServerError, response.StatusCode);
            
            
        }

        [TestMethod]        
        public async Task  ShouldReturnTransactionSummaryViewModelForTransactionSummaryEndPoint()
        {
            //arrange
            var mockMediator = new Mock<IMediator>();
            mockMediator.Setup(x => x.Send(It.IsAny<GetTransactionSummaryQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(new TransactionSummaryResponse { Response= new List<TransactionSummaryViewModel> { } });
            var controller = new TransactionController(mockMediator.Object);

            //act
            var response = await controller.GetSummary() as ObjectResult;

            //assert
            Assert.IsInstanceOfType(response.Value, typeof(List<TransactionSummaryViewModel>));
            Assert.IsTrue(response.StatusCode == 200);
            
        }

        [TestMethod]
        public async Task ShouldReturnInternalServerErrorWhenExceptionOccursForTransactionSummaryEndPoint()
        {
            //arrange
           
            var mockMediator = new Mock<IMediator>();
            mockMediator.Setup(x => x.Send(It.IsAny<GetTransactionSummaryQuery>(),It.IsAny<CancellationToken>())).ThrowsAsync(new System.Exception());
            var controller = new TransactionController(mockMediator.Object);

            //act
            var response = await controller.GetSummary() as ObjectResult;

            //assert
            Assert.AreEqual(StatusCodes.Status500InternalServerError, response.StatusCode);
            

        }
        
    }
}
