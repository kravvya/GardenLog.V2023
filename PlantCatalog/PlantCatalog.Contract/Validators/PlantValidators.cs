namespace PlantCatalog.Contract.Validators;

//https://docs.fluentvalidation.net/en/latest/aspnet.html
public class PlantValidator<T> : AbstractValidator<T>
    where T : PlantBase
{
    public PlantValidator()
    {
        RuleFor(command => command.Name).NotEmpty().Length(3, 50);
        RuleFor(command => command.Description).NotEmpty().MaximumLength(1000);
        RuleFor(command => command.Color).NotEmpty().MaximumLength(50);
        RuleFor(command => command.GardenTip).NotEmpty().MaximumLength(1000);
        RuleFor(command => command.Type).NotEmpty().WithMessage("Plant type has to be selected");
    }
}

public class PlantGrowInstructionValidator<T> : AbstractValidator<T>
    where T : PlantGrowInstructionBase
{
    public PlantGrowInstructionValidator()
    {
        RuleFor(command => command.Name).NotEmpty().Length(3, 50);
    }
}