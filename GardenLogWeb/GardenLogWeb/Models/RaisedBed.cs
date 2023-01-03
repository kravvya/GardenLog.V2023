using GardenLogWeb.Shared;

namespace GardenLogWeb.Models
{
    public class RaisedBed: PlantingBed
    {
        public RaisedBed()
        {
            BorderColor = Constants.RaisedBedBorderColor;
        }
        public int Height { get; set; }
    }
}
