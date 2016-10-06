namespace IntelHexFormatReader
{
    public class IntelHexRecord
    {
        public RecordType RecordType { get; set; }
        public int ByteCount { get; set; }
        public int Address { get; set; }
        public byte[] Bytes { get; set; }
        public int CheckSum { get; set; }
    }
}
