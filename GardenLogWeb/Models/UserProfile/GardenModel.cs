namespace GardenLogWeb.Models.UserProfile;

public record GardenModel: GardenViewModel
{
    public string ImageFileName { get; set; }
    public string ImageLabel { get; set; }
    public List<ImageViewModel> Images { get; set; }
}
