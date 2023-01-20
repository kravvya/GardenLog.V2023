namespace UserManagement.Contract.Base;

public record GardenBedBase
{
    public string Name { get; set; } = string.Empty;
    public string Notes { get; set; }= string.Empty;
    public int? RowNumber { get; set; }
    public int Length { get; set; }
    public int Width { get; set; }
    public double X { get; set; }
    public double Y { get; set; }
    public string? BorderColor { get; set; }
    public GardenBedTypeEnum Type { get; set; }
}
