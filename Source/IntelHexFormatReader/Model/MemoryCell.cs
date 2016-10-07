namespace IntelHexFormatReader.Model
{
    /// <summary>
    /// Logical representation of a MemoryCell.
    /// </summary>
    public class MemoryCell
    {
        public int Address { get; private set; }
        public bool Modified { get; set; }
        public byte Value { get; set; }

        public MemoryCell(int address)
        {
            Address = address;
        }

        public override string ToString()
        {
            return string.Format("MemoryCell : {0} Value: {1} (modified = {2})",
                Address.ToString("X8"), Value.ToString("X2"), Modified);
        }
    }
}
