using BenchmarkDotNet.Attributes;
using Microsoft.AspNetCore.Mvc;
using Moq;
using RunGroopWebApp.Controllers;
using RunGroopWebApp.Interfaces;
using RunGroopWebApp.Models;
using RunGroopWebApp.ViewModels;

namespace RunGroopWebApp.Benchmarks
{
    [MemoryDiagnoser]
    public class ClubControllerBenchmark
    {
        private readonly ClubController _controller;

        public ClubControllerBenchmark()
        {
            var mockClubRepository = new Mock<IClubRepository>();
            var mockPhotoService = new Mock<IPhotoService>();
            var mockHttpContextAccessor = new Mock<IHttpContextAccessor>();

            // Setup mock behavior for AddPhotoAsync to simulate a successful photo upload
            mockPhotoService.Setup(p => p.AddPhotoAsync(It.IsAny<IFormFile>()))
                .ReturnsAsync(new CloudinaryDotNet.Actions.ImageUploadResult { Url = new Uri("http://example.com/photo.jpg") });

            // Initialize the controller with mocks
            _controller = new ClubController(mockClubRepository.Object, mockPhotoService.Object, mockHttpContextAccessor.Object);
        }

        [Benchmark]
        public async Task<IActionResult> CreateBenchmark()
        {
            var clubVM = new CreateClubViewModel
            {
                Title = "Test Club",
                Description = "Test Description",
                Image = null, // You can mock or set a test image here
                AppUserId = "TestUserId",
                Address = new Address
                {
                    Street = "Test Street",
                    City = "Test City",
                    State = "Test State"
                }
            };

            return await _controller.Create(clubVM);
        }
    }
}
