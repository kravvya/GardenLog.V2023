namespace GardenLogWeb.Models;

public abstract class VisualComponent
{
    public int Length { get; set; }
    public int Width { get; set; }
    public double X { get; set; }
    public double Y { get; set; }
    public string? BorderColor { get; set; }
}
