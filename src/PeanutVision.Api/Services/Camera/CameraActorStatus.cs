using PeanutVision.MultiCamDriver;

namespace PeanutVision.Api.Services.Camera;

public sealed record CameraActorStatus(
    bool IsActive,
    bool HasFrame,
    string? LastError,
    ChannelState ChannelState,
    string? ActiveProfileId,
    IReadOnlySet<ChannelAction> AllowedActions,
    AcquisitionStatisticsSnapshot? Statistics,
    IReadOnlyList<ChannelEvent> RecentEvents);
