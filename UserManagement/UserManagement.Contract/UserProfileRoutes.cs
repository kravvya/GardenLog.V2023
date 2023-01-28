namespace UserManagement.Contract;

public class UserProfileRoutes
{
    public const string UserProfileBase = "/v1/api/UserProfiles";
    public const string GetUserProfile = UserProfileBase + "/LoggedId";
    public const string GetUserProfileById = UserProfileBase + "/{userProfileId}";
    public const string SearchUserProfile = UserProfileBase + "/search";
    public const string CreateUserProfile = UserProfileBase;
    public const string UpdateUserProfile = UserProfileBase + "/{userProfileId}";
    public const string DeleteUserProfile = UserProfileBase + "/{userProfileId}";
}
