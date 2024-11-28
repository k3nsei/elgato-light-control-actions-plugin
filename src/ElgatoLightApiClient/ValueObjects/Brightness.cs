namespace ElgatoLightApiClient.ValueObjects
{
    public readonly struct Brightness(Byte? value)
    {
        public required Byte Value { get; init; } = Math.Clamp(value ?? 1, (Byte)0, (Byte)100);

        public override String ToString() => $"{this.Value.ToString()}%";
    }
}