using PlantHarvest.Contract.Commands;

namespace PlantHarvest.Domain.HarvestAggregate
{
    public class HarvestCycle : BaseEntity, IAggregateRoot
    {
        public string HarvestCycleName { get; private set; }
        public DateTime StartDate { get; private set; }
        public DateTime? EndDate { get; private set; }
        public string Notes { get; private set; }
        public string UserProfileId { get; private set; }
        public string GardenId { get; private set; }

        private readonly List<PlantHarvestCycle> _plants = new();
        public IReadOnlyCollection<PlantHarvestCycle> Plants => _plants.AsReadOnly();

        private readonly List<PlanHarvestCycle> _plans = new();
        public IReadOnlyCollection<PlanHarvestCycle> Plans => _plans.AsReadOnly();

        private HarvestCycle()
        {

        }

        private HarvestCycle(
            string harvestCycleName,
            DateTime startDate,
            DateTime? endDate,
            string notes,
            string userProfileId,
            string gardenId,
            List<PlantHarvestCycle> plants,
            List<PlanHarvestCycle> plans
           )
        {
            this.UserProfileId = userProfileId;
            this.HarvestCycleName = harvestCycleName;
            this.StartDate = startDate;
            this.EndDate = endDate;
            this.Notes = notes;
            this.GardenId= gardenId;
            this._plants = plants;
            this._plans = plans;
        }

        public static HarvestCycle Create(
            string userProfileId,
            string name,
            DateTime startDate,
            DateTime? endDate,
            string notes,
            string gardenId)
        {
            var harvest = new HarvestCycle()
            {
                Id = Guid.NewGuid().ToString(),
                UserProfileId = userProfileId,
                HarvestCycleName = name,
                StartDate = startDate,
                EndDate = endDate,
                Notes = notes,
                GardenId=gardenId
            };

            harvest.DomainEvents.Add(
            new HarvestEvent(harvest, HarvestEventTriggerEnum.HarvestCycleCreated, new TriggerEntity(EntityTypeEnum.HarvestCyce, harvest.Id)));

            return harvest;

        }


        public void Update(
            string name,
            DateTime startDate,
            DateTime? endDate,
            string notes,
            string gardenId
            )
        {
            this.Set<string>(() => this.HarvestCycleName, name);
            this.Set<DateTime>(() => this.StartDate, startDate);
            this.Set<DateTime?>(() => this.EndDate, endDate);
            this.Set<string>(() => this.Notes, notes);
            this.Set<string>(() => this.GardenId, gardenId);
        }

        #region Events
        protected override void AddDomainEvent(string attributeName)
        {
            this.DomainEvents.Add(
              new HarvestEvent(this, HarvestEventTriggerEnum.HarvestCycleUpdated, new TriggerEntity(EntityTypeEnum.HarvestCyce, this.Id)));
        }

        private void AddChildDomainEvent(HarvestEventTriggerEnum trigger, TriggerEntity entity)
        {
            var newEvent = new HarvestEvent(this, trigger, entity);

            if (!this.DomainEvents.Contains(newEvent))
                this.DomainEvents.Add(newEvent);
        }
        #endregion

        #region Plants
        public string AddPlantHarvestCycle(CreatePlantHarvestCycleCommand command, string userProfileId)
        {
            command.HarvestCycleId = this.Id;
            var plant = PlantHarvestCycle.Create(command, userProfileId);

            this._plants.Add(plant);

            this.DomainEvents.Add(
             new HarvestEvent(this, HarvestEventTriggerEnum.PlantAddedToHarvestCycle, new TriggerEntity(EntityTypeEnum.PlantHarvestCycle, plant.Id)));

            return plant.Id;
        }

        public void UpdatePlantHarvestCycle(UpdatePlantHarvestCycleCommand command)
        {
            this.Plants.First(i => i.Id == command.PlantHarvestCycleId).Update(command, AddChildDomainEvent);
        }

        public void DeletePlantHarvestCycle(string id)
        {
            AddChildDomainEvent(HarvestEventTriggerEnum.PlantHarvestCycleDeleted, new TriggerEntity(EntityTypeEnum.PlantHarvestCycle, id));

        }
        #endregion

        #region Plans
        public string AddPlanHarvestCycle(CreatePlanHarvestCycleCommand command, string userProfileId)
        {
            command.HarvestCycleId = this.Id;
            var plan = PlanHarvestCycle.Create(command, userProfileId);

            this._plans.Add(plan);

            this.DomainEvents.Add(
             new HarvestEvent(this, HarvestEventTriggerEnum.PlanAddedToHarvestCycle, new TriggerEntity(EntityTypeEnum.PlanHarvestCycle, plan.Id)));

            return plan.Id;
        }

        public void UpdatePlanHarvestCycle(UpdatePlanHarvestCycleCommand command)
        {
            this.Plans.First(i => i.Id == command.PlanHarvestCycleId).Update(command, AddChildDomainEvent);
        }

        public void DeletePlanHarvestCycle(string id)
        {
            AddChildDomainEvent(HarvestEventTriggerEnum.PlanHarvestCycleDeleted, new TriggerEntity(EntityTypeEnum.PlanHarvestCycle, id));

        }
        #endregion
    }
}
