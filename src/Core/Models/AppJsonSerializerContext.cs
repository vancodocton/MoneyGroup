using System.Text.Json.Serialization;

using MoneyGroup.Core.Models.Orders;
using MoneyGroup.Core.Models.Paginations;

namespace MoneyGroup.Core.Models;

[JsonSerializable(typeof(decimal?))]
[JsonSerializable(typeof(OrderDto))]
[JsonSerializable(typeof(PaginatedModel<OrderDetailedDto>))]
public partial class AppJsonSerializerContext : JsonSerializerContext
{
}
