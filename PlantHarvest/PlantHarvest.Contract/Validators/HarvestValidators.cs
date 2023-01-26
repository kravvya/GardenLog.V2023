namespace PlantHarvest.Contract.Validators;

//https://docs.fluentvalidation.net/en/latest/aspnet.html
public class HarvestCycleValidator<T> : AbstractValidator<T>
    where T : HarvestCycleBase
{
    public HarvestCycleValidator()
    {
        RuleFor(command => command.HarvestCycleName).NotEmpty().Length(3, 100);
        RuleFor(command => command.Notes).NotNull().MaximumLength(1000);
        RuleFor(command => command.GardenId).NotEmpty().Length(3, 100);
    }
}

public class PlantHarvestCycleValidator<T>:AbstractValidator<T>
    where T: PlantHarvestCycleBase
{
    public PlantHarvestCycleValidator()
    {
        RuleFor(command => command.HarvestCycleId).NotEmpty().Length(2, 50);
        RuleFor(command => command.PlantId).NotEmpty().Length(2, 50);
        //RuleFor(command => command.PlantVarietyId).NotEmpty().Length(2, 50);
        //RuleFor(command => command.GardenBedId).NotEmpty().Length(2, 50);
        RuleFor(command => command.Notes).NotNull().MaximumLength(1000);
    }
}