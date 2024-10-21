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
            .Length(3, 100).WithMessage("Market name must be between 3 and 100 characters.");

        
        RuleFor(x => x.Code)
            .NotEmpty().WithMessage("Market code is required.")
            .Length(2).WithMessage("Market code shouuld have 2 charactors");

       
        RuleFor(x => x.LongMarketCode)
            .Length(3, 20).When(x => !string.IsNullOrEmpty(x.LongMarketCode))
            .WithMessage("Long Market Code must be between 3 and 20 characters.");

 
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
        // Rule 1: SubGroupName must not be empty
        RuleFor(x => x.SubGroupName)
            .NotEmpty().WithMessage("SubGroup name is required.")
            .Length(2, 50).WithMessage("SubGroup name must be between 2 and 50 characters.");

        // Rule 2: SubGroupCode must follow the validation logic
        RuleFor(x => x.SubGroupCode)
            .NotEmpty().WithMessage("SubGroupCode is required.")
            .Must(SubGroupValidation.IsValidSubGroupCode).WithMessage("SubGroupCode must be a single alphanumeric character.");
    }
}
