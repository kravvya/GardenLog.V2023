namespace UserManagement.Api.Model;

public interface IUserProfileRepository :  IRepository<UserProfile>
{
    Task<UserProfileViewModel> GetUserProfile(string userProfileId);
    Task<UserProfileViewModel> SearchForUserProfile(SearchUserProfiles searchCriteria);
}
