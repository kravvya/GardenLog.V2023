using GardenLogWeb.Shared;

namespace GardenLogWeb.Models
{
    public class GardenBed: PlantingBed
    {
        public GardenBed()
        {
            BorderColor = Constants.GardenBedBorderColor;
        }
    }
}
