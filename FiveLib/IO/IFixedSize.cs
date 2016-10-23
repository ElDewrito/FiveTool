namespace FiveLib.IO
{
    /// <summary>
    /// A structure that has a guaranteed size and expected alignment when serialized into binary form.
    /// </summary>
    public interface IFixedSize
    {
        ulong GetStructSize();

        ulong GetStructAlignment();
    }
}
