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
                Territory= "EnglandWalesScotland",
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
                Territory = "EnglandWalesScotland",
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
                Territory = "EnglandWalesScotland",
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
                Territory = "EnglandWalesScotland",
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
                Territory = "EnglandWalesScotland",
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

        [Fact]
        public void CalculateRainfallPostApplication_PreviousYearToNextYear_ShouldReturnCorrectRainfall_BB10()
        {
            // Arrange
            var climate = new ClimateDto
            {
                Territory = "EnglandWalesScotland",
                PostCode = "BB10",
                MeanTotalRainFallOct = 147.038808600m,
                MeanTotalRainFallNov = 138.823611600m,
                MeanTotalRainFallDec = 164.330885400m,
                MeanTotalRainFallJan = 148.321984300m,
                MeanTotalRainFallFeb = 110.597997900m,
                MeanTotalRainFallMar = 100.401203100m,
                MeanTotalRainFallApr = 86.786013080m
            };
            var applicationDate = new DateOnly(2023, 10, 15);
            var endSoilDrainageDate = new DateOnly(2024, 4, 15);

            // Act
            var result = _calculator.CalculateRainfallPostApplication(climate, applicationDate, endSoilDrainageDate);

            // Assert
            result.Should().Be(782);  // Adjust this value based on the exact proportional calculation.
        }

        [Fact]
        public void CalculateRainfallPostApplication_PreviousYearToNextYear_ShouldReturnCorrectRainfall_BB11()
        {
            // Arrange
            var climate = new ClimateDto
            {
                Territory = "EnglandWalesScotland",
                PostCode = "BB11",
                MeanTotalRainFallNov = 132.528806400m,
                MeanTotalRainFallDec = 155.132262700m,
                MeanTotalRainFallJan = 143.774361400m,
                MeanTotalRainFallFeb = 108.131827100m,
                MeanTotalRainFallMar = 99.394843070m
            };
            var applicationDate = new DateOnly(2023, 11, 1);
            var endSoilDrainageDate = new DateOnly(2024, 3, 31);

            // Act
            var result = _calculator.CalculateRainfallPostApplication(climate, applicationDate, endSoilDrainageDate);

            // Assert
            result.Should().Be(635);  // Adjust this value based on the exact proportional calculation.
        }

        [Fact]
        public void CalculateRainfallPostApplication_PreviousYearToNextYear_ShouldReturnCorrectRainfall_BB12()
        {
            // Arrange
            var climate = new ClimateDto
            {
                Territory = "EnglandWalesScotland",
                PostCode = "BB12",
                MeanTotalRainFallDec = 139.704101100m,
                MeanTotalRainFallJan = 121.728579200m,
                MeanTotalRainFallFeb = 92.499074490m
            };
            var applicationDate = new DateOnly(2023, 12, 15);
            var endSoilDrainageDate = new DateOnly(2024, 2, 15);

            // Act
            var result = _calculator.CalculateRainfallPostApplication(climate, applicationDate, endSoilDrainageDate);

            // Assert
            result.Should().Be(242);  // Adjust this value based on the exact proportional calculation.
        }

        [Fact]
        public void CalculateRainfallPostApplication_PreviousYearToNextYear_ShouldReturnCorrectRainfall_BB11_WholeMonths()
        {
            // Arrange
            var climate = new ClimateDto
            {
                Territory = "EnglandWalesScotland",
                PostCode = "BB12",
                MeanTotalRainFallOct = 146.216752100m,
                MeanTotalRainFallNov = 132.528806400m,
                MeanTotalRainFallDec = 155.132262700m,
                MeanTotalRainFallJan = 143.774361400m,
                MeanTotalRainFallFeb = 108.131827100m,
                MeanTotalRainFallMar = 99.394843070m,
                MeanTotalRainFallApr = 85.703282570m
            };
            var applicationDate = new DateOnly(2023, 10, 1);
            var endSoilDrainageDate = new DateOnly(2024, 4, 30);

            // Act
            var result = _calculator.CalculateRainfallPostApplication(climate, applicationDate, endSoilDrainageDate);

            // Assert
            result.Should().Be(867);  // Adjusted to the correct total rainfall value based on BB11 data.
        }


    }
}
