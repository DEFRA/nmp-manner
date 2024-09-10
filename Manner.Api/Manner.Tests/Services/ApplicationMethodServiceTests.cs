using AutoMapper;
using FluentAssertions;
using Manner.Application.DTOs;
using Manner.Application.Interfaces;
using Manner.Application.Services;
using Manner.Core.Entities;
using Manner.Core.Interfaces;
using Moq;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Manner.Tests.Services
{
    public class ApplicationMethodServiceTests
    {
        private readonly Mock<IApplicationMethodRepository> _mockRepository;
        private readonly Mock<IMapper> _mockMapper;
        private readonly IApplicationMethodService _service;

        public ApplicationMethodServiceTests()
        {
            // Mock the repository and the mapper
            _mockRepository = new Mock<IApplicationMethodRepository>();
            _mockMapper = new Mock<IMapper>();

            // Create the service with mocked dependencies
            _service = new ApplicationMethodService(_mockRepository.Object, _mockMapper.Object);
        }

        // Test FetchAllAsync()
        [Fact]
        public async Task FetchAllAsync_ShouldReturnAllMethods_WhenMethodsExist()
        {
            // Arrange
            var methods = GetSampleApplicationMethods();
            var mappedMethods = GetSampleApplicationMethodDtos();

            _mockRepository.Setup(repo => repo.FetchAllAsync())
                .ReturnsAsync(methods);
            _mockMapper.Setup(m => m.Map<IEnumerable<ApplicationMethodDto>>(methods))
                .Returns(mappedMethods);

            // Act
            var result = await _service.FetchAllAsync();

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(2); // Expect 2 methods
            result.Select(r => r.Name).Should().Contain(new[] { "Method 1", "Broadcast spreader" });
        }

        // Test FetchByIdAsync()
        [Fact]
        public async Task FetchByIdAsync_ShouldReturnMethod_WhenMethodExists()
        {
            // Arrange
            var method = new ApplicationMethod { ID = 1, Name = "Method 1" };
            var mappedMethod = new ApplicationMethodDto { ID = 1, Name = "Method 1" };

            _mockRepository.Setup(repo => repo.FetchByIdAsync(1))
                .ReturnsAsync(method);
            _mockMapper.Setup(m => m.Map<ApplicationMethodDto>(method))
                .Returns(mappedMethod);

            // Act
            var result = await _service.FetchByIdAsync(1);

            // Assert
            result.Should().NotBeNull();
            result.ID.Should().Be(1);
            result.Name.Should().Be("Method 1");
        }

        // Test FetchByCriteriaAsync()
        [Fact]
        public async Task FetchByCriteriaAsync_ShouldReturnMethods_WhenCriteriaIsSpecified()
        {
            // Arrange
            var methods = GetSampleApplicationMethods();
            var mappedMethods = GetSampleApplicationMethodDtos();

            _mockRepository.Setup(repo => repo.FetchByCriteriaAsync(It.IsAny<bool?>(), It.IsAny<int?>()))
                .ReturnsAsync(methods);
            _mockMapper.Setup(m => m.Map<IEnumerable<ApplicationMethodDto>>(methods))
                .Returns(mappedMethods);

            // Act
            var result = await _service.FetchByCriteriaAsync(isLiquid: true, fieldType: 1);

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(2); // Expect 2 methods
            result.Select(r => r.Name).Should().Contain(new[] { "Method 1", "Broadcast spreader" });
        }

        // Helper method to get sample application methods
        private IEnumerable<ApplicationMethod> GetSampleApplicationMethods()
        {
            return new List<ApplicationMethod>
            {
                new ApplicationMethod { ID = 1, Name = "Method 1", ApplicableForGrass = "L", ApplicableForArableAndHorticulture = "B" },
                new ApplicationMethod { ID = 2, Name = "Broadcast spreader", ApplicableForGrass = "B", ApplicableForArableAndHorticulture = "B" }
            };
        }

        // Helper method to get sample application method DTOs
        private IEnumerable<ApplicationMethodDto> GetSampleApplicationMethodDtos()
        {
            return new List<ApplicationMethodDto>
            {
                new ApplicationMethodDto { ID = 1, Name = "Method 1", ApplicableForGrass = "L", ApplicableForArableAndHorticulture = "B" },
                new ApplicationMethodDto { ID = 2, Name = "Broadcast spreader", ApplicableForGrass = "B", ApplicableForArableAndHorticulture = "B" }
            };
        }
    }
}
