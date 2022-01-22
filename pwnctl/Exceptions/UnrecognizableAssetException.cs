using System;

namespace pwnctl.Exceptions
{
    public class UnrecognizableAssetException : Exception
    {
        public UnrecognizableAssetException(string assetText)
            : base($"Can't parse asset {assetText}")
        { }
    }
}