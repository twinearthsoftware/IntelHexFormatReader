using System;
using System.IO;
using System.Linq;
using IntelHexFormatReader.Model;

namespace IntelHexFormatReader
{
    public static class Extensions
    {
        /// <summary>
        /// Verify an assertion about an IntelHexRecord, and throw an IOException if the predicate is not true.
        /// </summary>
        /// <param name="record">The record to verify the assertion for.</param>
        /// <param name="predicate">The assertion to verify.</param>
        /// <param name="message">The message to show in the exception if the assertion fails.</param>
        public static void Assert(this IntelHexRecord record, Func<IntelHexRecord, bool> predicate, string message)
        {
            if (!predicate(record)) 
                throw new IOException(
                    string.Format("{0} -- record {1}!", message, record));
        }

        /// <summary>
        /// Returns the index of the last item in the list for which a certain predicate is true.
        /// 
        /// Returns -1 when no item is found for which the predicate is true.
        /// </summary>
        /// <typeparam name="MemoryCell">MemoryCell</typeparam>
        /// <param name="source">The list of cells to consider.</param>
        /// <param name="predicate">The predicate to test.</param>
        /// <returns></returns>
        public static int LastIndexOf<MemoryCell>(this MemoryCell[] source, Func<MemoryCell, bool> predicate)
        {
            var index = source.Length -1;
            foreach (var item in source.Reverse())
            {
                if (predicate.Invoke(item)) return index;
                index--;
            }
            return -1;
        }
    }
}
