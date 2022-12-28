namespace pwnctl.domain.ValueObjects;

using pwnctl.kernel.BaseClasses;

// public sealed class Url : ValueObject
// {
//     public string Url { get; }

//     private Url(string url)
//     {
//         if (Uri.CheckHostName(url) != UriHostNameType.Dns)
//             throw new ArgumentException("Invalid Domain Name: " + url, nameof(url));

//         Url = url;
//     }

//     public static Url Create(string url)
//     {
//         return new Url(url);
//     }

//     protected override IEnumerable<object> GetEqualityComponents()
//     {
//         yield return Url;
//     }
// }
