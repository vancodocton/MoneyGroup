using System.Text.Json.Serialization;

using MoneyGroup.Core.Models.Orders;
using MoneyGroup.Core.Models.Paginations;
using MoneyGroup.Core.Models.Users;

namespace MoneyGroup.Core.Models;

[JsonSerializable(typeof(decimal?))]
[JsonSerializable(typeof(UserDto))]
[JsonSerializable(typeof(PaginatedModel<UserDto>))]
[JsonSerializable(typeof(OrderDto))]
[JsonSerializable(typeof(PaginatedModel<OrderDetailedDto>))]
public partial class AppJsonSerializerContext : JsonSerializerContext
{
}
