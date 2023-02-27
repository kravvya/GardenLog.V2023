using FluentValidation;
using UserManagement.Contract.Validators;

namespace GardenLogWeb.Models.UserProfile;

public record UserProfileModel : UserProfileViewModel
{
    public string  Password { get; set; } = string.Empty;
    public string PasswordConfirmation { get; set; } = string.Empty;
}

public class UserProfileModelValidator : UserProfileValidator<UserProfileModel>
{
    public UserProfileModelValidator()
    {
        RuleFor(x => x.Password).NotEmpty();

        RuleFor(customer => customer.Password).Equal(customer => customer.PasswordConfirmation).WithMessage("The password and confirmation password do not match");
    }
}
