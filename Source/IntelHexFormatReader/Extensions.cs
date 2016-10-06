using System;
using System.IO;

namespace IntelHexFormatReader
{
    internal static class Extensions
    {
        internal static void Assert(this IntelHexRecord record, Func<IntelHexRecord, bool> func, string message)
        {
            if (!func(record)) 
                throw new IOException(
                    string.Format("{0} -- record {1}!", message, record));
        }
    }
}
