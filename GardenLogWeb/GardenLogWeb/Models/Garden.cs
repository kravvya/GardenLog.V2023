namespace GardenLogWeb.Models
{
    public class Garden: VisualComponent
    {
        public Guid Id { get; set; }
        public string GardenName { get; set; } = string.Empty;
        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }
        public Guid UserId { get; set; }

        public IList<PlantingBed> Beds { get; set; }=new List<PlantingBed>();

     }
}
