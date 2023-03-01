namespace GardenLogWeb.Services;

public interface IUserProfileService
{
    Task<UserProfileModel> GetUserProfile( bool forceRefresh);
    Task<ApiObjectResponse<string>> CreateUserProfile(UserProfileModel workModel);
    Task<ApiResponse> UpdateUserProfile(UserProfileModel workModel);
}

public class UserProfileService : IUserProfileService
{
    private readonly ILogger<UserProfileService> _logger;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ICacheService _cacheService;
    private readonly IGardenLogToastService _toastService;
    private const int CACHE_DURATION = 10;
    private const string USER_KEY = "UserProfile";

    public UserProfileService(ILogger<UserProfileService> logger, IHttpClientFactory clientFactory, ICacheService cacheService, IGardenLogToastService toastService)
    {
        _logger = logger;
        _httpClientFactory = clientFactory;
        _cacheService = cacheService;
        _toastService = toastService;
    }

    #region Public User Functions

    public async Task<UserProfileModel> GetUserProfile(bool forceRefresh)
    {
        UserProfileModel user;

        if (forceRefresh || !_cacheService.TryGetValue<UserProfileModel>(USER_KEY, out user))
        {
            _logger.LogInformation("UserProfile not in cache or forceRefresh");

            user = await GetUserProfile();

            // Save data in cache.
            _cacheService.Set(USER_KEY, user, DateTime.Now.AddMinutes(CACHE_DURATION));
        }

        else
        {
            _logger.LogInformation($"UserProfile in cache.");
        }

        return user;
    }

    public async Task<ApiObjectResponse<string>> CreateUserProfile(UserProfileModel user)
    {
        var httpClient = _httpClientFactory.CreateClient(GlobalConstants.USERMANAGEMENT_CREATEONLY_API);

        var response = await httpClient.ApiPostAsync(UserProfileRoutes.CreateUserProfile, user);

        if (response.ValidationProblems != null)
        {
            _toastService.ShowToast($"Unable to create User Profile. Please resolve validatione errors and try again.", GardenLogToastLevel.Error);
        }
        else if (!response.IsSuccess)
        {
            _toastService.ShowToast($"Received an invalid response from User Profile post: {response.ErrorMessage}", GardenLogToastLevel.Error);
        }
        else
        {
            user.UserProfileId = response.Response;

            _cacheService.Set(USER_KEY, user, DateTime.Now.AddMinutes(CACHE_DURATION));

            _toastService.ShowToast($"User Profile saved", GardenLogToastLevel.Success);
        }

        return response;
    }

    public async Task<ApiResponse> UpdateUserProfile(UserProfileModel user)
    {
        var httpClient = _httpClientFactory.CreateClient(GlobalConstants.USERMANAGEMENT_API);

        var response = await httpClient.ApiPutAsync(UserProfileRoutes.UpdateUserProfile.Replace("{userProfileId}", user.UserProfileId), user);

        if (response.ValidationProblems != null)
        {
            _toastService.ShowToast($"Unable to update User Profile. Please resolve validatione errors and try again.", GardenLogToastLevel.Error);
        }
        else if (!response.IsSuccess)
        {
            _toastService.ShowToast($"Received an invalid response: {response.ErrorMessage}", GardenLogToastLevel.Error);
        }
        else
        {
            _cacheService.Set(USER_KEY, user, DateTime.Now.AddMinutes(CACHE_DURATION));

            _toastService.ShowToast($"User Profile successfully saved.", GardenLogToastLevel.Success);
        }

        return response;
    }

    #endregion


    #region Private User Profile Functions
    private async Task<UserProfileModel> GetUserProfile()
    {
        var httpClient = _httpClientFactory.CreateClient(GlobalConstants.USERMANAGEMENT_API);

        var url = UserProfileRoutes.GetUserProfile;

        var response = await httpClient.ApiGetAsync<UserProfileModel>(url);

        if (!response.IsSuccess)
        {
            _toastService.ShowToast("Unable to get User Profile", GardenLogToastLevel.Error);
            return new UserProfileModel();
        }

        return response.Response;
    }
    #endregion
}
