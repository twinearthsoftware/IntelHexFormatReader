using System;
using System.IO;
using FluentAssertions;
using IntelHexFormatReader;
using IntelHexFormatReader.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace IntelHexFormatReaderTests
{
    [TestClass]
    public class ExtensionsTests
    {

        [TestMethod]
        public void IntelHexRecordAssertionThrowsExceptionWhenFalse()
        {
            var record = HexFileLineParser.ParseLine(":10010000214601360121470136007EFE09D2190140");
            Action act = () => record.Assert(rec => true == false, "Impossible!");
            act.ShouldThrow<IOException>().WithMessage("Impossible*");
        }

        [TestMethod]
        public void LastIndexOfMemoryCellArrayReturnsCorrectIndexWhenFound()
        {
            var cell1 = new MemoryCell(0x00) {Value = 0xF0};
            var cell2 = new MemoryCell(0x01) {Value = 0xB0};
            var cell3 = new MemoryCell(0x02) {Value = 0xF0};
            var cell4 = new MemoryCell(0x03) {Value = 0xB0};
            var cell5 = new MemoryCell(0x04) {Value = 0xF0};
            var cell6 = new MemoryCell(0x05) {Value = 0xF0};
            var cell7 = new MemoryCell(0x06) {Value = 0xAA};

            var cells = new[] { cell1, cell2, cell3, cell4, cell5, cell6, cell7 };

            cells.LastIndexOf(cell => cell.Value == 0xB0).Should().Be(3);
            cells.LastIndexOf(cell => cell.Value == 0xAA).Should().Be(6);
            cells.LastIndexOf(cell => cell.Value == 0xF0).Should().Be(5);
            cells.LastIndexOf(cell => cell.Value == 0xFF).Should().Be(-1); // Not found
        }
    }
}
