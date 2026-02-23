namespace PeanutVision.Api.Services;

public readonly record struct ProfileId
{
    public string Value { get; }

    public ProfileId(string value)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(value);
        Value = value;
    }

    public static implicit operator ProfileId(string value) => new(value);
    public override string ToString() => Value;
}
