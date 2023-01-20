namespace UserManagement.Contract.Base;

public record GardenBase
{
    public string GardenName { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string StateCode { get; set; } = string.Empty;
    public decimal Latitude { get; set; }
    public decimal Longitude { get; set; }
    public string UserId { get; set; } = string.Empty;
}
