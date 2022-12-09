namespace pwnctl.app.Exceptions;

using System;

public sealed class UnparsableAssetException : Exception
{
    public UnparsableAssetException(string assetText)
        : base("Can't parse asset " + assetText)
    { }
}