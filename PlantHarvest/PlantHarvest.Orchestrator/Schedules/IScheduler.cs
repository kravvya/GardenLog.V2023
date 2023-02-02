

using PlantHarvest.Contract.Commands;

namespace PlantHarvest.Orchestrator.Schedules;

public interface IScheduler
{
    public bool CanSchedule(PlantGrowInstructionViewModel growInstruction);

    public CreatePlantScheduleCommand Schedule(PlantGrowInstructionViewModel growInstruction, GardenViewModel garden, int? daysToMaturityMin, int? daysToMaturityMax);
}
