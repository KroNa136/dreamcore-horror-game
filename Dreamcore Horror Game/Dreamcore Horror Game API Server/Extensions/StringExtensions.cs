namespace DreamcoreHorrorGameApiServer.Extensions;

public static class StringExtensions
{
    public static bool IsEmpty(this string value)
        => value.Length is 0;

    public static bool IsNotEmpty(this string value)
        => value.Length is > 0;
}
