using FluentAssertions;
using Manner.Core.Entities;
using Manner.Infrastructure.Data;
using Manner.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Manner.Tests.Repositories;

public class ApplicationMethodRepositoryTests
{
    private readonly ApplicationMethodRepository _repository;
    private readonly ApplicationDbContext _context;

    public ApplicationMethodRepositoryTests()
    {
        // Set up the in-memory database
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: "ApplicationMethodsTestDb")
            .Options;

        _context = new ApplicationDbContext(options);

        // Seed data
        SeedData(_context);

        var nullLogger = NullLogger<ApplicationMethodRepository>.Instance;

        _repository = new ApplicationMethodRepository(nullLogger, _context);
    }

    private void SeedData(ApplicationDbContext context)
    {
        var methods = new List<ApplicationMethod>
        {
            new ApplicationMethod { Name = "Method 1", ApplicableForGrass = "L", ApplicableForArableAndHorticulture = "B" },  // Liquid for Grass, Both for Arable
            new ApplicationMethod { Name = "Broadcast spreader", ApplicableForGrass = "B", ApplicableForArableAndHorticulture = "B" },  // Both for both fields
            new ApplicationMethod { Name = "Method 3", ApplicableForGrass = "B", ApplicableForArableAndHorticulture = null },  // Both for Grass, not applicable for Arable
            new ApplicationMethod { Name = "Method 4", ApplicableForGrass = null, ApplicableForArableAndHorticulture = "B" },  // Both for Arable, not applicable for Grass
            new ApplicationMethod { Name = "Liquid Arable", ApplicableForGrass = null, ApplicableForArableAndHorticulture = "L" },  // Liquid for Arable only
            new ApplicationMethod { Name = "Liquid Grass", ApplicableForGrass = "L", ApplicableForArableAndHorticulture = null },  // Liquid for Grass only
        };

        context.ApplicationMethods.RemoveRange(context.ApplicationMethods);
        context.SaveChanges();

        context.ApplicationMethods.AddRange(methods);
        context.SaveChanges();
    }



    [Fact]
    public async Task FetchByCriteriaAsync_ShouldReturnAllMethods_WhenIsLiquidIsTrue()
    {
        // Act
        var result = await _repository.FetchByCriteriaAsync(isLiquid: true);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(6);  // All methods have either 'B' or 'L' in one of the fields, so all are returned
    }

    [Fact]
    public async Task FetchByCriteriaAsync_ShouldReturnMethodsWithBInEitherColumn_WhenIsLiquidIsFalse()
    {
        // Act
        var result = await _repository.FetchByCriteriaAsync(isLiquid: false);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(4);  // Only methods with 'B' in either column: Method 1, Broadcast spreader, Method 3, Method 4
        result.Select(r => r.Name).Should().Contain(new[] { "Method 1", "Broadcast spreader", "Method 3", "Method 4" });
    }

    [Fact]
    public async Task FetchByCriteriaAsync_ShouldReturnMethodsWithBOrLInArable_WhenFieldTypeIsArable()
    {
        // Act
        var result = await _repository.FetchByCriteriaAsync(fieldType: 1);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(4);  // Methods with 'B' or 'L' in the Arable column: Method 1, Broadcast spreader, Method 4, Liquid Arable
        result.Select(r => r.Name).Should().Contain(new[] { "Method 1", "Broadcast spreader", "Method 4", "Liquid Arable" });
    }

    [Fact]
    public async Task FetchByCriteriaAsync_ShouldReturnMethodsWithBOrLInGrass_WhenFieldTypeIsGrass()
    {
        // Act
        var result = await _repository.FetchByCriteriaAsync(fieldType: 2);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(4);  // Methods with 'B' or 'L' in the Grass column: Method 1, Broadcast spreader, Method 3, Liquid Grass
        result.Select(r => r.Name).Should().Contain(new[] { "Method 1", "Broadcast spreader", "Method 3", "Liquid Grass" });
    }


    [Fact]
    public async Task FetchByCriteriaAsync_ShouldReturnArableLiquidMethods_WhenIsLiquidIsTrueAndFieldTypeIsArable()
    {
        // Act
        var result = await _repository.FetchByCriteriaAsync(isLiquid: true, fieldType: 1);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(4);  // Methods with 'B' or 'L' in the Arable field: Method 1, Broadcast spreader, Method 4, Liquid Arable
        result.Select(r => r.Name).Should().Contain(new[] { "Method 1", "Broadcast spreader", "Method 4", "Liquid Arable" });
    }


    [Fact]
    public async Task FetchByCriteriaAsync_ShouldReturnGrassLiquidMethods_WhenIsLiquidIsTrueAndFieldTypeIsGrass()
    {
        // Act
        var result = await _repository.FetchByCriteriaAsync(isLiquid: true, fieldType: 2);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(4);  // Methods with 'B' or 'L' in the Grass field: Method 1, Broadcast spreader, Method 3, Liquid Grass
        result.Select(r => r.Name).Should().Contain(new[] { "Method 1", "Broadcast spreader", "Method 3", "Liquid Grass" });
    }


    [Fact]
    public async Task FetchByCriteriaAsync_ShouldReturnNonLiquidArableMethods_WhenIsLiquidIsFalseAndFieldTypeIsArable()
    {
        // Act
        var result = await _repository.FetchByCriteriaAsync(isLiquid: false, fieldType: 1);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(3);  // Only methods with 'B' in the Arable field: Method 1, Broadcast spreader, Method 4
        result.Select(r => r.Name).Should().Contain(new[] { "Method 1", "Broadcast spreader", "Method 4" });
    }


    [Fact]
    public async Task FetchByCriteriaAsync_ShouldReturnNonLiquidGrassMethods_WhenIsLiquidIsFalseAndFieldTypeIsGrass()
    {
        // Act
        var result = await _repository.FetchByCriteriaAsync(isLiquid: false, fieldType: 2);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(2);  // Only methods with 'B' in the Grass field: Broadcast spreader, Method 3
        result.Select(r => r.Name).Should().Contain(new[] { "Broadcast spreader", "Method 3" });
    }


    [Fact]
    public async Task FetchByCriteriaAsync_ShouldReturnNoMethods_WhenNoDataExists()
    {
        // Clear all data from the database
        _context.ApplicationMethods.RemoveRange(_context.ApplicationMethods);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.FetchByCriteriaAsync();

        // Assert
        result.Should().BeEmpty();  // Expect no methods returned
    }

    [Fact]
    public async Task FetchByCriteriaAsync_ShouldReturnAllMethods_WhenInvalidFieldTypeIsProvided()
    {
        // Act
        var result = await _repository.FetchByCriteriaAsync(fieldType: 999);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(6);  // Invalid fieldType should return all methods since no filtering is applied
    }




    // Tear down in-memory database after each test
    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }
}
