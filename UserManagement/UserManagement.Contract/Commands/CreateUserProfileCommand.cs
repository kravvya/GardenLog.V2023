namespace UserManagement.Contract.Command;


public record CreateUserProfileCommand: UserProfileBase
{
}


public class CreateUserProfileCommandValidator : UserProfileValidator<CreateUserProfileCommand>
{
    public CreateUserProfileCommandValidator()
    {
    }
}
