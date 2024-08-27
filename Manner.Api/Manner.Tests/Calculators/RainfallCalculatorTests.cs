using Manner.Application.Calculators;
using Manner.Application.DTOs;
using Xunit;
using FluentAssertions;

namespace Manner.Tests.Calculators
{
    public class RainfallCalculatorTests
    {
        private readonly RainfallCalculator _calculator;

        public RainfallCalculatorTests()
        {
            _calculator = new RainfallCalculator();
        }

        [Fact]
        public void CalculateRainfallPostApplication_ShouldReturnCorrectRainfall_ForAL1ClimateData_JanToApr()
        {
            // Arrange
            var climate = new ClimateDto
            {
                PostCode = "AL1",
                MeanTotalRainFallJan = 69.550721170m,
                MeanTotalRainFallFeb = 48.041668180m,
                MeanTotalRainFallMar = 41.082543470m,
                MeanTotalRainFallApr = 49.688871710m,
            };
            var applicationDate = new DateOnly(2024, 1, 18);
            var endSoilDrainageDate = new DateOnly(2024, 4, 27);

            // Act
            var result = _calculator.CalculateRainfallPostApplication(climate, applicationDate, endSoilDrainageDate);

            // Assert
            result.Should().Be(164); // Based on output of old tool.
        }

        [Fact]
        public void CalculateRainfallPostApplication_ShouldReturnCorrectRainfall_ForAL10ClimateData_FebToApr()
        {
            // Arrange
            var climate = new ClimateDto
            {
                PostCode = "AL10",
                MeanTotalRainFallJan = 64.515481210m,
                MeanTotalRainFallFeb = 46.731500640m,
                MeanTotalRainFallMar = 39.774959690m,
                MeanTotalRainFallApr = 47.017421080m,
            };
            var applicationDate = new DateOnly(2024, 2, 18);
            var endSoilDrainageDate = new DateOnly(2024, 4, 27);

            // Act
            var result = _calculator.CalculateRainfallPostApplication(climate, applicationDate, endSoilDrainageDate);

            // Assert
            result.Should().Be(100); // Based on output of old tool.
        }

        [Fact]
        public void CalculateRainfallPostApplication_ShouldReturnCorrectRainfall_ForAL2ClimateData_MarToApr()
        {
            // Arrange
            var climate = new ClimateDto
            {
                PostCode = "AL2",
                MeanTotalRainFallJan = 68.985180580m,
                MeanTotalRainFallFeb = 49.709538810m,
                MeanTotalRainFallMar = 42.170457590m,
                MeanTotalRainFallApr = 50.919351920m,
            };
            var applicationDate = new DateOnly(2024, 3, 1);
            var endSoilDrainageDate = new DateOnly(2024, 4, 27);

            // Act
            var result = _calculator.CalculateRainfallPostApplication(climate, applicationDate, endSoilDrainageDate);

            // Assert
            result.Should().Be(87); // Based on output of old tool.
        }

        [Fact]
        public void CalculateRainfallPostApplication_ShouldReturnZero_ForEndSoilDrainageBeforeApplicationDate()
        {
            // Arrange
            var climate = new ClimateDto
            {
                PostCode = "AL3",
                MeanTotalRainFallJan = 70.709515820m,
                MeanTotalRainFallFeb = 50.605571410m,
                MeanTotalRainFallMar = 43.422745830m,
                MeanTotalRainFallApr = 52.830629990m,
            };
            var applicationDate = new DateOnly(2024, 2, 18);
            var endSoilDrainageDate = new DateOnly(2024, 2, 17); // End date before application date

            // Act
            var result = _calculator.CalculateRainfallPostApplication(climate, applicationDate, endSoilDrainageDate);

            // Assert
            result.Should().Be(0); // Based on output of old tool.
        }

        [Fact]
        public void CalculateRainfallPostApplication_ShouldHandleFullMonthCorrectly()
        {
            // Arrange
            var climate = new ClimateDto
            {
                PostCode = "AL4",
                MeanTotalRainFallJan = 67.570715100m,
                MeanTotalRainFallFeb = 48.491295090m,
                MeanTotalRainFallMar = 41.305389400m,
                MeanTotalRainFallApr = 49.085841670m,
            };
            var applicationDate = new DateOnly(2024, 1, 1); // Beginning of the month
            var endSoilDrainageDate = new DateOnly(2024, 4, 30); // End of the month

            // Act
            var result = _calculator.CalculateRainfallPostApplication(climate, applicationDate, endSoilDrainageDate);

            // Assert
            result.Should().Be(205); // Based on output of old tool.
        }
    }
}
