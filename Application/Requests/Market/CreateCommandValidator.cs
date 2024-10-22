using FluentValidation;
using Application.Requests.MarketRequests;
using Application.Validations;
using Application.DTOs;

public class CreateMarketCommandValidator : AbstractValidator<CreateMarketCommand>
{
    public CreateMarketCommandValidator()
    {
        
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Market name is required.")
            .MaximumLength(150).WithMessage("Market name must be less than 150 characters.");


        RuleFor(x => x.Code)
            .NotEmpty().WithMessage("Market code is required.")
            .Length(2).WithMessage("Market code should have 2 characters")
            .Matches("^[a-zA-Z]{2}$").WithMessage("Market code should only contain alphabetic characters.");


        RuleFor(x => x.LongMarketCode)
            .NotEmpty().WithMessage("Long Market Code is required.")
            .Matches(@"^[A-Z]-[A-Z]{2}\.[A-Z]{2}\.[A-Z]{2}$")
            .WithMessage("Long Market Code must be in the format X-XX.XX.XX.");
            

        RuleFor(x => x)
            .Must(x => RegionSubRegionValidation.IsValidSubRegionForRegion(x.Region, x.SubRegion))
            .WithMessage(x => $"SubRegion {x.SubRegion} is not valid for the Region {x.Region}");

        
        RuleForEach(x => x.MarketSubGroups)
            .SetValidator(new MarketSubGroupValidator());
    }
}

public class MarketSubGroupValidator : AbstractValidator<MarketSubGroupDTO>
{
    public MarketSubGroupValidator()
    {
        RuleFor(x => x.SubGroupName)
            .NotEmpty().WithMessage("SubGroup name is required.")
            .MaximumLength(150).WithMessage("Subgroup name must be less than 150 characters.");

        RuleFor(x => x.SubGroupCode)
            .NotEmpty().WithMessage("SubGroupCode is required.")
            .Length(1).WithMessage("SubGroupCode must be a single character.")
            .Matches("^[A-Za-z0-9]{1}$").WithMessage("SubGroupCode must be a single alphanumeric character.");
    }
}
