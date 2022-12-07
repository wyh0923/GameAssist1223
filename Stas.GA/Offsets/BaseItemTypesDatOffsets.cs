namespace Stas.GA
{
    using System.Runtime.InteropServices;


    [StructLayout(LayoutKind.Explicit, Pack = 1)]
    public struct BaseItemTypesDatOffsets
    {
        [FieldOffset(0x0030)] public StdWString BaseNamePtr;
    }
}