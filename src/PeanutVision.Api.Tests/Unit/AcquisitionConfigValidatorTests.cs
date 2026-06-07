using PeanutVision.Api.Services;
using PeanutVision.Api.Tests.Infrastructure;

namespace PeanutVision.Api.Tests.Unit;

public class AcquisitionConfigValidatorTests
{
    private readonly AcquisitionConfigValidator _validator;

    public AcquisitionConfigValidatorTests()
    {
        _validator = new AcquisitionConfigValidator(TestCamFileHelper.GetOrCreate());
    }

    private static AcquisitionConfig ValidConfig(string profileId = "crevis-tc-a160k-freerun-rgb8.cam") =>
        new(new ProfileId(profileId));

    // ── Valid configs ──

    [Fact]
    public void Valid_config_returns_no_errors()
    {
        var result = _validator.Validate(ValidConfig());
        Assert.True(result.IsValid);
        Assert.Empty(result.Errors);
    }

    [Fact]
    public void Valid_config_with_frameCount_and_intervalMs()
    {
        var config = new AcquisitionConfig(
            new ProfileId("crevis-tc-a160k-freerun-rgb8.cam"),
            FrameCount: 10,
            IntervalMs: 200);

        var result = _validator.Validate(config);
        Assert.True(result.IsValid);
    }

    // ── ProfileId ──

    [Fact]
    public void Unknown_profileId_returns_error_on_profileId_field()
    {
        var config = new AcquisitionConfig(new ProfileId("nonexistent.cam"));

        var result = _validator.Validate(config);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.Field == "profileId");
    }

    // ── IntervalMs ──

    [Fact]
    public void IntervalMs_below_50_returns_error()
    {
        var config = new AcquisitionConfig(
            new ProfileId("crevis-tc-a160k-freerun-rgb8.cam"),
            IntervalMs: 1);

        var result = _validator.Validate(config);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.Field == "intervalMs");
    }

    [Fact]
    public void IntervalMs_zero_returns_error()
    {
        var config = new AcquisitionConfig(
            new ProfileId("crevis-tc-a160k-freerun-rgb8.cam"),
            IntervalMs: 0);

        var result = _validator.Validate(config);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.Field == "intervalMs");
    }

    [Fact]
    public void IntervalMs_exactly_50_is_valid()
    {
        var config = new AcquisitionConfig(
            new ProfileId("crevis-tc-a160k-freerun-rgb8.cam"),
            IntervalMs: 50);

        var result = _validator.Validate(config);
        Assert.True(result.IsValid);
    }

    [Fact]
    public void IntervalMs_null_is_valid()
    {
        var result = _validator.Validate(ValidConfig());
        Assert.True(result.IsValid);
    }

    // ── FrameCount ──

    [Fact]
    public void FrameCount_zero_returns_error()
    {
        var config = new AcquisitionConfig(
            new ProfileId("crevis-tc-a160k-freerun-rgb8.cam"),
            FrameCount: 0);

        var result = _validator.Validate(config);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.Field == "frameCount");
    }

    [Fact]
    public void FrameCount_negative_returns_error()
    {
        var config = new AcquisitionConfig(
            new ProfileId("crevis-tc-a160k-freerun-rgb8.cam"),
            FrameCount: -1);

        var result = _validator.Validate(config);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.Field == "frameCount");
    }

    [Fact]
    public void FrameCount_null_is_valid()
    {
        var result = _validator.Validate(ValidConfig());
        Assert.True(result.IsValid);
    }

    // ── Multiple errors ──

    [Fact]
    public void Multiple_invalid_fields_return_all_errors()
    {
        var config = new AcquisitionConfig(
            new ProfileId("nonexistent.cam"),
            FrameCount: -1,
            IntervalMs: 10);

        var result = _validator.Validate(config);

        Assert.False(result.IsValid);
        Assert.True(result.Errors.Count >= 3);
        Assert.Contains(result.Errors, e => e.Field == "profileId");
        Assert.Contains(result.Errors, e => e.Field == "frameCount");
        Assert.Contains(result.Errors, e => e.Field == "intervalMs");
    }
}
