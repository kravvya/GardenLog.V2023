using GardenLog.SharedKernel;
using GardenLog.SharedKernel.Interfaces;

namespace ImageCatalog.Api.Model;

public class Image : BaseEntity, IEntity
{
    public string UserProfileId { get; private set; }
    public string ImageName { get; private set; }
    public string Label { get; private set; }
    public ImageEntityEnum RelatedEntityType { get; private set; }
    public string RelatedEntityId { get; private set; }
    public string FileName { get; private set; }
    public string FileType { get; private set; }
    public DateTime CreatedDateTimeUtc { get; private set; }

    private Image()
    {

    }

    public static Image Create(
        string imageName,
        string label,
        ImageEntityEnum relatedEntity,
        string relatedEntityId,
        string fileName,
        string fileType,
        string userProfileId
        )
    {
        var image = new Image()
        {
            Id = System.Guid.NewGuid().ToString(),
            ImageName = imageName,
            Label = label,
            RelatedEntityType = relatedEntity,
            RelatedEntityId = relatedEntityId,
            FileName = fileName,
            FileType = fileType,
            UserProfileId = userProfileId,
            CreatedDateTimeUtc = DateTime.UtcNow
        };

        image.DomainEvents.Add(
            new ImageEvent(image, ImageCatalogEventTriggerEnum.ImageCreated, new TriggerEntity(image.RelatedEntityType, image.RelatedEntityId)));

        return image;
    }


    public void Update(string label)
    {
        this.Set<string>(() => this.Label, label);
    }

    protected override void AddDomainEvent(string attributeName)
    {
        if (this.DomainEvents.Count == 0)
        {
            if (attributeName.Equals("Label"))
                this.DomainEvents.Add(new ImageEvent(this, ImageCatalogEventTriggerEnum.LabelChanged, new TriggerEntity(this.RelatedEntityType, this.RelatedEntityId)));
        }
    }

}

