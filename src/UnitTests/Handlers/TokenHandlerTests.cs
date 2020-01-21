using InterviewSiddhant_Gauchan.Handlers;
using InterviewSiddhant_Gauchan.Helpers;
using InterviewSiddhant_Gauchan.Services;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Moq.Protected;
using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using TrulayerApiTest.Handlers.Query;

namespace UnitTests.Handlers
{
    [TestClass]
    public class TokenHandlerTests
    {
        [TestMethod]
        [DataRow("testCode1")]
        [DataRow("testCode1")]
        public async Task ShouldReturnAccessTockenWhenAccessCodeIsSupplied(string accessCode)
        {
            //arrange 
            var mockHttpHelpers = new Mock<IHttpHelpers>();
            var mockIoptions = new Mock<IOptions<Config>>();
            var mockStorage = new Mock<IStorage>();
            var mockUserService = new Mock<IUserService>();
            var config = new Config { OAuthServerUrl = "http://auth.test.com", ClientId = "testclient", ClientSecret = "testSecret" };
            mockIoptions.Setup(x => x.Value).Returns(config);
            var mockHttpCient = new Mock<IHttpHelpers>();
            
            var handlerMock = new Mock<HttpMessageHandler>(MockBehavior.Strict);
            handlerMock
               .Protected()
               .Setup<Task<HttpResponseMessage>>(
                  "SendAsync",
                  ItExpr.IsAny<HttpRequestMessage>(),
                  ItExpr.IsAny<CancellationToken>()
               )
               .ReturnsAsync(new HttpResponseMessage()
               {
                   StatusCode = HttpStatusCode.OK,
                   Content = new StringContent("{\"access_token\":\"eyJhbGciOiJSUzI1NiIsImtpZCI6IjE0NTk4OUIwNTdDOUMzMzg0MDc4MDBBOEJBNkNCOUZFQjMzRTk1MTAiLCJ0eXAiOiJKV1QiLCJ4NXQiOiJGRm1Kc0ZmSnd6aEFlQUNvdW15NV9yTS1sUkEifQ.eyJuYmYiOjE1NzkyODgwNDMsImV4cCI6MTU3OTI5MTY0MywiaXNzIjoiaHR0cHM6Ly9hdXRoLnRydWVsYXllci5jb20iLCJhdWQiOlsiaHR0cHM6Ly9hdXRoLnRydWVsYXllci5jb20vcmVzb3VyY2VzIiwiaW5mb19hcGkiLCJhY2NvdW50c19hcGkiLCJ0cmFuc2FjdGlvbnNfYXBpIl0sImNsaWVudF9pZCI6IjEyMzQtNmU4ZTFlIiwic3ViIjoiL1JUVll5ZmhqMU55QVBzNWVjVEZJVXZqUGkzc0ZSV3NaL3BYdTZybU1xQT0iLCJhdXRoX3RpbWUiOjE1NzkyODgwMzgsImlkcCI6ImxvY2FsIiwiY29ubmVjdG9yX2lkIjoibW9jayIsImNyZWRlbnRpYWxzX2tleSI6IjYxODQzOWQ4ODFhOWE3OGQ5MmIzNDRhYzdhMzQ3NmQ2NzVmMzY4MWE5NGUwZWRjYWNlNDc4NGNlNWEwMGVjYzAiLCJwcml2YWN5X3BvbGljeSI6IkZlYjIwMTkiLCJjb25zZW50X2lkIjoiMmI5NzQ4OTYtNDcyZC00ZmRlLTg4YzgtZWVjOTMzYTU5NGEzIiwic2NvcGUiOlsiaW5mbyIsImFjY291bnRzIiwidHJhbnNhY3Rpb25zIiwib2ZmbGluZV9hY2Nlc3MiXSwiYW1yIjpbInB3ZCJdfQ.3vU89qEsmhUD5ukaA6blCOsOBC_kYgqfAbQdok9Tv0mMD27pybeVKpiFFShe7gKnuzE0xrs_bycteTgqyOq4P46Di0d6mNKAKbn8_c8dfBWZi_uDC3eJ8Nl1krVioKo_RmfBDodxjGPCu44_YB4DciF5EDV9xa4bnawEaARao4Dqz66nSRbFOOT--heFQTgXFfsiYwSPAMUTj8rZZPHu3h8IxqJ4GC2Jge1bowjWxsVUX6VHrnHNAIRgVxvcQE6uiDqwX6R5NI_zjGGiMX7QkdvsEFrv-8TImA2udKztpwfXFGli0ciXIhRFMUaCBSWsqoxrZYTpJ8vWSnxR1OjAtQ\","+
                                                "\"expires_in\":3600,"+
                                                "\"token_type\":\"Bearer\","+
                                                "\"refresh_token\":\"0b04997d6173789aa1e709ceae2ef1ce2bd830133f49c4ec188bd29443395e00\","+
                                                "\"scope\":\"transactions accounts info offline_access\"}")
               })
               .Verifiable();

            var httpClient = new HttpClient(handlerMock.Object)
            {
                BaseAddress = new Uri(mockIoptions.Object.Value.OAuthServerUrl),
            };

            mockHttpHelpers.Setup(x => x.GetClient(mockIoptions.Object.Value.OAuthServerUrl)).Returns(httpClient);

            var handler = new TokenHandler(mockHttpHelpers.Object, mockIoptions.Object, mockStorage.Object, mockUserService.Object);
            
            //act
             var result = await handler.Handle(new GetTokenQuery { AccessCode = accessCode }, It.IsAny<CancellationToken>());
            //assert
            Assert.IsTrue(!string.IsNullOrEmpty(result.Response.AccessToken));
            
        }
        
        [TestMethod]
        [DataRow("")]
        [DataRow(null)]
        public async Task ShouldNotGenerateAccessTokenWhenAccessCodeIsNotSupplied(string accessCode)
        {
            //arrange 
            var mockHttpHelpers = new Mock<IHttpHelpers>();
            var mockIoptions = new Mock<IOptions<Config>>();
            var mockStorage = new Mock<IStorage>();
            var mockUserService = new Mock<IUserService>();
            var config = new Config { OAuthServerUrl = "http://auth.test.com", ClientId = "testclient", ClientSecret = "testSecret" };
            mockIoptions.Setup(x => x.Value).Returns(config);
            var mockHttpCient = new Mock<IHttpHelpers>();

            var handlerMock = new Mock<HttpMessageHandler>(MockBehavior.Strict);
            handlerMock
               .Protected()
               .Setup<Task<HttpResponseMessage>>(
                  "SendAsync",
                  ItExpr.IsAny<HttpRequestMessage>(),
                  ItExpr.IsAny<CancellationToken>()
               )
               .ReturnsAsync(new HttpResponseMessage()
               {
                   StatusCode = HttpStatusCode.BadRequest
                   
               })
               .Verifiable();

            var httpClient = new HttpClient(handlerMock.Object)
            {
                BaseAddress = new Uri(mockIoptions.Object.Value.OAuthServerUrl),
            };

            mockHttpHelpers.Setup(x => x.GetClient(mockIoptions.Object.Value.OAuthServerUrl)).Returns(httpClient);

            var handler = new TokenHandler(mockHttpHelpers.Object, mockIoptions.Object, mockStorage.Object, mockUserService.Object);

            //act
            var result = await handler.Handle(new GetTokenQuery { AccessCode = accessCode }, It.IsAny<CancellationToken>());
            //assert
            Assert.IsTrue(result.Response==null);

        }

    }
}
