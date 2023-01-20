namespace PlantHarvest.Contract.Commands;


#region Harvest Cycle
public record CreateHarvestCycleCommand : HarvestCycleBase
{ }

public record UpdateHarvestCycleCommand : HarvestCycleBase
{
    public string HarvestCycleId { get; init; }
}

public class CreateHarvestCycleCommandValidator : HarvestCycleValidator<CreateHarvestCycleCommand>
{
    public CreateHarvestCycleCommandValidator()
    {
    }
}

public class UpdateHarvestCycleCommandValidator : HarvestCycleValidator<UpdateHarvestCycleCommand>
{
    public UpdateHarvestCycleCommandValidator()
    {
        RuleFor(command => command.HarvestCycleId).NotEmpty().Length(3, 50);
    }
}
#endregion

#region Plan Harvest Cycle
public record CreatePlanHarvestCycleCommand : PlanHarvestCycleBase
{ }

public record UpdatePlanHarvestCycleCommand : PlanHarvestCycleBase
{
    public string PlanHarvestCycleId { get; init; }
}

public class CreatePlanHarvestCycleCommandValidator : PlanHarvestCycleValidator<CreatePlanHarvestCycleCommand>
{
    public CreatePlanHarvestCycleCommandValidator()
    {
    }
}

public class UpdatePlanHarvestCycleCommandValidator : PlanHarvestCycleValidator<UpdatePlanHarvestCycleCommand>
{
    public UpdatePlanHarvestCycleCommandValidator()
    {
        RuleFor(command => command.PlanHarvestCycleId).NotEmpty().Length(3, 50);
    }
}
#endregion

#region Plant Harvest Cycle

public record CreatePlantHarvestCycleCommand : PlantHarvestCycleBase
{ }

public record UpdatePlantHarvestCycleCommand : PlantHarvestCycleBase
{
    public string PlantHarvestCycleId { get; init; }
}

public class CreatePlantHarvestCycleCommandValidator : PlantHarvestCycleValidator<CreatePlantHarvestCycleCommand>
{
    public CreatePlantHarvestCycleCommandValidator()
    {
    }
}

public class UpdatePlantHarvestCycleCommandValidator : PlantHarvestCycleValidator<UpdatePlantHarvestCycleCommand>
{
    public UpdatePlantHarvestCycleCommandValidator()
    {
        RuleFor(command => command.PlantHarvestCycleId).NotEmpty().Length(3, 50);
    }
}
#endregion
