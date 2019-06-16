using AuthExam.Controllers;
using AuthExam.Infrastructure.Interfaces;
using AuthExam.Models;
using Microsoft.AspNet.Identity;
using Moq;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace AuthExam.Tests
{
    public class UserResourcesControllerTest
    {
        private Mock<IImageRepository> mockImageRepository;
        private Mock<ControllerContext> mockControllerContext;
        private Mock<ApplicationUserManager> mockUserManager;

        private UserResourceViewModel testUserResourceViewModel = new UserResourceViewModel
        {
            Directory = "TestDirectory",
            FileNames = new List<string>
            {
                "FileName1",
                "FileName2",
                "FileName3",
            }
        };

        [SetUp]
        public void Setup()
        {
            mockImageRepository = new Mock<IImageRepository>();
            var mockUserStore = new Mock<IUserStore<ApplicationUser>>();
            mockUserManager = new Mock<ApplicationUserManager>(mockUserStore.Object);
            mockControllerContext = new Mock<ControllerContext>();
            var mockContext = new Mock<HttpContextBase>();
            var mockIdentity = new GenericIdentity("User");
            var principal = new GenericPrincipal(mockIdentity, null);
            var mockServer = new Mock<HttpServerUtilityBase>();

            mockImageRepository.Setup(mock => mock.getAll(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(testUserResourceViewModel);
            mockContext.Setup(mock => mock.User).Returns(principal);
            mockContext.Setup(mock => mock.Server).Returns(mockServer.Object);
            mockControllerContext.Setup(mock => mock.HttpContext).Returns(mockContext.Object);
            mockUserManager.Setup(mock => mock.FindByEmailAsync(It.IsAny<string>())).Returns(Task.FromResult(new ApplicationUser()));
        }

        [TestCase(TestName = "GetAll - Should return user's resources")]
        public async Task GetAllUserIdValidShouldReturnAllResources()
        {
            var controller = new UserResourcesController(mockImageRepository.Object, mockUserManager.Object);
            controller.ControllerContext = mockControllerContext.Object;
            var result = await controller.Index();
            Assert.AreEqual(typeof(UserResourceViewModel), (result as ViewResult)?.Model.GetType());
            var model = (result as ViewResult).Model as UserResourceViewModel;
            Assert.AreEqual(model.FileNames?.ToList().Count, testUserResourceViewModel.FileNames.Count());
            for (int i = 0; i < model.FileNames.Count(); i++)
            {
                Assert.AreEqual(testUserResourceViewModel.FileNames.ToList()[i], model.FileNames.ToList()[i]);
            }
        }
    }
}
