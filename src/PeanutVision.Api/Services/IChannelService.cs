namespace PeanutVision.Api.Services;

public interface IChannelService
{
    ChannelState ChannelState { get; }
    ProfileId? ActiveProfileId { get; }
    TriggerMode? ChannelTriggerMode { get; }
    IReadOnlySet<ChannelAction> GetAllowedActions();
    void CreateChannel(ProfileId profileId, TriggerMode? triggerMode = null);
    void ReleaseChannel();
}
