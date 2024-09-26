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

    public class RaceControllerBenchmark
    {
        private readonly RaceController _controller;
        public RaceControllerBenchmark()
        {
            // Mockovanje zavisnosti
            var mockRaceRepository = new Mock<IRaceRepository>();
            var mockPhotoService = new Mock<IPhotoService>();
            var mockHttpContextAccessor = new Mock<IHttpContextAccessor>();

            // Postavljanje lažnih podataka za GetByIdAsyncNoTracking
            mockRaceRepository.Setup(repo => repo.GetByIdAsyncNoTracking(It.IsAny<int>()))
                .ReturnsAsync(new Race
                {
                    Id = 1,
                    Title = "Test Race",
                    Description = "Test Description",
                    Image = "http://example.com/oldphoto.jpg",
                    AddressId = 1,
                    Address = new Address
                    {
                        Street = "Test Street",
                        City = "Test City",
                        State = "Test State"
                    }
                });

            // Postavljanje lažnih podataka za AddPhotoAsync
            mockPhotoService.Setup(service => service.AddPhotoAsync(It.IsAny<IFormFile>()))
                .ReturnsAsync(new CloudinaryDotNet.Actions.ImageUploadResult
                {
                    Url = new Uri("http://example.com/newphoto.jpg")
                });

            // Inicijalizacija kontrolera sa mock zavisnostima
            _controller = new RaceController(mockRaceRepository.Object, mockPhotoService.Object, mockHttpContextAccessor.Object);
        }

        [Benchmark]
        public async Task<IActionResult> EditBenchmark()
        {
            var raceVM = new EditRaceViewModel
            {
                Id = 1,
                Title = "Test Race",
                Description = "Test Description",
                AddressId = 1,
                Address = new Address
                {
                    Street = "Test Street",
                    City = "Test City",
                    State = "Test State"
                }
            };

            return await _controller.Edit(1, raceVM);
        }
    }
}
