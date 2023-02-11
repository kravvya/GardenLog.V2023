using GardenLogWeb.Pages.GardenLayout.Components;
using System.Reflection.Metadata.Ecma335;

namespace GardenLogWeb.Models.UserProfile;

public record GardenBedModel : GardenBedViewModel, IVisualComponent
{
    public string? CssClass { get => this.Type.ToString(); }
    public string Id { get => this.GardenBedId; }

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