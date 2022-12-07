namespace Stas.GA
{
    
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Explicit, Pack = 1)]
    public struct ObjectMagicPropertiesOffsets
    {
        [FieldOffset(0x120 + 0x1c)] public int Rarity;
        [FieldOffset(0x160)] public StdVector Mods;
    }
}