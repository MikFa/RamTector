using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RamTector
{
    public class Memory
    {
        public Memory(float memValue, BytePrefix prefix)
        {
            this.MemValue = memValue;
            this.Prefix = prefix;            
        }
        public Memory(){}

        public float MemValue { get; private set; }
        public BytePrefix Prefix { get; private set; }

        public void ConvertPrefix(BytePrefix targetPrefix)
        {
            var conversion =  (double)this.Prefix - (double)targetPrefix;
            this.MemValue = this.MemValue * (float)Math.Pow(1024, conversion);
            this.Prefix = targetPrefix;
        }
        public float ConvertAndReturn(Memory mem, BytePrefix targetPrefix)
        {
            mem.ConvertPrefix(targetPrefix);
            return mem.MemValue;            
        }

        public override string ToString()
        {
            return $"{this.MemValue} {this.Prefix}";
        }

        public void SetNewMemoryValue(float memValue, BytePrefix prefix)
        {
            this.MemValue = memValue;
            this.Prefix = prefix;
        }
        public void SetNewMemoryValue(Memory mem)
        {
            this.MemValue = mem.MemValue;
            this.Prefix = mem.Prefix;
        }
        public void AddMemoryValue(Memory mem)
        {            
            this.MemValue += mem.Prefix != this.Prefix ? mem.ConvertAndReturn(mem,this.Prefix) : mem.MemValue;               
        }
        public void ResetMemory() { MemValue = 0; Prefix = BytePrefix.Byte; }
    }
    public enum BytePrefix
    {
        Byte = 0,
        KiloBytes = 1,
        MegaBytes = 2,
        GigaBytes = 3,
        TeraBytes = 4,
        PetaBytes = 5,
        ExaBytes = 6,
        ZettaBytes = 7,
        YottaBytes = 8
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
