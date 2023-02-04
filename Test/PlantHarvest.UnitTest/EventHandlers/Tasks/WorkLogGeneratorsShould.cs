using PlantHarvest.Api.EventHandlers.Tasks;

namespace PlantHarvest.UnitTest.EventHandlers.Tasks;

public class WorkLogGeneratorsShould
{
    Mock<IWorkLogCommandHandler> _workLogCommandHandlerMock;

    public WorkLogGeneratorsShould()
    {
        _workLogCommandHandlerMock =new Mock<IWorkLogCommandHandler>();
    }

    [Fact]
    public async Task WorkLogGenerator_Creates_WorkLog_When_PlantWorkLog_Sends_Seeded_Event()
    {
        var workLogGenerator = new WorkLogGenerator(_workLogCommandHandlerMock.Object);

        var evt = HarvestHelper.GetPlantHarvestEvent(HarvestEventTriggerEnum.PlantHarvestCycleSeeded,
                                Contract.Enum.PlantingMethodEnum.SeedIndoors,
                                WorkLogReasonEnum.SowIndoors);

        evt.Harvest.UpdatePlantHarvestCycle(new UpdatePlantHarvestCycleCommand() { SeedingDateTime = DateTime.UtcNow , PlantHarvestCycleId = evt.Harvest.Plants.First().Id});

        await workLogGenerator.Handle(evt, new CancellationToken());

        _workLogCommandHandlerMock.Verify(t => t.CreateWorkLog(It.Is<CreateWorkLogCommand>(c => c.Reason == WorkLogReasonEnum.SowIndoors)), Times.Once);
    }
}
