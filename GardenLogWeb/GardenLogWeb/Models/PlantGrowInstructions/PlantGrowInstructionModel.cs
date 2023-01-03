namespace GardenLogWeb.Models.PlantGrowInstructions;

public record PlantGrowInstructionModel : PlantGrowInstructionCommandBase
{
    public string PlantGrowInstructionId { get; set; }
}
