

namespace UserManagement.Contract.Command;

public record UpdateUserProfileCommand: UserProfileBase
{
    public string UserProfileId { get; set; }
}



public class UpdateUserProfileCommandValidator : UserProfileValidator<UpdateUserProfileCommand>
{
    public UpdateUserProfileCommandValidator()
    {
        RuleFor(command => command.UserProfileId).NotEmpty().Length(3, 50);
    }
}