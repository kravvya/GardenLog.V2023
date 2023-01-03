﻿using PlantCatalog.Contract.Commands;

namespace PlantCatalog.Contract.Validators;

//https://docs.fluentvalidation.net/en/latest/aspnet.html
public class PlantCommandValidator<T> : AbstractValidator<T>
    where T : PlantBase
{
    public PlantCommandValidator()
    {
        RuleFor(command => command.Name).NotEmpty().Length(3, 50);
        RuleFor(command => command.Description).NotEmpty().MaximumLength(1000);
        RuleFor(command => command.Color).NotEmpty().MaximumLength(50);
        RuleFor(command => command.GardenTip).NotEmpty().MaximumLength(1000);
    }
}

public class CreatePlantCommandValidator : PlantCommandValidator<CreatePlantCommand>
{
    public CreatePlantCommandValidator()
    {
    }
}