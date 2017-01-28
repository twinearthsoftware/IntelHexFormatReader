using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using IntelHexFormatReader.Model;

namespace IntelHexFormatReader
{
    public class HexFileReader
    {
        private IEnumerable<string> hexRecordLines;
        private int memorySize;

        #region Constructors

        public HexFileReader(string fileName, int memorySize)
        {
            if (!File.Exists(fileName))
                throw new ArgumentException(string.Format("File {0} does not exist!", fileName));
            Initialize(File.ReadLines(fileName), memorySize);
        }

        public HexFileReader(IEnumerable<string> hexFileContents, int memorySize)
        {
            Initialize(hexFileContents, memorySize);
        }

        #endregion

        private void Initialize(IEnumerable<string> lines, int memSize)
        {
            var fileContents = lines as IList<string> ?? lines.ToList();
            if (!fileContents.Any()) throw new ArgumentException("Hex file contents can not be empty!");
            if (memSize <= 0) throw new ArgumentException("Memory size must be greater than zero!");
            hexRecordLines = fileContents;
            memorySize = memSize;
        }

        /// <summary>
        /// Parse the currently loaded HEX file contents.
        /// </summary>
        /// <returns>A MemoryBlock representation of the HEX file.</returns>
        public MemoryBlock Parse()
        {
            return ReadHexFile(hexRecordLines, memorySize);
        }

        private static MemoryBlock ReadHexFile(IEnumerable<string> hexRecordLines, int memorySize)
        {
            var result = new MemoryBlock(memorySize);

            var baseAddress = 0;
            var encounteredEndOfFile = false;
            foreach (var hexRecordLine in hexRecordLines)
            {
                var hexRecord = HexFileLineParser.ParseLine(hexRecordLine);
                switch (hexRecord.RecordType)
                {
                    case RecordType.Data:
                        {
                            var nextAddress = hexRecord.Address + baseAddress;
                            for (var i = 0; i < hexRecord.ByteCount; i++)
                            {
                                if (nextAddress + i > memorySize)
                                    throw new IOException(
                                        string.Format("Trying to write to position {0} outside of memory boundaries ({1})!",
                                            nextAddress + i, memorySize));

                                var cell = result.Cells[nextAddress + i];
                                cell.Value = hexRecord.Bytes[i];
                                cell.Modified = true;
                            }
                            break;
                        }
                    case RecordType.EndOfFile:
                        {
                            hexRecord.Assert(rec => rec.Address == 0, "Address should equal zero in EOF.");
                            hexRecord.Assert(rec => rec.ByteCount == 0, "Byte count should be zero in EOF.");
                            hexRecord.Assert(rec => rec.Bytes.Length == 0, "Number of bytes should be zero for EOF.");
                            hexRecord.Assert(rec => rec.CheckSum == 0xff, "Checksum should be 0xff for EOF.");
                            encounteredEndOfFile = true;
                            break;
                        }
                    case RecordType.ExtendedSegmentAddress:
                        {
                            hexRecord.Assert(rec => rec.ByteCount == 2, "Byte count should be 2.");
                            baseAddress = (hexRecord.Bytes[0] << 8 | hexRecord.Bytes[1]) << 4;
                            break;
                        }
                    case RecordType.ExtendedLinearAddress:
                        {
                            hexRecord.Assert(rec => rec.ByteCount == 2, "Byte count should be 2.");
                            baseAddress = (hexRecord.Bytes[0] << 8 | hexRecord.Bytes[1]) << 16;
                            break;
                        }
                    case RecordType.StartSegmentAddress:
                        {
                            hexRecord.Assert(rec => rec.ByteCount == 4, "Byte count should be 4.");
                            hexRecord.Assert(rec => rec.Address == 0, "Address should be zero.");
                            result.CS = (ushort)(hexRecord.Bytes[0] << 8 + hexRecord.Bytes[1]);
                            result.IP = (ushort)(hexRecord.Bytes[2] << 8 + hexRecord.Bytes[3]);
                            break;
                        }
                    case RecordType.StartLinearAddress:
                        hexRecord.Assert(rec => rec.ByteCount == 4, "Byte count should be 4.");
                        hexRecord.Assert(rec => rec.Address == 0, "Address should be zero.");
                        result.EIP =
                            (uint)(hexRecord.Bytes[0] << 24) + (uint)(hexRecord.Bytes[1] << 16)
                            + (uint)(hexRecord.Bytes[2] << 8) + hexRecord.Bytes[3];
                        break;
                }
            }
            if (!encounteredEndOfFile) throw new IOException("No EndOfFile marker found!");
            return result;
        }
    }
}
