using GardenLogWeb.Pages.GardenLayout.Components;

namespace GardenLogWeb.Models.UserProfile;

public record GardenBedModel : GardenBedViewModel, IVisualComponent
{
    public double GetHeightInPixels()
    {
        return this.Length / 12 * GardenPlanSettings.TickFootHeight;
    }

    public double GetWidthInPixels()
    {
        return this.Width / 12 * GardenPlanSettings.TickFootWidth;
    }

    public void IncreaseLengthByPixels(double pixels)
    {
        Length += 12 * pixels / GardenPlanSettings.TickFootHeight;
        if (Length <= 0) Length = 1;
    }

    public void IncreaseWidthByPixels(double pixels)
    {
        Width += 12 * pixels / GardenPlanSettings.TickFootWidth;
        if (Width <= 0) Width = 1;
    }
}