namespace UserManagement.Api.Model;

public class Garden : BaseEntity, IAggregateRoot
{
    public string Name { get; private set; }
    public string City { get; private set; }
    public string StateCode { get; private set; }
    public decimal Latitude { get; private set; }
    public decimal Longitude { get; private set; }
    public string Notes { get; private set; }
    public string UserProfileId { get; private set; }
    public DateOnly LastFrostDate { get; private set; }
    public DateOnly FirstFrostDate { get; private set; }


    private readonly List<GardenBed> _gardenBeds = new();
    public IReadOnlyCollection<GardenBed> GardenBeds => _gardenBeds.AsReadOnly();

    public Garden() { }

    private Garden(string name, string city, string stateCode, decimal latitude, decimal longitude, string notes, string userProfileId, List<GardenBed> gardenBeds, DateOnly lastFrostDate, DateOnly firstFrostDate)
    {
        Name = name;
        City = city;
        StateCode = stateCode;
        Latitude = latitude;
        Longitude = longitude;
        Notes = notes;
        UserProfileId = userProfileId;
        _gardenBeds = gardenBeds;
        LastFrostDate = lastFrostDate;
        FirstFrostDate = firstFrostDate;
    }

    public static Garden Create(
        string gardenName,
        string city,
        string stateCode,
        decimal latitude,
        decimal longitude,
        string notes,
        string userProfileId,
        DateOnly lastFrostDate,
        DateOnly firstFrostDate
    )
    {
        var garden = new Garden()
        {
            Id = Guid.NewGuid().ToString(),
            UserProfileId = userProfileId,
            Name = gardenName,
            City = city,
            StateCode = stateCode,
            Latitude = latitude,
            Longitude = longitude,
            Notes = notes,
            LastFrostDate= lastFrostDate,
            FirstFrostDate= firstFrostDate
        };

        garden.DomainEvents.Add(
                  new GardenEvent(garden, UserProfileEventTriggerEnum.GardenCreated, new TriggerEntity(EntityTypeEnum.Garden, garden.Id)));

        return garden;
    }


    public void Update(
        string gardenName,
        string city,
        string stateCode,
        decimal latitude,
        decimal longitude,
        string notes,
        DateOnly lastFrostDate,
        DateOnly firstFrostDate
        )
    {
        this.Set<string>(() => this.Name, gardenName);
        this.Set<string>(() => this.City, city);
        this.Set<string>(() => this.StateCode, stateCode);
        this.Set<decimal>(() => this.Latitude, latitude);
        this.Set<decimal>(() => this.Longitude, longitude);
        this.Set<string>(() => this.Notes, notes);
        this.Set<DateOnly>(() => this.LastFrostDate, lastFrostDate);
        this.Set<DateOnly>(() => this.FirstFrostDate, firstFrostDate);

    }
    #region GardenBed
    public string AddGardenBed(CreateGardenBedCommand command)
    {
        command.GardenId = this.Id;
        var gardenBed = GardenBed.Create(command);

        this._gardenBeds.Add(gardenBed);

        this.DomainEvents.Add(
         new GardenEvent(this, UserProfileEventTriggerEnum.GardenBedCreated, new TriggerEntity(EntityTypeEnum.GardenBed, gardenBed.Id)));

        return gardenBed.Id;
    }

    public void UpdateGardenBed(UpdateGardenBedCommand command)
    {
        this.GardenBeds.First(i => i.Id == command.GardenBedId).Update(command, AddChildDomainEvent);
    }

    public void DeleteGardenBed(string id)
    {
        AddChildDomainEvent(UserProfileEventTriggerEnum.GardenBedDeleted, new TriggerEntity(EntityTypeEnum.GardenBed, id));

    }
    #endregion

    private void AddChildDomainEvent(UserProfileEventTriggerEnum trigger, TriggerEntity entity)
    {
        var newEvent = new GardenEvent(this, trigger, entity);

        if (!this.DomainEvents.Contains(newEvent))
            this.DomainEvents.Add(newEvent);
    }

    protected override void AddDomainEvent(string attributeName)
    {
        if (this.DomainEvents.Count == 0)
        {
            this.DomainEvents.Add(
                  new GardenEvent(this, UserProfileEventTriggerEnum.GardenUpdated, new TriggerEntity(EntityTypeEnum.Garden, this.Id)));
        }
    }
}
