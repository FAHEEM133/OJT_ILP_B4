using Application.Requests.MarketRequests;
using Application.DTOs;
using Domain.Enums;
using Domain.Model;
using FluentValidation;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using Domain.Enums.Domain.Enums;

namespace UnitTests;

[TestFixture]
public class CreateMarketCommandHandlerTests
{
    private AppDbContext _context;
    private CreateMarketCommandHandler _handler;
    private CancellationToken _cancellationToken;

    [SetUp]
    public void SetUp()
    {
        // Use InMemory database for testing
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName:"TestDatabase") // Unique name for the in-memory database
            .Options;   

        _context = new AppDbContext(options);
        _cancellationToken = new CancellationToken();

        // Initialize the handler with the actual validator
        var validator = new CreateMarketCommandValidator();
        _handler = new CreateMarketCommandHandler(_context, validator);
    }

    [TearDown]
    public void TearDown()
    {
        // Ensure the in-memory database is cleared between tests
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }

    [Test]
    public async Task Handle_ShouldCreateMarket_WhenValidRequest()
    {
        // Arrange
        var command = new CreateMarketCommand
        {
            Name = "Test Market",
            Code = "TM",
            LongMarketCode = "E-GO.AF.TM",
            Region = Region.EURO,
            SubRegion = SubRegion.Europe,
            MarketSubGroups = new List<MarketSubGroupDTO>
            {
                new MarketSubGroupDTO { SubGroupName = "SubGroup A", SubGroupCode = "S" }
            }
        };


        // Act
        var result = await _handler.Handle(command, _cancellationToken);

        // Assert: Ensure market is created (assuming the first market ID is 1)
        Assert.That(result, Is.EqualTo(1));
    }

    [Test]
    public async Task Handle_ShouldThrowValidationException_WhenMarketNameIsEmpty()
    {
        // Arrange: Empty market name
        var command = new CreateMarketCommand
        {
            Name = "", // Invalid name
            Code = "TM",
            LongMarketCode = "E-GO.AF.TM",
            Region = Region.EURO,
            SubRegion = SubRegion.Europe,
            MarketSubGroups = new List<MarketSubGroupDTO>
            {
                new MarketSubGroupDTO { SubGroupName = "SubGroup A", SubGroupCode = "S" }
            }
        };

        // Act & Assert: Expect a validation exception
        var ex = Assert.ThrowsAsync<ValidationException>(async () => await _handler.Handle(command, _cancellationToken));
        Assert.That(ex.Message, Does.Contain("Market name is required."));
    }

    [Test]
    public async Task Handle_ShouldThrowValidationException_WhenMarketNameExceedsMaxLength()
    {
        // Arrange: Market name exceeding 150 characters
        var command = new CreateMarketCommand
        {
            Name = new string('A', 151), // Invalid name, length > 150
            Code = "TM",
            LongMarketCode = "E-GO.AF.TM",
            Region = Region.EURO,
            SubRegion = SubRegion.Europe,
            MarketSubGroups = new List<MarketSubGroupDTO>
            {
                new MarketSubGroupDTO { SubGroupName = "SubGroup A", SubGroupCode = "S" }
            }
        };

        // Act & Assert: Expect a validation exception
        var ex = Assert.ThrowsAsync<ValidationException>(async () => await _handler.Handle(command, _cancellationToken));
        Assert.That(ex.Message, Does.Contain("Market name must be less than 150 characters."));
    }

    [Test]
    public void Handle_ShouldThrowValidationException_WhenMarketNameIsNotUnique()
    {
        // Arrange
        var command = new CreateMarketCommand
        {
            Name = "Existing Market",
            Code = "EM",
            LongMarketCode = "E-GO.AF.EM",
            Region = Region.EURO,
            SubRegion = SubRegion.Europe
        };

        // Simulate a market already exists with the same name
        var existingMarket = new Market
        {
            Name = "Existing Market",
            Code = "EM",
            LongMarketCode = "E-GO.AF.EM",
            Region = Region.EURO,
            SubRegion = SubRegion.Europe
        };

        _context.Markets.Add(existingMarket);
        _context.SaveChanges(); // Save the existing market

        // Act & Assert: Expect validation to fail due to non-unique market name
        var ex = Assert.ThrowsAsync<System.ComponentModel.DataAnnotations.ValidationException>(
            async () => await _handler.Handle(command, _cancellationToken)
        );

        Assert.That(ex.Message, Is.EqualTo("A market with this name already exists."));
    }

    [Test]
    public async Task Handle_ShouldThrowValidationException_WhenMarketCodeIsEmpty()
    {
        // Arrange: Empty market code
        var command = new CreateMarketCommand
        {
            Name = "Test Market",
            Code = "", // Invalid market code
            LongMarketCode = "E-GO.AF.TM",
            Region = Region.EURO,
            SubRegion = SubRegion.Europe,
            MarketSubGroups = new List<MarketSubGroupDTO>
            {
                new MarketSubGroupDTO { SubGroupName = "SubGroup A", SubGroupCode = "S" }
            }
        };

        // Act & Assert: Expect a validation exception
        var ex = Assert.ThrowsAsync<ValidationException>(async () => await _handler.Handle(command, _cancellationToken));
        Assert.That(ex.Message, Does.Contain("Market code is required."));
    }

    [Test]
    public async Task Handle_ShouldThrowValidationException_WhenMarketCodeExceedsMaxLength()
    {
        // Arrange: Market code not exactly 2 characters long
        var command = new CreateMarketCommand
        {
            Name = "Test Market",
            Code = "AAA", // Invalid market code (should be exactly 2 characters)
            LongMarketCode = "E-GO.AF.AAA",
            Region = Region.EURO,
            SubRegion = SubRegion.Europe,
            MarketSubGroups = new List<MarketSubGroupDTO>
            {
                new MarketSubGroupDTO { SubGroupName = "SubGroup A", SubGroupCode = "S" }
            }
        };

        // Act & Assert: Expect a validation exception
        var ex = Assert.ThrowsAsync<ValidationException>(async () => await _handler.Handle(command, _cancellationToken));
        Assert.That(ex.Message, Does.Contain("Market code should have 2 characters"));
    }

    [Test]
    public async Task Handle_ShouldThrowValidationException_WhenMarketCodeContainsNonAlphabeticCharacters()
    {
        // Arrange: Market code containing non-alphabetic characters
        var command = new CreateMarketCommand
        {
            Name = "Test Market",
            Code = "T1", // Invalid market code (contains a digit)
            LongMarketCode = "E-GO.AF.T1",
            Region = Region.EURO,
            SubRegion = SubRegion.Europe,
            MarketSubGroups = new List<MarketSubGroupDTO>
        {
            new MarketSubGroupDTO { SubGroupName = "SubGroup A", SubGroupCode = "S" }
        }
        };

        // Act & Assert: Expect a validation exception
        var ex = Assert.ThrowsAsync<ValidationException>(async () => await _handler.Handle(command, _cancellationToken));
        Assert.That(ex.Message, Does.Contain("Market code should only contain alphabetic characters."));
    }


    [Test]
    public void Handle_ShouldThrowValidationException_WhenMarketCodeIsNotUnique()
    {
        // Arrange
        var command = new CreateMarketCommand
        {
            Name = "Existing Market 1",
            Code = "EM",
            LongMarketCode = "E-GO.AF.EM",
            Region = Region.EURO,
            SubRegion = SubRegion.Europe
        };

        // Simulate a market already exists with the same name
        var existingMarket = new Market
        {
            Name = "Existing Market 2",
            Code = "EM",
            LongMarketCode = "E-GO.AF.EM",
            Region = Region.EURO,
            SubRegion = SubRegion.Europe
        };

        _context.Markets.Add(existingMarket);
        _context.SaveChanges(); // Save the existing market

        // Act & Assert: Expect validation to fail due to non-unique market name
        var ex = Assert.ThrowsAsync<System.ComponentModel.DataAnnotations.ValidationException>(
            async () => await _handler.Handle(command, _cancellationToken)
        );

        Assert.That(ex.Message, Is.EqualTo("A market with this code already exists."));
    }

    [Test]
    public async Task Handle_ShouldThrowValidationException_WhenSubGroupCodeExceedsMaxLength()
    {
        // Arrange: SubGroupCode exceeding 1 character
        var command = new CreateMarketCommand
        {
            Name = "Test Market",
            Code = "TM",
            LongMarketCode = "E-GO.AF.TM",
            Region = Region.EURO,
            SubRegion = SubRegion.Europe,
            MarketSubGroups = new List<MarketSubGroupDTO>
        {
            new MarketSubGroupDTO { SubGroupName = "SubGroup A", SubGroupCode = "AA" } // Invalid: too long
        }
        };

        // Act & Assert: Expect a validation exception
        var ex = Assert.ThrowsAsync<ValidationException>(async () => await _handler.Handle(command, _cancellationToken));
        Assert.That(ex.Message, Does.Contain("SubGroupCode must be a single character."));
    }

    [Test]
    public async Task Handle_ShouldThrowValidationException_WhenSubGroupCodeIsNotAlphanumeric()
    {
        // Arrange: Invalid SubGroupCode (contains special character)
        var command = new CreateMarketCommand
        {
            Name = "Test Market",
            Code = "TM",
            LongMarketCode = "E-GO.AF.TM",
            Region = Region.EURO,
            SubRegion = SubRegion.Europe,
            MarketSubGroups = new List<MarketSubGroupDTO>
        {
            new MarketSubGroupDTO { SubGroupName = "SubGroup A", SubGroupCode = "@" } // Invalid: non-alphanumeric
        }
        };

        // Act & Assert: Expect a validation exception
        var ex = Assert.ThrowsAsync<ValidationException>(async () => await _handler.Handle(command, _cancellationToken));
        Assert.That(ex.Message, Does.Contain("SubGroupCode must be a single alphanumeric character."));
    }

    [Test]
    public async Task Handle_ShouldThrowValidationException_WhenSubGroupCodeIsEmpty()
    {
        // Arrange: SubGroupCode is empty
        var command = new CreateMarketCommand
        {
            Name = "Test Market",
            Code = "TM",
            LongMarketCode = "E-GO.AF.TM",
            Region = Region.EURO,
            SubRegion = SubRegion.Europe,
            MarketSubGroups = new List<MarketSubGroupDTO>
        {
            new MarketSubGroupDTO { SubGroupName = "SubGroup A", SubGroupCode = "" } // Invalid: empty code
        }
        };

        // Act & Assert: Expect a validation exception
        var ex = Assert.ThrowsAsync<ValidationException>(async () => await _handler.Handle(command, _cancellationToken));
        Assert.That(ex.Message, Does.Contain("SubGroupCode is required."));
    }

    [Test]
    public void Handle_ShouldThrowValidationException_WhenSubGroupCodeIsNotUnique()
    {
        // Arrange
        var command = new CreateMarketCommand
        {
            Name = "Test Market",
            Code = "TM",
            LongMarketCode = "E-GO.AF.TM",
            Region = Region.EURO,
            SubRegion = SubRegion.Europe,
            MarketSubGroups = new List<MarketSubGroupDTO>
        {
            new MarketSubGroupDTO { SubGroupName = "SubGroup 1", SubGroupCode = "A" },
            new MarketSubGroupDTO { SubGroupName = "SubGroup 2", SubGroupCode = "A" } // Duplicate code
        }
        };

        // Act & Assert: Expect validation to fail due to non-unique subgroup code
        var ex = Assert.ThrowsAsync<System.ComponentModel.DataAnnotations.ValidationException>(async () => await _handler.Handle(command, _cancellationToken));

        Assert.That(ex.Message, Does.Contain("SubGroupCode must be unique within the market."));
    }

    [Test]
    public void Handle_ShouldThrowValidationException_WhenSubGroupNameIsNotUnique()
    {
        // Arrange
        var command = new CreateMarketCommand
        {
            Name = "Test Market",
            Code = "TM",
            LongMarketCode = "E-GO.AF.TM",
            Region = Region.EURO,
            SubRegion = SubRegion.Europe,
            MarketSubGroups = new List<MarketSubGroupDTO>
        {
            new MarketSubGroupDTO { SubGroupName = "SubGroup 1", SubGroupCode = "A" },
            new MarketSubGroupDTO { SubGroupName = "SubGroup 1", SubGroupCode = "B" } // Duplicate code
        }
        };

        // Act & Assert: Expect validation to fail due to non-unique subgroup code
        var ex = Assert.ThrowsAsync<System.ComponentModel.DataAnnotations.ValidationException>(async () => await _handler.Handle(command, _cancellationToken));

        Assert.That(ex.Message, Does.Contain("SubGroupName must be unique within the market."));
    }


    [Test]
    public async Task Handle_ShouldThrowValidationException_WhenSubGroupNameIsEmpty()
    {
        // Arrange: SubGroupName is empty
        var command = new CreateMarketCommand
        {
            Name = "Test Market",
            Code = "TM",
            LongMarketCode = "E-GO.AF.TM",
            Region = Region.EURO,
            SubRegion = SubRegion.Europe,
            MarketSubGroups = new List<MarketSubGroupDTO>
        {
            new MarketSubGroupDTO { SubGroupName = "", SubGroupCode = "A" } // Invalid: empty name
        }
        };

        // Act & Assert: Expect a validation exception
        var ex = Assert.ThrowsAsync<ValidationException>(async () => await _handler.Handle(command, _cancellationToken));
        Assert.That(ex.Message, Does.Contain("SubGroup name is required."));
    }

    [Test]
    public async Task Handle_ShouldThrowValidationException_WhenSubGroupNameExceedsMaxLength()
    {
        // Arrange: SubGroupName exceeding max length of 150 characters
        var command = new CreateMarketCommand
        {
            Name = "Test Market",
            Code = "TM",
            LongMarketCode = "E-GO.AF.TM",
            Region = Region.EURO,
            SubRegion = SubRegion.Europe,
            MarketSubGroups = new List<MarketSubGroupDTO>
        {
            new MarketSubGroupDTO { SubGroupName = new string('A', 151), SubGroupCode = "A" } // Invalid: too long
        }
        };

        // Act & Assert: Expect a validation exception
        var ex = Assert.ThrowsAsync<ValidationException>(async () => await _handler.Handle(command, _cancellationToken));
        Assert.That(ex.Message, Does.Contain("Subgroup name must be less than 150 characters."));
    }

    [Test]
    public async Task Handle_ShouldThrowValidationException_WhenLongMarketCodeIsInvalid()
    {
        // Arrange: Invalid LongMarketCode (contains invalid characters)
        var command = new CreateMarketCommand
        {
            Name = "Test Market",
            Code = "TM",
            LongMarketCode = "E-GO$AF.TM", // Invalid: contains special character $
            Region = Region.EURO,
            SubRegion = SubRegion.Europe,
            MarketSubGroups = new List<MarketSubGroupDTO>
        {
            new MarketSubGroupDTO { SubGroupName = "SubGroup A", SubGroupCode = "A" }
        }
        };

        // Act & Assert: Expect a validation exception
        var ex = Assert.ThrowsAsync<ValidationException>(async () => await _handler.Handle(command, _cancellationToken));
        Assert.That(ex.Message, Does.Contain("Long Market Code must be in the format X-XX.XX.XX"));
    }

    [Test]
    public async Task Handle_ShouldThrowValidationException_WhenLongMarketCodeIsEmpty()
    {
        // Arrange: Empty LongMarketCode
        var command = new CreateMarketCommand
        {
            Name = "Test Market",
            Code = "TM",
            LongMarketCode = "",  // Empty long market code
            Region = Region.EURO,
            SubRegion = SubRegion.Europe,
            MarketSubGroups = new List<MarketSubGroupDTO>
        {
            new MarketSubGroupDTO { SubGroupName = "SubGroup A", SubGroupCode = "S" }
        }
        };

        // Act & Assert: Expect a validation exception
        var ex = Assert.ThrowsAsync<ValidationException>(async () =>
            await _handler.Handle(command, _cancellationToken));

        Assert.That(ex.Message, Does.Contain("Long Market Code is required."));
    }


}
