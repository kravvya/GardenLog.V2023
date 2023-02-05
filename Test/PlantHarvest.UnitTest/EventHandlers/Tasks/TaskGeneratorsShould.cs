

using PlantHarvest.Domain.HarvestAggregate.Events;

namespace PlantHarvest.UnitTest.EventHandlers.Tasks;

public class TaskGeneratorsShould
{
    Mock<IPlantTaskCommandHandler> _taskCommandHandlerMock;
    Mock<IPlantTaskQueryHandler> _taskQueryHandlerMock;

    public TaskGeneratorsShould()
    {
        _taskCommandHandlerMock = new Mock<IPlantTaskCommandHandler>();
        _taskQueryHandlerMock = new Mock<IPlantTaskQueryHandler>();

        _taskQueryHandlerMock.SetupGet(x => x.SearchPlantTasks(It.IsAny<PlantTaskSearch>()).Result)
          .Returns((new List<PlantTaskViewModel>() { HarvestHelper.GetPlantTaskViewModel(HarvestHelper.PLANT_TASK_ID, HarvestHelper.PLANT_HARVEST_CYCLE_ID, WorkLogReasonEnum.SowIndoors) }).AsReadOnly());

    }

    [Fact]
    public async Task TaskGenerator_Creates_SowInside_Task_When_PlantHarvestCycle_Created()
    {
        var IndoorSawTaskGenerator = new IndoorSawTaskGenerator(_taskCommandHandlerMock.Object, _taskQueryHandlerMock.Object);

        var harvest = HarvestHelper.GetHarvestCycle();
        var plantHarvestId = harvest.AddPlantHarvestCycle(HarvestHelper.GetCommandToCreatePlantHarvestCycle(Contract.Enum.PlantingMethodEnum.SeedIndoors));
        var evt = harvest.DomainEvents.First(e => ((HarvestEvent)e).Trigger == HarvestEventTriggerEnum.PlantAddedToHarvestCycle);

        harvest.AddPlantSchedule(HarvestHelper.GetCommandToCreateSchedule(plantHarvestId, WorkLogReasonEnum.SowIndoors));

        await IndoorSawTaskGenerator.Handle((HarvestEvent)evt, new CancellationToken());

        _taskCommandHandlerMock.Verify(t => t.CreatePlantTask(It.Is<CreatePlantTaskCommand>(c => c.Type == WorkLogReasonEnum.SowIndoors)), Times.Once);
    }

    [Fact]
    public async Task TaskGenerator_Deletes_SowInside_Task_When_PlantHarvestCycle_Removed()
    {
  
        var IndoorSawTaskGenerator = new IndoorSawTaskGenerator(_taskCommandHandlerMock.Object, _taskQueryHandlerMock.Object);

        var harvest = HarvestHelper.GetHarvestCycle();
        var plantHarvestId = harvest.AddPlantHarvestCycle(HarvestHelper.GetCommandToCreatePlantHarvestCycle(Contract.Enum.PlantingMethodEnum.SeedIndoors));
        harvest.DeletePlantHarvestCycle(plantHarvestId);
        var evt = harvest.DomainEvents.First(e => ((HarvestEvent)e).Trigger == HarvestEventTriggerEnum.PlantHarvestCycleDeleted);

        await IndoorSawTaskGenerator.Handle((HarvestEvent)evt, new CancellationToken());

        _taskCommandHandlerMock.Verify(t => t.DeletePlantTask(It.Is<string>(c => c == HarvestHelper.PLANT_TASK_ID)), Times.Once);
    }

    [Fact]
    public async Task TaskGenerator_Complete_SowIndoor_Task_When_PlantHarvest_Is_Seeded()
    {
        var IndoorSawTaskGenerator = new IndoorSawTaskGenerator(_taskCommandHandlerMock.Object, _taskQueryHandlerMock.Object);

        //await IndoorSawTaskGenerator.Handle(HarvestHelper.GetWorkLogEvent(WorkLogEventTriggerEnum.WorkLogCreated
        //                                                                    , HarvestHelper.PLANT_HARVEST_CYCLE_ID
        //                                                                    , WorkLogReasonEnum.SowIndoors), new CancellationToken());

        var harvest = HarvestHelper.GetHarvestCycle();
        var plantHarvestId = harvest.AddPlantHarvestCycle(HarvestHelper.GetCommandToCreatePlantHarvestCycle(Contract.Enum.PlantingMethodEnum.SeedIndoors));
        harvest.UpdatePlantHarvestCycle(new UpdatePlantHarvestCycleCommand() { SeedingDateTime = DateTime.UtcNow, NumberOfSeeds=101, SeedVendorName="Good seeds", PlantHarvestCycleId = plantHarvestId });
        var evt = harvest.DomainEvents.First(e => ((HarvestEvent)e).Trigger == HarvestEventTriggerEnum.PlantHarvestCycleSeeded);

        await IndoorSawTaskGenerator.Handle((HarvestEvent)evt, new CancellationToken());


        _taskCommandHandlerMock.Verify(t => t.CompletePlantTask(It.Is<UpdatePlantTaskCommand>(c => c.PlantTaskId == HarvestHelper.PLANT_TASK_ID)), Times.Once);
    }
}
