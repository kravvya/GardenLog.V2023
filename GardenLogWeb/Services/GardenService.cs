using GardenLogWeb.Models;

namespace GardenLogWeb.Services
{
    public interface IGardenService
    {
        Garden GetGarden();
    }

    public class GardenService : IGardenService
    {


        #region Garden Functions
        public Garden GetGarden()
        {
            Garden garden = new Garden();
            garden.GardenName = "Steve's Garden";
            garden.BorderColor = "#585858";
            garden.Length = 180;
            garden.Width= 240;
            return garden;
        }
        #endregion
    }
}
