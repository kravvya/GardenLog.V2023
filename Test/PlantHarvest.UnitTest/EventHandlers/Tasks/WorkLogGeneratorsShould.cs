using GardenLog.SharedKernel;
using PlantHarvest.Api.EventHandlers.Tasks;
using PlantHarvest.Domain.HarvestAggregate.Events;

namespace PlantHarvest.UnitTest.EventHandlers.Tasks;

public class WorkLogGeneratorsShould
{
    Mock<IWorkLogCommandHandler> _workLogCommandHandlerMock;

    public WorkLogGeneratorsShould()
    {
        _workLogCommandHandlerMock = new Mock<IWorkLogCommandHandler>();
    }

    [Fact]
    public async Task WorkLogGenerator_Creates_WorkLog_When_PlantWorkLog_Sends_Seeded_Event()
    {
        var workLogGenerator = new WorkLogGenerator(_workLogCommandHandlerMock.Object);

        var harvest = HarvestHelper.GetHarvestCycle();
        var plantHarvestId = harvest.AddPlantHarvestCycle(HarvestHelper.GetCommandToCreatePlantHarvestCycle(Contract.Enum.PlantingMethodEnum.SeedIndoors));
        harvest.UpdatePlantHarvestCycle(new UpdatePlantHarvestCycleCommand() { SeedingDateTime = DateTime.UtcNow, PlantHarvestCycleId = plantHarvestId });
        var evt = harvest.DomainEvents.First(e => ((HarvestEvent)e).Trigger == HarvestEventTriggerEnum.PlantHarvestCycleSeeded);

        await workLogGenerator.Handle((HarvestEvent)evt, new CancellationToken());

        _workLogCommandHandlerMock.Verify(t => t.CreateWorkLog(It.Is<CreateWorkLogCommand>(c => c.Reason == WorkLogReasonEnum.SowIndoors)), Times.Once);
        _workLogCommandHandlerMock.Verify(t => t.CreateWorkLog(It.Is<CreateWorkLogCommand>(c => c.Log.Contains("Test Plant were planted indoors on "))), Times.Once);
    }

    [Fact]
    public async Task WorkLogGenerator_Creates_WorkLog_When_PlantWorkLog_Sends_Germinated_Event()
    {
        var workLogGenerator = new WorkLogGenerator(_workLogCommandHandlerMock.Object);

        var harvest = HarvestHelper.GetHarvestCycle();
        var plantHarvestId = harvest.AddPlantHarvestCycle(HarvestHelper.GetCommandToCreatePlantHarvestCycle(Contract.Enum.PlantingMethodEnum.SeedIndoors));
        harvest.UpdatePlantHarvestCycle(new UpdatePlantHarvestCycleCommand() { GerminationDate = DateTime.UtcNow, GerminationRate = 80, SeedVendorName = "Good seeds", PlantHarvestCycleId = plantHarvestId });

        var evt = harvest.DomainEvents.First(e => ((HarvestEvent)e).Trigger == HarvestEventTriggerEnum.PlantHarvestCycleGerminated);

        await workLogGenerator.Handle((HarvestEvent)evt, new CancellationToken());

        _workLogCommandHandlerMock.Verify(t => t.CreateWorkLog(It.Is<CreateWorkLogCommand>(c => c.Reason == WorkLogReasonEnum.Information)), Times.Once);
        _workLogCommandHandlerMock.Verify(t => t.CreateWorkLog(It.Is<CreateWorkLogCommand>(c => c.Log.Equals($"80% gemmanation of Test Plant  on {DateTime.Now.ToShortDateString()}  from Good seeds "))), Times.Once);
    }
}
