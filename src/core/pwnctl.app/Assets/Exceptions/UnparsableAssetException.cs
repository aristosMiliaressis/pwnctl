namespace pwnctl.app.Assets.Exceptions;

using pwnctl.app.Common.Exceptions;

public sealed class UnparsableAssetException : AppException
{
    public UnparsableAssetException(string assetText)
        : base("Can't parse asset " + assetText)
    { }

    public UnparsableAssetException(string assetText, Exception innerEx)
        : base("Can't parse asset " + assetText, innerEx)
    { }
}