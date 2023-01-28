using FluentValidation;
using GardenLog.SharedInfrastructure.Extensions;


namespace UserManagement.CommandHandlers
{
    public interface IUserProfileCommandHandler
    {
        Task<string> CreateUserProfile(CreateUserProfileCommand request);
        Task<int> DeleteUserProfile(string id);
        Task<int> UpdateUserProfile(UpdateUserProfileCommand request);
    }

    public class UserProfileCommandHandler : IUserProfileCommandHandler
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserProfileRepository _userProfileRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<UserProfileCommandHandler> _logger;

        public UserProfileCommandHandler(IUnitOfWork unitOfWork, IUserProfileRepository userProfileRepository, IHttpContextAccessor httpContextAccessor, ILogger<UserProfileCommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _userProfileRepository = userProfileRepository;
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
        }

        public async Task<string> CreateUserProfile(CreateUserProfileCommand request)
        {
            var oldUser = await _userProfileRepository.SearchForUserProfile(new SearchUserProfiles(request.UserName, request.EmailAddress));

            if (oldUser != null && oldUser.UserName.Equals(request.UserName))
                throw new ArgumentException("UserName already in use", nameof(request.UserName));

            if (oldUser != null && oldUser.EmailAddress.Equals(request.EmailAddress))
                throw new ArgumentException("This Email is already registered", nameof(request.UserName));


            var user = UserProfile.Create(request.UserName, request.FirstName, request.LastName, request.EmailAddress);

            _userProfileRepository.Add(user);

            await _unitOfWork.SaveChangesAsync();

            return user.Id;
        }

        public async Task<int> UpdateUserProfile(UpdateUserProfileCommand request)
        {
            request.UserProfileId = _httpContextAccessor.HttpContext.User.GetUserProfileId();


            var oldUser = await _userProfileRepository.SearchForUserProfile(new SearchUserProfiles(request.UserName, request.EmailAddress));

            if (oldUser != null && oldUser.UserName.Equals(request.UserName) && !oldUser.UserProfileId.Equals(request.UserProfileId))
                throw new ArgumentException("UserName already in use", nameof(request.UserName));

            if (oldUser != null && oldUser.EmailAddress.Equals(request.EmailAddress) && !oldUser.UserProfileId.Equals(request.UserProfileId))
                throw new ArgumentException("This Email is already registered", nameof(request.UserName));


            var user = await _userProfileRepository.GetByIdAsync(request.UserProfileId);
            if (user == null) return 0;

            user.Update(request.UserName, request.FirstName, request.LastName, request.EmailAddress);

            _userProfileRepository.Update(user);

            return await _unitOfWork.SaveChangesAsync();
        }

        public async Task<int> DeleteUserProfile(string id)
        {
            _userProfileRepository.Delete(id);

            return await _unitOfWork.SaveChangesAsync();
        }

    }
}
