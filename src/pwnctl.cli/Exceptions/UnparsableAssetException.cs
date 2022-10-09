using System;

namespace pwnctl.cli.Exceptions
{
    public class UnparsableAssetException : Exception
    {
        public UnparsableAssetException(string assetText)
            : base("Can't parse asset " + assetText)
        { }
    }
}