using System;

namespace pwnctl.app.Exceptions
{
    public class UnparsableAssetException : Exception
    {
        public UnparsableAssetException(string assetText)
            : base("Can't parse asset " + assetText)
        { }
    }
}