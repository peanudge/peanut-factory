using PeanutVision.MultiCamDriver;

namespace PeanutVision.Api.Services;

public sealed record AcquisitionStatus(
    ChannelState ChannelState,
    AcquisitionConfig? ActiveConfig,
    bool HasFrame,
    string? LastError,
    AcquisitionStatisticsSnapshot? Statistics,
    IReadOnlyList<ChannelEvent> RecentEvents,
    IReadOnlySet<ChannelAction> AllowedActions
)
{
    public bool IsActive => ChannelState == ChannelState.Active;
}
