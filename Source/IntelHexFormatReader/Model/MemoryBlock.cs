namespace IntelHexFormatReader.Model
{
    public class MemoryBlock
    {
        public int MaxAddress { get; set; }
        public MemoryCell[] Cells { get; set; }
    }
}
