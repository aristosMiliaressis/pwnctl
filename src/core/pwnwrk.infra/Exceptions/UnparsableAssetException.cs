namespace pwnwrk.infra.Exceptions;

using System;

public class UnparsableAssetException : Exception
{
    public UnparsableAssetException(string assetText)
        : base("Can't parse asset " + assetText)
    { }
}