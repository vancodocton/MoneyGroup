using System.Net;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using MoneyGroup.Core.Models.Orders;
using MoneyGroup.Core.Models.Paginations;
using MoneyGroup.FunctionalTests.Fixture;

namespace MoneyGroup.FunctionalTests.Endpoints;

public class OrderEndpointsTest
    : IClassFixture<WebApiFactory>
{
    private static readonly JsonSerializerOptions JsonSerializerOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        PropertyNameCaseInsensitive = false,
        WriteIndented = true,
    };

    private readonly HttpClient _client;

    public OrderEndpointsTest(WebApiFactory factory)
    {
        _client = factory.CreateClient();
    }

    #region GetOrders
    [Fact]
    public async Task GetOrders_ValidRequest_ReturnsPaginatedOrders()
    {
        // Arrange
        var page = 1;
        var size = 2;
        var request = $"/api/Order?p={page}&s={size}";

        // Act
        var response = await _client.GetAsync(request, TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var paginationModel = await response.Content.ReadFromJsonAsync<PaginatedModel<OrderDetailedDto>>(TestContext.Current.CancellationToken);
        Assert.NotNull(paginationModel);
        Assert.Equal(page, paginationModel.Page);
        Assert.Equal(size, paginationModel.Count);
        Assert.NotEmpty(paginationModel.Items);
        Assert.All(paginationModel.Items, order =>
        {
            Assert.NotNull(order);
            Assert.NotEqual(0, order.Id);
            Assert.NotNull(order.Title);
            Assert.NotEqual(0, order.Total);
            Assert.NotEqual(0, order.BuyerId);
            Assert.NotNull(order.BuyerName);
            Assert.NotEmpty(order.Participants);
            Assert.All(order.Participants, p =>
            {
                Assert.NotNull(p);
                Assert.NotEqual(0, p.ParticipantId);
                Assert.NotNull(p.ParticipantName);
            });
        });
    }

    [Fact]
    public async Task GetOrders_InvalidRequest_ReturnsProblemDetails()
    {
        // Arrange
        var page = 0; // Invalid page number
        var size = -1; // Invalid page size
        var buyerId = -1; // Invalid buyer Id
        var participantId = -1; // Invalid participant Id
        var request = $"/api/Order?p={page}&s={size}&buyerId={buyerId}&participantId={participantId}";

        // Act
        var response = await _client.GetAsync(request, TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        var problemDetails = await response.Content.ReadFromJsonAsync<ValidationProblemDetails>(TestContext.Current.CancellationToken);
        Assert.NotNull(problemDetails);
        Assert.Equal(4, problemDetails.Errors.Count);
        Assert.Contains("Size", problemDetails.Errors.Keys);
        Assert.Contains("Page", problemDetails.Errors.Keys);
        Assert.Contains("BuyerId", problemDetails.Errors.Keys);
        Assert.Contains("ParticipantId", problemDetails.Errors.Keys);
    }
    #endregion GetOrders

    #region GetOrderById
    [Fact]
    public async Task GetOrderById_ValidId_ShouldReturnsOrder()
    {
        // Arrange
        var orderId = 1; // Assuming an order with Id 1 exists
        var request = $"/api/Order/{orderId}";

        // Act
        var response = await _client.GetAsync(request, TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var order = await response.Content.ReadFromJsonAsync<OrderDetailedDto>(TestContext.Current.CancellationToken);
        Assert.NotNull(order);
        Assert.Equal(orderId, order.Id);
        Assert.NotNull(order.Title);
        Assert.NotEqual(0, order.Total);
        Assert.NotEqual(0, order.BuyerId);
        Assert.NotNull(order.BuyerName);
        Assert.NotEmpty(order.Participants);
        Assert.All(order.Participants, p =>
        {
            Assert.NotNull(p);
            Assert.NotEqual(0, p.ParticipantId);
            Assert.NotNull(p.ParticipantName);
        });
    }

    [Fact]
    public async Task GetOrderById_InvalidId_ShouldReturnsOrder()
    {
        // Arrange
        var orderId = "string"; // orderId is not an integer
        var request = $"/api/Order/{orderId}";

        // Act
        var response = await _client.GetAsync(request, TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task GetOrderById_NonExistedId_ShouldReturnsOrder()
    {
        // Arrange
        var orderId = int.MaxValue; // Assuming an order Id that does not exist
        var request = $"/api/Order/{orderId}";

        // Act
        var response = await _client.GetAsync(request, TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
    #endregion GetOrderById

    #region CreateOrder
    [Fact]
    public async Task CreateOrder_ValidDto_ReturnsCreatedOrder()
    {
        // Arrange
        var request = "/api/Order";
        var newOrder = new
        {
            // Populate with necessary order properties
            Title = "New order",
            Description = "New order description",
            Total = 10_000,
            BuyerId = 1,
            Participants = new List<object>()
            {
                new { Id = 1 },
                new { Id = 2 },
            },
        };
        var content = new StringContent(JsonSerializer.Serialize(newOrder, JsonSerializerOptions), Encoding.UTF8, "application/json");

        // Act
        var response = await _client.PostAsync(request, content, TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var order = await response.Content.ReadFromJsonAsync<OrderDto>(TestContext.Current.CancellationToken);

        Assert.NotNull(order);
        Assert.NotEqual(0, order.Id);
        Assert.Equal("New order", order.Title);
        Assert.Equal("New order description", order.Description);
        Assert.Equal(10_000, order.Total);
        Assert.Equal(1, order.BuyerId);
        Assert.Equal(1, order.Participants.Skip(0).First().ParticipantId);
        Assert.Equal(2, order.Participants.Skip(1).First().ParticipantId);

        Assert.Equal($"/api/Order/{order.Id}", response.Headers.Location?.PathAndQuery);
    }

    [Fact]
    public async Task CreateOrder_InvalidDto_ReturnsProblemDetails()
    {
        // Arrange
        var request = "/api/Order";
        var newOrder = new
        {
        };
        var content = new StringContent(JsonSerializer.Serialize(newOrder, JsonSerializerOptions), Encoding.UTF8, "application/json");

        // Act
        var response = await _client.PostAsync(request, content, TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        var problemDetails = await response.Content.ReadFromJsonAsync<ValidationProblemDetails>(TestContext.Current.CancellationToken);

        Assert.NotNull(problemDetails);
        Assert.NotEmpty(problemDetails.Errors);
        Assert.Contains("Title", problemDetails.Errors.Keys);
        Assert.Contains("BuyerId", problemDetails.Errors.Keys);
        Assert.Contains("Participants", problemDetails.Errors.Keys);
    }

    [Fact]
    public async Task CreateOrder_NonExistedBuyerId_ReturnsProblemDetails()
    {
        // Arrange
        var request = "/api/Order";
        var newOrder = new
        {
            Title = "New order",
            Description = "New order description",
            Total = 10_000,
            BuyerId = int.MaxValue, // Assuming an Buyer Id that does not exist
            Participants = new List<object>()
            {
                new { Id = 1 },
                new { Id = 2 },
            },
        };
        var content = new StringContent(JsonSerializer.Serialize(newOrder, JsonSerializerOptions), Encoding.UTF8, "application/json");

        // Act
        var response = await _client.PostAsync(request, content, TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        var problemDetails = await response.Content.ReadFromJsonAsync<ValidationProblemDetails>(TestContext.Current.CancellationToken);

        Assert.NotNull(problemDetails);
        Assert.Equal("Buyer not found", problemDetails.Detail);
    }

    [Fact]
    public async Task CreateOrder_NonExistedParticipantId_ReturnsProblemDetails()
    {
        // Arrange
        var request = "/api/Order";
        var newOrder = new
        {
            Title = "New order",
            Description = "New order description",
            Total = 10_000,
            BuyerId = 1,
            Participants = new List<object>()
            {
                new { Id = 1 },
                new { Id = int.MaxValue } , // Assuming a participant Id that does not exist
            },
        };
        var content = new StringContent(JsonSerializer.Serialize(newOrder, JsonSerializerOptions), Encoding.UTF8, "application/json");

        // Act
        var response = await _client.PostAsync(request, content, TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        var problemDetails = await response.Content.ReadFromJsonAsync<ValidationProblemDetails>(TestContext.Current.CancellationToken);

        Assert.NotNull(problemDetails);
        Assert.Equal("Participant not found", problemDetails.Detail);
    }

    [Fact]
    public async Task CreateOrder_DuplicatedParticipants_ReturnsProblemDetails()
    {
        // Arrange
        var request = "/api/Order";
        var newOrder = new
        {
            Title = "New order",
            Description = "New order description",
            Total = 10_000,
            BuyerId = 1,
            Participants = new List<object>()
            {
                new { Id = 1 },
                new { Id = 1 }, // Duplicated participant Id
            },
        };
        var content = new StringContent(JsonSerializer.Serialize(newOrder, JsonSerializerOptions), Encoding.UTF8, "application/json");

        // Act
        var response = await _client.PostAsync(request, content, TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        var problemDetails = await response.Content.ReadFromJsonAsync<HttpValidationProblemDetails>(TestContext.Current.CancellationToken);

        Assert.NotNull(problemDetails);
        Assert.Contains("Duplicated participant", problemDetails.Errors["Participants"]);
    }
    #endregion CreateOrder

    #region DeleteOrder
    [Fact]
    public async Task DeleteOrder_ValidId_ReturnsNoContent()
    {
        // Arrange
        var orderId = 3; // Assuming an order with Id 3 exists
        var request = $"/api/Order/{orderId}";

        // Act
        var response = await _client.DeleteAsync(request, TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

        // Verify the order is actually deleted
        var getResponse = await _client.GetAsync(request, TestContext.Current.CancellationToken);
        Assert.Equal(HttpStatusCode.NotFound, getResponse.StatusCode);
    }

    [Fact]
    public async Task DeleteOrder_NonExistedId_ReturnsNotFound()
    {
        // Arrange
        var orderId = int.MaxValue; // Assuming an order Id that does not exist
        var request = $"/api/Order/{orderId}";

        // Act
        var response = await _client.DeleteAsync(request, TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
    #endregion DeleteOrder
}
