namespace pwnwrk.domain.Assets.Exceptions;

using System;

public sealed class UnparsableAssetException : Exception
{
    public UnparsableAssetException(string assetText)
        : base("Can't parse asset " + assetText)
    { }
}