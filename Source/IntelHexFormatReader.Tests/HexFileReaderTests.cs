using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace IntelHexFormatReader.Tests
{
    [TestClass]
    public class HexFileReaderTests
    {
        private static readonly Random random = new Random();

        // Create an extremely simple valid hex snippet (based on the one from the wikipedia entry on Intel Hex).
        private static string[] CreateValidHexSnippet(int length)
        {
            var snippet = new string[length+1];
            int i;
            for (i = 0; i < length;)
            {
                snippet[i++] = new []
                {
                    ":10010000214601360121470136007EFE09D2190140",
                    ":100110002146017E17C20001FF5F16002148011928",
                    ":10012000194E79234623965778239EDA3F01B2CAA7",
                    ":100130003F0156702B5E712B722B732146013421C7"
                }[random.Next(0,4)];
            }
            snippet[i] = ":00000001FF"; // EOF
            return snippet.ToArray();
        }

        [TestMethod]
        [SuppressMessage("ReSharper", "ObjectCreationAsStatement")]
        public void HexFileReaderThrowsExceptionWhenHexFileContentsIsEmpty()
        {
            Action initialize = () => new HexFileReader(new List<string>(), 1024);
            initialize.ShouldThrow<ArgumentException>().WithMessage("*can not be empty*");
        }

        [TestMethod]
        [SuppressMessage("ReSharper", "ObjectCreationAsStatement")]
        public void HexFileReaderThrowsExceptionWhenMemorySizeIsZero()
        {
            Action initialize = () => new HexFileReader(CreateValidHexSnippet(3), 0);
            initialize.ShouldThrow<ArgumentException>().WithMessage("Memory size must be greater*");
        }

        [TestMethod]
        public void HexFileReaderThrowsExceptionWhenEOFIsMissing()
        {
            var snippet = CreateValidHexSnippet(4);
            snippet = snippet.Take(snippet.Length - 1).ToArray(); // DROP EOF

            Action act = () => new HexFileReader(snippet, 1024).Parse();
            act.ShouldThrow<IOException>().WithMessage("No EndOfFile*");
        }

        [TestMethod]
        public void HexFileReaderThrowsExceptionWhenEOFByteCountIsMalformed()
        {
            var snippet = CreateValidHexSnippet(1);
            snippet[1] = ":01000001F707"; // Introduce one byte (illegal in the context of EOF).

            Action act = () => new HexFileReader(snippet, 1024).Parse();
            act.ShouldThrow<IOException>().WithMessage("Byte count should be zero in EOF*");
        }

        [TestMethod]
        public void HexFileReaderThrowsExceptionWhenEOFAddressIsMalformed()
        {
            var snippet = CreateValidHexSnippet(1);
            snippet[1] = ":00010001FE"; // Address not zero (illegal in the context of EOF).

            Action act = () => new HexFileReader(snippet, 1024).Parse();
            act.ShouldThrow<IOException>().WithMessage("Address should equal zero in EOF*");
        }

        [TestMethod]
        public void HexFileReaderThrowsExceptionWhenTryingToWriteOutOfBounds()
        {
            Action act = () => new HexFileReader(CreateValidHexSnippet(4), 32).Parse(); // Unsufficient memory size
            act.ShouldThrow<IOException>().WithMessage("Trying to write *outside of memory boundaries*");
        }
    }
}
