namespace PlantHarvest.Contract.Validators;

public class WorkLogValidator<T> : AbstractValidator<T>
    where T : WorkLogBase
{
    public WorkLogValidator()
    {
        RuleFor(command => command.RelatedEntityid).NotEmpty().Length(3, 100);
        RuleFor(command => command.Log).NotEmpty().Length(3, 1000);
        RuleFor(command => command.EventDateTime).NotNull();
        RuleFor(command => command.Reason).Must(r => r != WorkLogReasonEnum.Unspecified).WithMessage("Please select reason");
    }
}
