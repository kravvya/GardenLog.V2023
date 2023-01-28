namespace UserManagement.Contract.Command;

public record UpdateGardenBedCommand: GardenBedBase
{
    public string GardenBedId { get; init; }
}

public class UpdateGardenBedCommandValidator : GardenBedValidator<UpdateGardenBedCommand>
{
    public UpdateGardenBedCommandValidator()
    {
        RuleFor(command => command.GardenBedId).NotEmpty().Length(3, 50);
    }
}
