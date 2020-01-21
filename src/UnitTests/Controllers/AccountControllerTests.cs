using TrulayerApiTest;
using TrulayerApiTest.Controllers;
using TrulayerApiTest.Handlers;
using TrulayerApiTest.Helpers;
using TrulayerApiTest.Model;
using MediatR;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using TrulayerApiTest.Handlers.Query;

namespace UnitTests
{
    [TestClass]
    public class AccountControllerTests
    {
        

        [TestMethod]
        public void ShouldRedirectToAuthServerWhenTryToLogin()
        {
            //arrange
            var mockHandler = new Mock<ITokenHandler>();
            var mockIoptions = new Mock<IOptions<Config>>();
            var mockStorage = new Mock<IStorage>();
            var mockMediator = new Mock<IMediator>();
            mockIoptions.Setup(x => x.Value).Returns(new Config { OAuthServerUrl = "http://auth.test.com", ClientId = "testclient", ClientSecret = "testSecret" });

            var controller = new AccountController(mockHandler.Object, mockIoptions.Object,mockStorage.Object,mockMediator.Object);
            //act
            var response = controller.Login() as RedirectResult;

            //assert
            Assert.AreEqual("http://auth.test.com", response.Url.Split("/?")[0]);

        }

        [TestMethod]
        public void ShouldReturnInternalServerErrorWhenExceptionOccursWhenTryToLogin()
        {
            //arrange
            var mockHandler = new Mock<ITokenHandler>();
            var mockIoptions = new Mock<IOptions<Config>>();
            var mockStorage = new Mock<IStorage>();
            mockIoptions.Setup(x => x.Value).Throws(new System.Exception());
            var mockMediator = new Mock<IMediator>();

            var controller = new AccountController(mockHandler.Object, mockIoptions.Object, mockStorage.Object,mockMediator.Object);
            //act
            var response = controller.Login() as ObjectResult;

            //assert
            Assert.AreEqual(StatusCodes.Status500InternalServerError, response.StatusCode);


        }
        [TestMethod]
        [DataRow("")]
        [DataRow(null)]
        public async Task  ShouldReturnErrorWhenCallbackFromAuthServerDoesnotReturnCode(string testCallbackCode)
        {
            //arrange
            var mockHandler = new Mock<ITokenHandler>();
            var mockIoptions = new Mock<IOptions<Config>>();
            var mockStorage = new Mock<IStorage>();
            var mockMediator = new Mock<IMediator>();
            var controller = new AccountController(mockHandler.Object, mockIoptions.Object, mockStorage.Object,mockMediator.Object);
            //act
            var response = await controller.Callback(testCallbackCode) as ObjectResult;
            
            //assert
            Assert.AreEqual("error", response.Value);
        }

        [TestMethod]
        [DataRow("testCode1")]
        [DataRow("testCode2")]
        public async Task ShouldReturnAccessTokenWhenCallbackFromAuthServerReturnsCode(string testCallbackCode)
        {
            //arrange
            var mockHandler = new Mock<ITokenHandler>();
            var mockIoptions = new Mock<IOptions<Config>>();
            var mockStorage = new Mock<IStorage>();
            var mockMediator = new Mock<IMediator>();
            mockStorage.Setup(x => x.Get<List<UserModel>>("userInfo")).Returns(new List<UserModel>{new UserModel
            {
                Addresses = new List<AddressModel>
                { new AddressModel
                    {
                        Address = "testAddress",
                        City = "testcity"
                    }
                },
                Emails = new List<string> { "test1@test.com" },
                FullName= "test test",
                Phones= new List<string> { "123456789" }

            }});
            var authServiceMock = new Mock<IAuthenticationService>();
            authServiceMock
                .Setup(_ => _.SignInAsync(It.IsAny<HttpContext>(), It.IsAny<string>(), It.IsAny<ClaimsPrincipal>(), It.IsAny<AuthenticationProperties>()))
                .Returns(Task.FromResult((object)null));

            var serviceProviderMock = new Mock<IServiceProvider>();
            serviceProviderMock
                .Setup(_ => _.GetService(typeof(IAuthenticationService)))
                .Returns(authServiceMock.Object);
            mockMediator.Setup(x => x.Send(It.IsAny<GetTokenQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(new TokenRespose { Response= new TokenDetails { AccessToken = "testAccessToken", RefreshToken = "testRefreshToken", ExpiryDate = DateTime.Now.AddMinutes(30) } });
            
            
            var controller = new AccountController(mockHandler.Object, mockIoptions.Object, mockStorage.Object, mockMediator.Object)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = new DefaultHttpContext
                    {
                        
                        RequestServices = serviceProviderMock.Object
                    }
                }
            };
       
            //act
            var response = await controller.Callback(testCallbackCode) as ObjectResult;
            var result = response.Value as TokenDetails;
            //assert
            Assert.IsTrue(!string.IsNullOrEmpty(result.AccessToken));
            Assert.AreEqual(StatusCodes.Status200OK, response.StatusCode);


        }


        [TestMethod]
        [DataRow("testCode1")]
        [DataRow("testCode2")]
        public async Task ShouldReturnInternalServerErrorWhenCallbackFromAuthServerReturnsCodeAndThrowsExceptionWhenGettingAccessToken(string testCallbackCode)
        {
            //arrange
            var mockHandler = new Mock<ITokenHandler>();
            var mockIoptions = new Mock<IOptions<Config>>();
            var mockStorage = new Mock<IStorage>();
            var mockMediator = new Mock<IMediator>();         

            mockMediator.Setup(x => x.Send(It.IsAny<GetTokenQuery>(), It.IsAny<CancellationToken>())).ThrowsAsync(new Exception());
            var controller = new AccountController(mockHandler.Object, mockIoptions.Object, mockStorage.Object,mockMediator.Object);
            //act
            var response = await controller.Callback(testCallbackCode) as ObjectResult;
            var result = response.Value as TokenDetails;
            //assert
           
            Assert.AreEqual(StatusCodes.Status500InternalServerError, response.StatusCode);


        }
    }
}
