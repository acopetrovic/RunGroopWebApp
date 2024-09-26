using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using RunGroopWebApp.Benchmarks;
using RunGroopWebApp.Controllers;
using RunGroopWebApp.Data;
using RunGroopWebApp.Models;
using RunGroopWebApp.ViewModels;
using Moq;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Toolchains.InProcess.NoEmit;
using BenchmarkDotNet.Toolchains;
using BenchmarkDotNet.Configs;
using Microsoft.EntityFrameworkCore;
namespace RunGroopWebApp.Benchmarks
{
    [MemoryDiagnoser]
    public class AccountControllerBenchmark
    {
        private readonly AccountController _controller;

        public AccountControllerBenchmark()
        {
            var mockUserManager = new Mock<UserManager<AppUser>>(
            new Mock<IUserStore<AppUser>>().Object,
            null, null, null, null, null, null, null, null);

            var mockSignInManager = new Mock<SignInManager<AppUser>>(
                mockUserManager.Object,
                new Mock<IHttpContextAccessor>().Object,
                new Mock<IUserClaimsPrincipalFactory<AppUser>>().Object,
                null, null, null, null);

            // Dodaj default setup za UserManager mock
            mockUserManager.Setup(um => um.FindByEmailAsync(It.IsAny<string>()))
                           .ReturnsAsync(new AppUser { Email = "teddysmithdeveloper@gmail.com" });

            mockUserManager.Setup(um => um.CheckPasswordAsync(It.IsAny<AppUser>(), It.IsAny<string>()))
                           .ReturnsAsync(true);

            mockSignInManager.Setup(sm => sm.PasswordSignInAsync(It.IsAny<AppUser>(), It.IsAny<string>(), false, false))
                             .ReturnsAsync(Microsoft.AspNetCore.Identity.SignInResult.Success);

            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;
            var context = new ApplicationDbContext(options);

            _controller = new AccountController(mockUserManager.Object, mockSignInManager.Object, context);
        }

        [Benchmark]
        public async Task LoginBenchmark()
        {
            var viewModel = new LoginViewModel
            {
                EmailAddress = "teddysmithdeveloper@gmail.com",
                Password = "Coding@1234?"
            };

            var result = await _controller.Login(viewModel);
        }
    }
}