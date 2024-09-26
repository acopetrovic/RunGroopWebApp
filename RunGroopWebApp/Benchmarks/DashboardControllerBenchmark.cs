using BenchmarkDotNet.Attributes;
using Microsoft.AspNetCore.Mvc;
using Moq;
using RunGroopWebApp.Controllers;
using RunGroopWebApp.Interfaces;
using RunGroopWebApp.Models;

namespace RunGroopWebApp.Benchmarks
{
    [MemoryDiagnoser]
    public class DashboardControllerBenchmark
    {
        private readonly DashboardController _controller;

        public DashboardControllerBenchmark()
        {
            var mockDashboardRepository = new Mock<IDashboardRepository>();
            var mockHttpContextAccessor = new Mock<IHttpContextAccessor>();
            var mockPhotoService = new Mock<IPhotoService>();

            // Setup mock behavior for GetAllUserClubs and GetAllUserRaces
            mockDashboardRepository.Setup(repo => repo.GetAllUserClubs())
                .ReturnsAsync(new List<Club>()); // Return an empty list or mock data

            mockDashboardRepository.Setup(repo => repo.GeyAllUserRaces())
                .ReturnsAsync(new List<Race>()); // Return an empty list or mock data

            _controller = new DashboardController(
                mockDashboardRepository.Object,
                mockHttpContextAccessor.Object,
                mockPhotoService.Object);
        }

        [Benchmark]
        public async Task<IActionResult> IndexBenchmark()
        {
            return await _controller.Index();
        }
    }
}
