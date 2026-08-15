using MoneyGroup.Core.Models.Orders;

namespace MoneyGroup.UnitTests.Builders;

/// <summary>
/// Builds <see cref="OrderDto"/> instances for tests. <see cref="Valid"/> returns a
/// dto that passes every OrderDtoValidator rule, so each test only states the one
/// field it cares about.
/// </summary>
public sealed class OrderDtoBuilder
{
    private int _id;
    private string _title = "Title";
    private string? _description = "Description";
    private decimal _total;
    private int _buyerId = 1;
    private IEnumerable<ParticipantDto> _participants = [new ParticipantDto { ParticipantId = 1 }];

    public static OrderDtoBuilder Valid() => new();

    public OrderDtoBuilder WithId(int id)
    {
        _id = id;
        return this;
    }

    public OrderDtoBuilder WithTitle(string? title)
    {
        _title = title!;
        return this;
    }

    public OrderDtoBuilder WithDescription(string? description)
    {
        _description = description;
        return this;
    }

    public OrderDtoBuilder WithTotal(decimal total)
    {
        _total = total;
        return this;
    }

    public OrderDtoBuilder WithBuyer(int buyerId)
    {
        _buyerId = buyerId;
        return this;
    }

    public OrderDtoBuilder WithParticipants(params int[] participantIds)
    {
        _participants = [.. participantIds.Select(id => new ParticipantDto { ParticipantId = id })];
        return this;
    }

    public OrderDtoBuilder WithParticipants(IEnumerable<ParticipantDto> participants)
    {
        _participants = participants;
        return this;
    }

    public OrderDtoBuilder WithNoParticipants()
    {
        _participants = [];
        return this;
    }

    public OrderDtoBuilder WithNullParticipants()
    {
        _participants = null!;
        return this;
    }

    public OrderDto Build() => new()
    {
        Id = _id,
        Title = _title,
        Description = _description,
        Total = _total,
        BuyerId = _buyerId,
        Participants = _participants,
    };
}
