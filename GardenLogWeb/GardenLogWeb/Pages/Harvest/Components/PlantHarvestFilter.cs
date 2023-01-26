namespace GardenLogWeb.Pages.Harvest.Components;

public class PlantHarvestFilter 
{
    public event EventHandler<EventArgs> ModelChanged;

    public string? PlantId { get; set; }

    public bool IsStartIndoors { get; set; }
    public bool IsDirectSow { get; set; }
    public bool IsTransplant { get; set; }

    public void SetValue(String fieldName, object value)
    {
        var propertyInfo = this.GetType().GetProperty(fieldName);
        propertyInfo.SetValue(this, value);
        OnModelChanged();
    }
    protected void OnModelChanged()
    {
        ModelChanged?.Invoke(this, new EventArgs());
    }
}
