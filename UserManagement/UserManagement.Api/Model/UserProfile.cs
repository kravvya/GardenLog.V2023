

namespace UserManagement.Api.Model;

public class UserProfile :   BaseEntity, IAggregateRoot
{
    public string UserName { get; private set; }
    public string FirstName { get; private set; }
    public string LastName { get; private set; }
    public string EmailAddress { get; private set; }
   
    public DateTime UserProfileCreatedDateTimeUtc { get; private set; }

    private UserProfile() { }

    public static UserProfile Create(string userName, string firstName, string lastName, string emailAddress)
    {
        var user = new UserProfile()
        {
            Id = Guid.NewGuid().ToString(),
            UserName = userName,
            FirstName = firstName,
            LastName = lastName,
            EmailAddress = emailAddress,
            UserProfileCreatedDateTimeUtc = DateTime.UtcNow
        };

        user.DomainEvents.Add(
                  new UserProfileEvent(user, UserProfileEventTriggerEnum.UserProfileCreated, new UserManagment.Api.Model.Meta.TriggerEntity(EntityTypeEnum.UserProfile, user.Id)));

        return user;
    }

    public void Update(string userName, string firstName, string lastName, string emailAddress)
    {
        this.Set<string>(() => this.UserName, userName);

        this.Set<string>(() => this.FirstName, firstName);

        this.Set<string>(() => this.LastName, lastName);

        this.Set<string>(() => this.EmailAddress, emailAddress);

    }

    protected override void AddDomainEvent(string attributeName)
    {
        if (this.DomainEvents.Count == 0)
        {
            this.DomainEvents.Add(
                  new UserProfileEvent(this, UserProfileEventTriggerEnum.UserProfileUpdated, new UserManagment.Api.Model.Meta.TriggerEntity(EntityTypeEnum.UserProfile, this.Id)));
        }
    }
}

