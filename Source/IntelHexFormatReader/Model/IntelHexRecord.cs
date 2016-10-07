namespace IntelHexFormatReader.Model
{
    /// <summary>
    /// Logical representation of an Intel HEX record (a single line in an Intel HEX file).
    /// </summary>
    public class IntelHexRecord
    {
        public RecordType RecordType { get; set; }
        public int ByteCount { get; set; }
        public int Address { get; set; }
        public byte[] Bytes { get; set; }
        public int CheckSum { get; set; }
    }
}
