using System;
using System.IO;
using FluentAssertions;
using IntelHexFormatReader.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace IntelHexFormatReader.Tests
{
    [TestClass]
    public class HexFileLineParserTests
    {

        [TestMethod]
        public void HexFileLineParserThrowsExceptionLineIsWhenNull()
        {
            Action nullArgAction = () => HexFileLineParser.ParseLine(null);
            nullArgAction.ShouldThrow<IOException>().WithMessage("*can not be null*");
        }

        [TestMethod]
        public void HexFileLineParserThrowsExceptionWhenMinimumLengthIsNotReached()
        {
            Action emptyLineAction = () => HexFileLineParser.ParseLine(string.Empty);
            Action just1CharShortAction = () => HexFileLineParser.ParseLine(":000010015");

            emptyLineAction.ShouldThrow<IOException>().WithMessage("*is too short*");
            just1CharShortAction.ShouldThrow<IOException>().WithMessage("*is too short*");
        }

        [TestMethod]
        public void HexFileLineParserThrowsExceptionWhenFirstCharacterIsNoColon()
        {
            string randomLine;
            do { randomLine = Guid.NewGuid().ToString(); } 
            while (randomLine.StartsWith(":"));

            Action randomStringAction = () => HexFileLineParser.ParseLine(randomLine);

            randomStringAction.ShouldThrow<IOException>().WithMessage("*Illegal line start character*");
        }

        [TestMethod]
        public void HexFileLineParserThrowsExceptionForInvalidCharactersInByteCount()
        {
            const string illegalByteCount = ":uy00000110"; // no legal hex representation for byte count
            Action parseIllegalByteCount = () => HexFileLineParser.ParseLine(illegalByteCount);
            parseIllegalByteCount
                .ShouldThrow<IOException>()
                .WithInnerException<FormatException>()
                .WithMessage("Unable to extract byte count*");
        }

        [TestMethod]
        public void HexFileLineParserThrowsExceptionWhenCalculatedRequiredLengthIsWrong()
        {
            const string invalidRecord1 = ":100130003F015672B5E712B722B732146013421C7";   // one char too short
            const string invalidRecord2 = ":100130003F0156702B5E712B722B7321460133421C7"; // one char too long

            Action parseInvalidRecord1 = () => HexFileLineParser.ParseLine(invalidRecord1);
            Action parseInvalidRecord2 = () => HexFileLineParser.ParseLine(invalidRecord2);

            parseInvalidRecord1.ShouldThrow<IOException>().WithMessage("*required record length*");
            parseInvalidRecord2.ShouldThrow<IOException>().WithMessage("*required record length*");
        }

        [TestMethod]
        public void HexFileLineParserThrowsExceptionForInvalidCharactersInAddress()
        {
            const string invalidAddress = ":010frf03ffe5"; // 'r' is not valid in hex representation of address
            Action parseInvalidAddress = () => HexFileLineParser.ParseLine(invalidAddress);
            parseInvalidAddress
                .ShouldThrow<IOException>()
                .WithInnerException<FormatException>()
                .WithMessage("Unable to extract address*");
        }

        [TestMethod]
        public void HexFileLineParserThrowsExceptionForInvalidCharactersInRecordType()
        {
            const string invalidRecordType = ":01ffffGUffe5"; // 'GU' is not valid in hex representation of recordType
            Action parseInvalidRecordType = () => HexFileLineParser.ParseLine(invalidRecordType);
            parseInvalidRecordType
                .ShouldThrow<IOException>()
                .WithInnerException<FormatException>()
                .WithMessage("Unable to extract record type*");
        }

        [TestMethod]
        public void HexFileLineParserThrowsExceptionForInvalidRecordTypeEnumValue()
        {
            const string invalidRecordType = ":01ffff06ffe5"; // '06' is not valid a valid recordType
            Action parseInvalidRecordType = () => HexFileLineParser.ParseLine(invalidRecordType);
            parseInvalidRecordType
                .ShouldThrow<IOException>()
                .WithMessage("Invalid record type value*");            
        }

        [TestMethod]
        public void HexFileLineParserThrowsExceptionForInvalidCharactersInByteSequence()
        {
            const string invalidBytes = ":04001000FFAAFFuy01"; 
            // 'uy' (last byte) is not valid in a hex representation of a byte sequence
            Action parseInvalidBytes = () => HexFileLineParser.ParseLine(invalidBytes);
            parseInvalidBytes
                .ShouldThrow<IOException>()
                .WithInnerException<FormatException>()
                .WithMessage("Unable to extract bytes for*");
        }

        [TestMethod]
        public void HexFileLineParserThrowsExceptionForInvalidCharactersInCheckSum()
        {
            const string invalidChecksum = ":04001000FFAAFFFFt1"; 
            // 't1' is not valid for a hex representation of a checksum
            Action parseInvalidCheckSum = () => HexFileLineParser.ParseLine(invalidChecksum);
            parseInvalidCheckSum
                .ShouldThrow<IOException>()
                .WithInnerException<FormatException>()
                .WithMessage("Unable to extract checksum for*");
        }

        [TestMethod]
        public void HexFileLineParserThrowsExceptionForInvalidCheckSum()
        {
            const string invalidCheckSum1 = ":10010000214601360121470136007EFE09D2190141";
            const string invalidCheckSum2 = ":100130003F0156702B5E712B722B732146013421C3";
            const string invalidCheckSum3 = ":00000001F1";

            Action parseInvalidRecord1 = () => HexFileLineParser.ParseLine(invalidCheckSum1);
            Action parseInvalidRecord2 = () => HexFileLineParser.ParseLine(invalidCheckSum2);
            Action parseInvalidRecord3 = () => HexFileLineParser.ParseLine(invalidCheckSum3);

            parseInvalidRecord1.ShouldThrow<IOException>().WithMessage("Checksum for line*is incorrect*");
            parseInvalidRecord2.ShouldThrow<IOException>().WithMessage("Checksum for line*is incorrect*");
            parseInvalidRecord3.ShouldThrow<IOException>().WithMessage("Checksum for line*is incorrect*");
        }

        [TestMethod]
        public void HexFileLineParserReadsCorrectlyFormattedRecords()
        {
            const string line1 = ":10010000214601360121470136007EFE09D2190140";
            const string line2 = ":100130003F0156702B5E712B722B732146013421C7";
            const string line3 = ":00000001FF";

            Func<string,IntelHexRecord> parseLine = HexFileLineParser.ParseLine;

            var result1 = parseLine(line1);
            var result2 = parseLine(line2);
            var result3 = parseLine(line3);

            result1.Should().NotBeNull();
            result1.RecordType.Should().Be(RecordType.Data);
            result1.Address.Should().Be(256);
            result1.ByteCount.Should().Be(16);
            result1.CheckSum.Should().Be(64);
            result1.Bytes[0].Should().Be(33);
            result1.Bytes[1].Should().Be(70);
            result1.Bytes[2].Should().Be(1);
            result1.Bytes.Length.Should().Be(16);

            result2.Should().NotBeNull();
            result2.RecordType.Should().Be(RecordType.Data);
            result2.Address.Should().Be(304);
            result2.ByteCount.Should().Be(16);
            result2.CheckSum.Should().Be(199);
            result2.Bytes[0].Should().Be(63);
            result2.Bytes[1].Should().Be(1);
            result2.Bytes[2].Should().Be(86);
            result2.Bytes.Length.Should().Be(16);

            result3.Should().NotBeNull();
            result3.RecordType.Should().Be(RecordType.EndOfFile);
            result3.Address.Should().Be(0);
            result3.ByteCount.Should().Be(0);
            result3.CheckSum.Should().Be(255);
            result3.Bytes.Length.Should().Be(0);
        }
    }
}
