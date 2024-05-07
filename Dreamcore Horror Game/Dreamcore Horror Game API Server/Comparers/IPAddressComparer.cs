using System.Net;

namespace DreamcoreHorrorGameApiServer.Comparers;

public class IPAddressComparer : IComparer<object?>
{
    public int Compare(object? x, object? y) => (x, y) switch
    {
        (null, null) => 0,
        (null, IPAddress) => -1,
        (IPAddress, null) => 1,
        (IPAddress, IPAddress) => string.Compare(x.ToString(), y.ToString()),
        _ => throw new InvalidCastException()
    };
}
