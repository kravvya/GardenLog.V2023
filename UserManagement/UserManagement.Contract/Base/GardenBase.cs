namespace UserManagement.Contract.Base;

public abstract record GardenBase
{
    public string Name { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string StateCode { get; set; } = string.Empty;
    public decimal Latitude { get; set; }
    public decimal Longitude { get; set; }
    public string Notes { get; set; } = string.Empty;
    public string UserProfileId { get; set; } = string.Empty;
}
