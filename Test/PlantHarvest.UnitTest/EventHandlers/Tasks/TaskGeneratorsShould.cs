

using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using PlantHarvest.Domain.HarvestAggregate.Events;
using PlantHarvest.Domain.WorkLogAggregate.Events;
using PlantHarvest.Infrastructure.ApiClients;

namespace PlantHarvest.UnitTest.EventHandlers.Tasks;

public class TaskGeneratorsShould
{
    Mock<IPlantTaskCommandHandler> _taskCommandHandlerMock;
    Mock<IPlantTaskQueryHandler> _taskQueryHandlerMock;

    IPlantCatalogApiClient _plantCatalogApiClient;
    Mock<IHarvestQueryHandler> _harvestQueryHandlerMock;

    public TaskGeneratorsShould()
    {
        _taskCommandHandlerMock = new Mock<IPlantTaskCommandHandler>();
        _taskQueryHandlerMock = new Mock<IPlantTaskQueryHandler>();
        _harvestQueryHandlerMock = new Mock<IHarvestQueryHandler>();

        _taskQueryHandlerMock.SetupGet(x => x.SearchPlantTasks(It.Is<PlantTaskSearch>(s => s.Reason == WorkLogReasonEnum.SowIndoors)).Result)
          .Returns((new List<PlantTaskViewModel>()
            {
              HarvestHelper.GetPlantTaskViewModel(HarvestHelper.PLANT_TASK_ID, HarvestHelper.PLANT_HARVEST_CYCLE_ID, WorkLogReasonEnum.SowIndoors)
            }).AsReadOnly());

        _taskQueryHandlerMock.SetupGet(x => x.SearchPlantTasks(It.Is<PlantTaskSearch>(s => s.Reason == WorkLogReasonEnum.FertilizeIndoors)).Result)
         .Returns((new List<PlantTaskViewModel>()
           {
              HarvestHelper.GetPlantTaskViewModel(HarvestHelper.PLANT_TASK_ID, HarvestHelper.PLANT_HARVEST_CYCLE_ID, WorkLogReasonEnum.FertilizeIndoors)
           }).AsReadOnly());

        _taskQueryHandlerMock.SetupGet(x => x.SearchPlantTasks(It.Is<PlantTaskSearch>(s => s.Reason == WorkLogReasonEnum.TransplantOutside)).Result)
        .Returns((new List<PlantTaskViewModel>()
          {
              HarvestHelper.GetPlantTaskViewModel(HarvestHelper.PLANT_TASK_ID, HarvestHelper.PLANT_HARVEST_CYCLE_ID, WorkLogReasonEnum.TransplantOutside)
          }).AsReadOnly());

        _taskQueryHandlerMock.SetupGet(x => x.SearchPlantTasks(It.Is<PlantTaskSearch>(s => s.Reason == WorkLogReasonEnum.Harden)).Result)
        .Returns((new List<PlantTaskViewModel>()
          {
              HarvestHelper.GetPlantTaskViewModel(HarvestHelper.PLANT_TASK_ID, HarvestHelper.PLANT_HARVEST_CYCLE_ID, WorkLogReasonEnum.Harden)
          }).AsReadOnly());

        var httpPlantCatalogClient = HttpClientsHelper.GetPlantCatalogHttpClientForGrowInstructions(PlantCatalog.Contract.Enum.PlantingMethodEnum.SeedIndoors);

        var configMock = new Mock<IConfiguration>();
        configMock.SetupGet(x => x[It.Is<string>(s => s == "Services:PlantCatalog.Api")]).Returns(HttpClientsHelper.PLANT_CATALOG_URL);
        IConfiguration configuration = configMock.Object;

        ILogger<PlantCatalogApiClient> loggerPlantCatalogClient = new Mock<ILogger<PlantCatalogApiClient>>().Object;

        IMemoryCache memoryCache = new MemoryCache(new MemoryCacheOptions());

        _plantCatalogApiClient = new PlantCatalogApiClient(httpPlantCatalogClient, configuration, loggerPlantCatalogClient, memoryCache);


    }

    [Fact]
    public async Task TaskGenerator_Creates_SowInside_Task_When_PlantHarvest_Created()
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
    public async Task TaskGenerator_Deletes_SowInside_Task_When_PlantHarvest_Removed()
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
        harvest.UpdatePlantHarvestCycle(new UpdatePlantHarvestCycleCommand() { SeedingDateTime = DateTime.UtcNow, NumberOfSeeds = 101, SeedVendorName = "Good seeds", PlantHarvestCycleId = plantHarvestId });
        var evt = harvest.DomainEvents.First(e => ((HarvestEvent)e).Trigger == HarvestEventTriggerEnum.PlantHarvestCycleSeeded);

