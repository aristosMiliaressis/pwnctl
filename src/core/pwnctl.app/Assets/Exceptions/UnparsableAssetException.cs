namespace pwnctl.app.Assets.Exceptions;

using pwnctl.app.Common.Exceptions;

public sealed class UnparsableAssetException : AppException
{
    public UnparsableAssetException(string assetText)
        : base("Can't parse asset " + assetText)
    { }
}