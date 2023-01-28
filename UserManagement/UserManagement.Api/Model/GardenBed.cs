

using UserManagement.Contract.Command;

namespace UserManagement.Api.Model;

public class GardenBed : BaseEntity, IEntity
{
    public string Name { get; private set; }
    public int? RowNumber { get; private set; }
    public int Length { get; private set; }
    public int Width { get; private set; }
    public double X { get; private set; }
    public double Y { get; private set; }
    public string? BorderColor { get; private set; }
    public string Notes { get; private set; }
    public GardenBedTypeEnum Type { get; private set; }
    public string UserProfileId { get; private set; }

    public GardenBed() { }

    public static GardenBed Create(CreateGardenBedCommand command, string userProfileId
    )
    {
        var gardenBed = new GardenBed()
        {
            Id = Guid.NewGuid().ToString(),
            Name = command.Name,
            RowNumber = command.RowNumber,
            Length = command.Length,
            Width = command.Width,
            X = command.X,
            Y = command.Y,
            BorderColor = command.BorderColor,
            Notes = command.Notes,
            Type = command.Type,
            UserProfileId = userProfileId
        };

        gardenBed.DomainEvents.Add(
                  new GardenChildEvent(UserProfileEventTriggerEnum.GardenBedCreated, new TriggerEntity(EntityTypeEnum.GardenBed, gardenBed.Id)));

        return gardenBed;
    }


    public void Update(UpdateGardenBedCommand command, Action<UserProfileEventTriggerEnum, TriggerEntity> addGardenEvent)
    {
        this.Set<string>(() => this.Name, command.Name);
        this.Set<int?>(() => this.RowNumber, command.RowNumber);
        this.Set<int>(() => this.Length, command.Length);
        this.Set<int>(() => this.Width, command.Width);
        this.Set<double>(() => this.X, command.X);
        this.Set<double>(() => this.Y, command.Y);
        this.Set<string?>(() => this.BorderColor, command.BorderColor);
        this.Set<string>(() => this.Notes, command.Notes);

        if (this.DomainEvents != null && this.DomainEvents.Count > 0)
        {
            this.DomainEvents.Clear();
            addGardenEvent(UserProfileEventTriggerEnum.GardenBedUpdated, new TriggerEntity(EntityTypeEnum.GardenBed, this.Id));
        }
    }

    protected override void AddDomainEvent(string attributeName)
    {
        if (this.DomainEvents.Count == 0)
        {
            this.DomainEvents.Add(
                  new GardenChildEvent(UserProfileEventTriggerEnum.GardenBedUpdated, new TriggerEntity(EntityTypeEnum.GardenBed, this.Id)));
        }
    }
}
