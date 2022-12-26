namespace GardenLogWeb.Models
{
    public abstract class PlantingBed: VisualComponent
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }
}
