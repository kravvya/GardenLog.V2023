

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

        await IndoorSawTaskGenerator.Handle(HarvestHelper.GetPlantHarvestEvent(HarvestEventTriggerEnum.PlantAddedToHarvestCycle
                                                                            , Contract.Enum.PlantingMethodEnum.SeedIndoors
                                                                            , WorkLogReasonEnum.SowIndoors), new CancellationToken());

        _taskCommandHandlerMock.Verify(t => t.CreatePlantTask(It.Is<CreatePlantTaskCommand>(c => c.Type == WorkLogReasonEnum.SowIndoors)), Times.Once);
    }

    [Fact]
    public async Task TaskGenerator_Deletes_SowInside_Task_When_PlantHarvestCycle_Removed()
    {
  
        var IndoorSawTaskGenerator = new IndoorSawTaskGenerator(_taskCommandHandlerMock.Object, _taskQueryHandlerMock.Object);

        await IndoorSawTaskGenerator.Handle(HarvestHelper.GetPlantHarvestEvent(HarvestEventTriggerEnum.PlantHarvestCycleDeleted
                                                                            , Contract.Enum.PlantingMethodEnum.SeedIndoors
                                                                            , WorkLogReasonEnum.SowIndoors), new CancellationToken());

        _taskCommandHandlerMock.Verify(t => t.DeletePlantTask(It.Is<string>(c => c == HarvestHelper.PLANT_TASK_ID)), Times.Once);
    }

    [Fact]
    public async Task TaskGenerator_Complete_Task_When_WorkLog_Is_Created()
    {
        var IndoorSawTaskGenerator = new IndoorSawTaskGenerator(_taskCommandHandlerMock.Object, _taskQueryHandlerMock.Object);

        await IndoorSawTaskGenerator.Handle(HarvestHelper.GetWorkLogEvent(WorkLogEventTriggerEnum.WorkLogCreated
                                                                            , HarvestHelper.PLANT_HARVEST_CYCLE_ID
                                                                            , WorkLogReasonEnum.SowIndoors), new CancellationToken());

        _taskCommandHandlerMock.Verify(t => t.CompletePlantTask(It.Is<UpdatePlantTaskCommand>(c => c.PlantTaskId == HarvestHelper.PLANT_TASK_ID)), Times.Once);
    }
}
