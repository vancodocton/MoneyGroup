using System.Net;
using System.Net.Http.Json;
using System.Text.Json;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using MoneyGroup.Core.Models.Paginations;
using MoneyGroup.Core.Models.Users;
using MoneyGroup.FunctionalTests.Fixture;

namespace MoneyGroup.FunctionalTests.Endpoints;

public class UserEndpointsTest
    : IClassFixture<WebApiFactory>
{
    private static readonly JsonSerializerOptions JsonSerializerOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        PropertyNameCaseInsensitive = false,
        WriteIndented = true,
    };

    private readonly WebApiFactory _factory;
    private readonly HttpClient _client;

    public UserEndpointsTest(WebApiFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    #region GetUsers
    [Fact]
    public async Task GetUsers_ValidRequest_ReturnsPaginatedUsers()
    {
        // Arrange
        var page = 1;
        var size = 2;
        var request = $"/api/User?p={page}&s={size}";

        // Act
        var response = await _client.GetAsync(request, TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var paginationModel = await response.Content.ReadFromJsonAsync<PaginatedModel<UserDto>>(TestContext.Current.CancellationToken);
        Assert.NotNull(paginationModel);
        Assert.Equal(page, paginationModel.Page);
        Assert.Equal(size, paginationModel.Count);
        Assert.NotEmpty(paginationModel.Items);
        Assert.All(paginationModel.Items, user =>
        {
            Assert.NotNull(user);
            Assert.NotEqual(0, user.Id);
            Assert.NotNull(user.Name);
        });
    }

    [Fact]
    public async Task GetUsers_InvalidRequest_ReturnsProblemDetails()
    {
        // Arrange
        var page = 0; // Invalid page number
        var size = -1; // Invalid page size
        var request = $"/api/User?p={page}&s={size}";

        // Act
        var response = await _client.GetAsync(request, TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        var problemDetails = await response.Content.ReadFromJsonAsync<ValidationProblemDetails>(TestContext.Current.CancellationToken);
        Assert.NotNull(problemDetails);
        Assert.Contains("Size", problemDetails.Errors.Keys);
        Assert.Contains("Page", problemDetails.Errors.Keys);
    }
    #endregion GetUsers

    #region GetUserById
    [Fact]
    public async Task GetUserById_ValidId_ReturnsUser()
    {
        // Arrange
        var userId = 1; // Assuming a user with Id 1 exists
        var request = $"/api/User/{userId}";

        // Act
        var response = await _client.GetAsync(request, TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var user = await response.Content.ReadFromJsonAsync<UserDto>(TestContext.Current.CancellationToken);
        Assert.NotNull(user);
        Assert.Equal(userId, user.Id);
        Assert.NotNull(user.Name);
    }

    [Fact]
    public async Task GetUserById_InvalidId_ReturnsNotFound()
    {
        // Arrange
        var userId = int.MaxValue; // Assuming a user Id that does not exist
        var request = $"/api/User/{userId}";

        // Act
        var response = await _client.GetAsync(request, TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
    #endregion GetUserById

    #region GetExecutingUser
    [Fact]
    public async Task GetExecutingUser_Authenticated_ReturnsUser()
    {
        // Arrange
        var request = "/api/User/my";
        var user = _factory.CurrentUser;

        // Act
        var response = await _client.GetAsync(request, TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var res = await response.Content.ReadFromJsonAsync<UserDto>(TestContext.Current.CancellationToken);
        Assert.NotNull(res);
        Assert.Equal(user.Name, res.Name);
    }
    #endregion GetExecutingUser
}
