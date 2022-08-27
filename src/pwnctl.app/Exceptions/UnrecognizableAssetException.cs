using System;

namespace pwnctl.app.Exceptions
{
    public class UnrecognizableAssetException : Exception
    {
        public UnrecognizableAssetException(string assetText)
            : base($"Can't parse asset {assetText}")
        { }
    }
}