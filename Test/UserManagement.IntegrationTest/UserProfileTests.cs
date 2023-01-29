﻿using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using UserManagement.Contract.ViewModels;

namespace UserManagement.IntegrationTest;

public partial class GardenTests // : IClassFixture<UserManagementServiceFixture>
{
    public const string TEST_USER = "TestUser";
    public const string TEST_DELETE_USER = "TestDeleteUser";

    #region User Profile
    [Fact]
    public async Task Post_UserProfile_CreateNew_UserProfile()
    {
        var response = await _userProfileClient.CreateUserProfile(TEST_USER);

        var returnString = await response.Content.ReadAsStringAsync();

        _output.WriteLine($"Service to create user profile responded with {response.StatusCode} code and {returnString} message");

        if (response.StatusCode == System.Net.HttpStatusCode.OK)
        {
            Assert.NotEmpty(returnString);
            Assert.True(Guid.TryParse(returnString, out Guid userId));
            Assert.True(Guid.TryParse(returnString, out var harvestCycleId));
        }
    }

    [Fact]
    public async Task Post_UserProfile_ShouldNotCreateNewUserProfile_WithoutUserName()
    {
        var response = await _userProfileClient.CreateUserProfile(string.Empty);

        var returnString = await response.Content.ReadAsStringAsync();

        _output.WriteLine($"Service to create user profile responded with {response.StatusCode} code and {returnString} message");


        Assert.True(response.StatusCode == System.Net.HttpStatusCode.BadRequest);
        Assert.NotEmpty(returnString);
        Assert.Contains("'User Name' must not be empty.", returnString);
    }

    [Fact]
    public async Task Put_UserProfile_ShouldUpdateUserProfile()
    {
        var userProfile = await GetUserProfileToWorkWith(TEST_USER);

        userProfile.LastName = $"User{DateTime.Now.ToString()}";

        var response = await _userProfileClient.UpdateUserProfile(userProfile);

        var returnString = await response.Content.ReadAsStringAsync();

        _output.WriteLine($"Service to update user profile responded with {response.StatusCode} code and {returnString} message");

        Assert.True(response.StatusCode == System.Net.HttpStatusCode.OK);
        Assert.NotEmpty(returnString);
    }

    [Fact]
    public async Task Delete_UserProfile_ShouldDelete()
    {
        var userProfile = await GetUserProfileToWorkWith(TEST_DELETE_USER);
        if (userProfile == null )
        {
            await _userProfileClient.CreateUserProfile(TEST_DELETE_USER);
        }

        userProfile = await GetUserProfileToWorkWith(TEST_DELETE_USER);

        var response = await _userProfileClient.DeleteUserProfile(userProfile.UserProfileId);

        var returnString = await response.Content.ReadAsStringAsync();

        _output.WriteLine($"Service to delete user profile responded with {response.StatusCode} code and {returnString} message");

        Assert.True(response.StatusCode == System.Net.HttpStatusCode.OK);
        Assert.NotEmpty(returnString);
    }

    #endregion

    private async Task<UserProfileViewModel> GetUserProfileToWorkWith(string userName)
    {
        var response = await _userProfileClient.GetUserProfile(userName);

        var options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            Converters =
                {
                    new JsonStringEnumConverter(),
                },
        };

        var returnString = await response.Content.ReadAsStringAsync();

        _output.WriteLine($"Service to get user responded with {response.StatusCode} code and {returnString} message");

        UserProfileViewModel userProfile = null;
        if (response.StatusCode == System.Net.HttpStatusCode.OK)
        {
            userProfile = await response.Content.ReadFromJsonAsync<UserProfileViewModel>(options);
        }

        return userProfile;
    }
}
