using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RamTector
{
    public class Memory
    {
        public Memory(decimal memValue, BytePrefix prefix)
        {
            this.MemValue = memValue;
            this.Prefix = prefix;            
        }
        public decimal MemValue { get; private set; }
        public BytePrefix Prefix { get; private set; }

        public void ConvertPrefix(BytePrefix targetPrefix)
        {
            var conversion =  (double)this.Prefix - (double)targetPrefix;
            this.MemValue = this.MemValue * (decimal)Math.Pow(10, conversion);
            this.Prefix = targetPrefix;
        }

        public override string ToString()
        {
            return $"{this.MemValue} {this.Prefix}";
        }

        public void SetNewMemoryValue(decimal memValue, BytePrefix prefix)
        {
            this.MemValue = memValue;
            this.Prefix = prefix;
        }
        public void SetNewMemoryValue(Memory mem)
        {
            this.MemValue = mem.MemValue;
            this.Prefix = mem.Prefix;
        }
    }
    public enum BytePrefix
    {
        Byte = 0,
        KiloBytes = 3,
        MegaBytes = 6,
        GigaBytes = 9,
        TeraBytes = 12,
        PetaBytes = 15,
        ExaBytes = 18,
        ZettaBytes = 21,
        YottaBytes = 24
    }

    public enum HerzPrefix
    {
        None = 0,
        Hertz = 1,
        KiloHertz = 2,
        MegaHertz = 3,
        GigaHertz = 4,
        TeraHertz = 5,
        PetaHertz = 6,
        ExaHertz = 7,
        ZettaHertz = 8,
        YottaHertz = 9
    }
}
