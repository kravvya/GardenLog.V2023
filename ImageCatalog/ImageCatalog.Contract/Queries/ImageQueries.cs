using FluentValidation;
using ImageCatalog.Contract.Enum;

namespace ImageCatalog.Contract.Queries;

public record GetImagesByRelatedEntity(ImageEntityEnum RelatedEntityType, string? RelatedEntityId, bool FilterUserOnly);

public record GetImagesByRelatedEntities
{
    public List<GetImagesByRelatedEntity> Requests { get; set; }
}

public class GetImagesByRelatedEntityValidator : AbstractValidator<GetImagesByRelatedEntity>
{
    public GetImagesByRelatedEntityValidator()
    {
        RuleFor(command => command.RelatedEntityType).NotEmpty();
    }
}