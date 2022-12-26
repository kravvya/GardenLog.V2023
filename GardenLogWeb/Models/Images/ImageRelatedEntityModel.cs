namespace GardenLogWeb.Models.Images;

public record ImageRelatedEntities
{
    public IList<ImageRelatedEntityModel> Requests { get; set; }
}

public record ImageRelatedEntityModel : GetImagesByRelatedEntity
{
    public ImageRelatedEntityModel(string RelatedEntityType, string RelatedEntityId, bool FilterUserOnlyl) : base(RelatedEntityType, RelatedEntityId, FilterUserOnlyl)
    {
    }
}

public static class RelatedEntityTypes
{
    public const string RELATED_ENTITY_PLANT = "Plant";
    public const string RELATED_ENTITY_PLANT_VARIETY = "PlantVariety";
    public const string RELATED_ENTITY_PLANT_LOCATION = "PlantLocation";
    public const string RELATED_ENTITY_SEED = "Seed";
    public const string RELATED_ENTITY_SEED_VENDOR = "SeedVendor";
    public const string RELATED_ENTITY_HARVEST_CYCLE = "HarvestCycle";
    public const string RELATED_ENTITY_PLANT_HARVEST_CYCLE = "PlantHarvestCycle";
    public const string RELATED_ENTITY_TASK = "Task";
}

//Todo - remove this.
public record GetImagesByRelatedEntity(string RelatedEntityType, string? RelatedEntityId, bool FilterUserOnly);