        await IndoorSawTaskGenerator.Handle((HarvestEvent)evt, new CancellationToken());


        _taskCommandHandlerMock.Verify(t => t.CompletePlantTask(It.Is<UpdatePlantTaskCommand>(c => c.PlantTaskId == HarvestHelper.PLANT_TASK_ID)), Times.Once);
    }

    #region "Indoor Fertilize"

    [Fact]
    public async Task TaskGenerator_Creates_Fertilize_Task_When_PlantHarvest_Germinated()
    {
        var IndoorSawTaskGenerator = new IndoorFertilizeTaskGenerator(_taskCommandHandlerMock.Object, _taskQueryHandlerMock.Object, _plantCatalogApiClient, _harvestQueryHandlerMock.Object, new Mock<ILogger<IndoorFertilizeTaskGenerator>>().Object);


        var harvest = HarvestHelper.GetHarvestCycle();
        var plantHarvestId = harvest.AddPlantHarvestCycle(HarvestHelper.GetCommandToCreatePlantHarvestCycle(Contract.Enum.PlantingMethodEnum.SeedIndoors));
        harvest.UpdatePlantHarvestCycle(new UpdatePlantHarvestCycleCommand() { GerminationDate = DateTime.UtcNow, GerminationRate = 80, PlantingMethod = Contract.Enum.PlantingMethodEnum.SeedIndoors, SeedVendorName = "Good seeds", PlantHarvestCycleId = plantHarvestId });
        var evt = harvest.DomainEvents.First(e => ((HarvestEvent)e).Trigger == HarvestEventTriggerEnum.PlantHarvestCycleGerminated);

        await IndoorSawTaskGenerator.Handle((HarvestEvent)evt, new CancellationToken());


        _taskCommandHandlerMock.Verify(t => t.CreatePlantTask(It.Is<CreatePlantTaskCommand>(c => c.Type == WorkLogReasonEnum.FertilizeIndoors)), Times.Once);
    }

    [Fact]
    public async Task TaskGenerator_Deletes_Fertilize_Task_When_PlantHarvest_Removed()
    {
        var IndoorFetilizeTaskGenerator = new IndoorFertilizeTaskGenerator(_taskCommandHandlerMock.Object, _taskQueryHandlerMock.Object, _plantCatalogApiClient, _harvestQueryHandlerMock.Object, new Mock<ILogger<IndoorFertilizeTaskGenerator>>().Object);

        var harvest = HarvestHelper.GetHarvestCycle();
        var plantHarvestId = harvest.AddPlantHarvestCycle(HarvestHelper.GetCommandToCreatePlantHarvestCycle(Contract.Enum.PlantingMethodEnum.SeedIndoors));
        harvest.DeletePlantHarvestCycle(plantHarvestId);
        var evt = harvest.DomainEvents.First(e => ((HarvestEvent)e).Trigger == HarvestEventTriggerEnum.PlantHarvestCycleDeleted);

        await IndoorFetilizeTaskGenerator.Handle((HarvestEvent)evt, new CancellationToken());

        _taskCommandHandlerMock.Verify(t => t.DeletePlantTask(It.Is<string>(c => c == HarvestHelper.PLANT_TASK_ID)), Times.Once);
    }

    [Fact]
    public async Task TaskGenerator_Deletes_Fertilize_Task_When_PlantHarvest_Transplanted()
    {
        var IndoorFetilizeTaskGenerator = new IndoorFertilizeTaskGenerator(_taskCommandHandlerMock.Object, _taskQueryHandlerMock.Object, _plantCatalogApiClient, _harvestQueryHandlerMock.Object, new Mock<ILogger<IndoorFertilizeTaskGenerator>>().Object);

        var harvest = HarvestHelper.GetHarvestCycle();
        var plantHarvestId = harvest.AddPlantHarvestCycle(HarvestHelper.GetCommandToCreatePlantHarvestCycle(Contract.Enum.PlantingMethodEnum.SeedIndoors));
        harvest.UpdatePlantHarvestCycle(new UpdatePlantHarvestCycleCommand() { TransplantDate = DateTime.UtcNow, NumberOfSeeds = 101, SeedVendorName = "Good seeds", PlantHarvestCycleId = plantHarvestId });

        var evt = harvest.DomainEvents.First(e => ((HarvestEvent)e).Trigger == HarvestEventTriggerEnum.PlantHarvestCycleTransplanted);

        await IndoorFetilizeTaskGenerator.Handle((HarvestEvent)evt, new CancellationToken());

        _taskCommandHandlerMock.Verify(t => t.DeletePlantTask(It.Is<string>(c => c == HarvestHelper.PLANT_TASK_ID)), Times.Once);
    }

    [Fact]
    public async Task TaskGenerator_Complete_Fertilize_Task_When_WorkLog_FertilizeIndoors_CreateNewFertilizeTask()
    {
        var schedule = HarvestHelper.GetPlantScheduleViewModel(HarvestHelper.PLANT_HARVEST_CYCLE_ID, WorkLogReasonEnum.TransplantOutside, DateTime.Now.AddDays(90));
        var plantHarvest = HarvestHelper.GetPlantHarvestCycleViewModel(Contract.Enum.PlantingMethodEnum.SeedIndoors, schedule );
        plantHarvest.GerminationDate= DateTime.Now;

        _harvestQueryHandlerMock.SetupGet(x => x.GetPlantHarvestCycle(It.IsAny<string>(), It.IsAny<string>()).Result).Returns(plantHarvest);

        var IndoorFetilizeTaskGenerator = new IndoorFertilizeTaskGenerator(_taskCommandHandlerMock.Object, _taskQueryHandlerMock.Object, _plantCatalogApiClient, _harvestQueryHandlerMock.Object, new Mock<ILogger<IndoorFertilizeTaskGenerator>>().Object);

        var workLog = HarvestHelper.GetWorkLog(HarvestHelper.HARVEST_CYCLE_ID, "Test Harvest", HarvestHelper.PLANT_HARVEST_CYCLE_ID, "Test Plant", WorkLogReasonEnum.FertilizeIndoors);

        var evt = workLog.DomainEvents.First(e => ((WorkLogEvent)e).Work.Reason == WorkLogReasonEnum.FertilizeIndoors);

        await IndoorFetilizeTaskGenerator.Handle((WorkLogEvent)evt, new CancellationToken());

        _taskCommandHandlerMock.Verify(t => t.CompletePlantTask(It.Is<UpdatePlantTaskCommand>(c => c.PlantTaskId == HarvestHelper.PLANT_TASK_ID)), Times.Once);
        _taskCommandHandlerMock.Verify(t => t.CreatePlantTask(It.Is<CreatePlantTaskCommand>(c => c.Type == WorkLogReasonEnum.FertilizeIndoors)), Times.Once);
    }

    [Fact]
    public async Task TaskGenerator_Complete_Fertilize_Task_When_WorkLog_FertilizeIndoors_DoNotCreateNewFertilizeTask()
    {
        var schedule = HarvestHelper.GetPlantScheduleViewModel(HarvestHelper.PLANT_HARVEST_CYCLE_ID, WorkLogReasonEnum.TransplantOutside, DateTime.Now.AddDays(1));
        var plantHarvest = HarvestHelper.GetPlantHarvestCycleViewModel(Contract.Enum.PlantingMethodEnum.SeedIndoors, schedule);
        plantHarvest.GerminationDate = DateTime.Now;

        _harvestQueryHandlerMock.SetupGet(x => x.GetPlantHarvestCycle(It.IsAny<string>(), It.IsAny<string>()).Result).Returns(plantHarvest);

        var IndoorFetilizeTaskGenerator = new IndoorFertilizeTaskGenerator(_taskCommandHandlerMock.Object, _taskQueryHandlerMock.Object, _plantCatalogApiClient, _harvestQueryHandlerMock.Object, new Mock<ILogger<IndoorFertilizeTaskGenerator>>().Object);

        var workLog = HarvestHelper.GetWorkLog(HarvestHelper.HARVEST_CYCLE_ID, "Test Harvest", HarvestHelper.PLANT_HARVEST_CYCLE_ID, "Test Plant", WorkLogReasonEnum.FertilizeIndoors);

        var evt = workLog.DomainEvents.First(e => ((WorkLogEvent)e).Work.Reason == WorkLogReasonEnum.FertilizeIndoors);

        await IndoorFetilizeTaskGenerator.Handle((WorkLogEvent)evt, new CancellationToken());

        _taskCommandHandlerMock.Verify(t => t.CompletePlantTask(It.Is<UpdatePlantTaskCommand>(c => c.PlantTaskId == HarvestHelper.PLANT_TASK_ID)), Times.Once);
        _taskCommandHandlerMock.Verify(t => t.CreatePlantTask(It.Is<CreatePlantTaskCommand>(c => c.Type == WorkLogReasonEnum.FertilizeIndoors)), Times.Never);

    }

    #endregion

    #region "Transplant Outside"
    [Fact]
    public async Task TaskGenerator_Creates_TransplantOutside_Task_When_PlantHarvest_Seeded()
    {
        var tranplantOutsideTaskGenerator = new TransplantOutsideTaskGenerator(_taskCommandHandlerMock.Object, _taskQueryHandlerMock.Object);


        var harvest = HarvestHelper.GetHarvestCycle();
        var plantHarvestId = harvest.AddPlantHarvestCycle(HarvestHelper.GetCommandToCreatePlantHarvestCycle(Contract.Enum.PlantingMethodEnum.SeedIndoors));
        harvest.UpdatePlantHarvestCycle(new UpdatePlantHarvestCycleCommand() { SeedingDateTime = DateTime.Now, NumberOfSeeds=100, PlantingMethod = Contract.Enum.PlantingMethodEnum.SeedIndoors, SeedVendorName = "Good seeds", PlantHarvestCycleId = plantHarvestId });
        var evt = harvest.DomainEvents.First(e => ((HarvestEvent)e).Trigger == HarvestEventTriggerEnum.PlantHarvestCycleSeeded);

        harvest.AddPlantSchedule(HarvestHelper.GetCommandToCreateSchedule(plantHarvestId, WorkLogReasonEnum.TransplantOutside));

        await tranplantOutsideTaskGenerator.Handle((HarvestEvent)evt, new CancellationToken());

        _taskCommandHandlerMock.Verify(t => t.CreatePlantTask(It.Is<CreatePlantTaskCommand>(c => c.Type == WorkLogReasonEnum.TransplantOutside)), Times.Once);
        _taskCommandHandlerMock.Verify(t => t.CreatePlantTask(It.Is<CreatePlantTaskCommand>(c => c.Type == WorkLogReasonEnum.Harden)), Times.Once);
      
    }

    [Fact]
    public async Task TaskGenerator_Complete_TransplantOutside_Task_When_PlantHarvest_Is_Transplanted()
    {
        var tranplantOutsideTaskGenerator = new TransplantOutsideTaskGenerator(_taskCommandHandlerMock.Object, _taskQueryHandlerMock.Object);

        //await IndoorSawTaskGenerator.Handle(HarvestHelper.GetWorkLogEvent(WorkLogEventTriggerEnum.WorkLogCreated
        //                                                                    , HarvestHelper.PLANT_HARVEST_CYCLE_ID
        //                                                                    , WorkLogReasonEnum.SowIndoors), new CancellationToken());

        var harvest = HarvestHelper.GetHarvestCycle();
        var plantHarvestId = harvest.AddPlantHarvestCycle(HarvestHelper.GetCommandToCreatePlantHarvestCycle(Contract.Enum.PlantingMethodEnum.SeedIndoors));
        harvest.UpdatePlantHarvestCycle(new UpdatePlantHarvestCycleCommand() { TransplantDate = DateTime.UtcNow, NumberOfTransplants=50, SeedVendorName = "Good seeds", PlantHarvestCycleId = plantHarvestId });
        var evt = harvest.DomainEvents.First(e => ((HarvestEvent)e).Trigger == HarvestEventTriggerEnum.PlantHarvestCycleTransplanted);

        await tranplantOutsideTaskGenerator.Handle((HarvestEvent)evt, new CancellationToken());


        _taskCommandHandlerMock.Verify(t => t.CompletePlantTask(It.Is<UpdatePlantTaskCommand>(c => c.PlantTaskId == HarvestHelper.PLANT_TASK_ID)), Times.Exactly(2));
    }

    [Fact]
    public async Task TaskGenerator_Deletes_TransplantOutside_Task_When_PlantHarvest_Removed()
    {
        var tranplantOutsideTaskGenerator = new TransplantOutsideTaskGenerator(_taskCommandHandlerMock.Object, _taskQueryHandlerMock.Object);

        var harvest = HarvestHelper.GetHarvestCycle();
        var plantHarvestId = harvest.AddPlantHarvestCycle(HarvestHelper.GetCommandToCreatePlantHarvestCycle(Contract.Enum.PlantingMethodEnum.SeedIndoors));
        harvest.DeletePlantHarvestCycle(plantHarvestId);
        var evt = harvest.DomainEvents.First(e => ((HarvestEvent)e).Trigger == HarvestEventTriggerEnum.PlantHarvestCycleDeleted);

        await tranplantOutsideTaskGenerator.Handle((HarvestEvent)evt, new CancellationToken());

        _taskCommandHandlerMock.Verify(t => t.DeletePlantTask(It.Is<string>(c => c == HarvestHelper.PLANT_TASK_ID)), Times.Exactly(2));
    }
    #endregion
}
