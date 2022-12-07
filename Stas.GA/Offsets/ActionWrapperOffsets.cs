using System.Runtime.InteropServices;
using V2 = System.Numerics.Vector2;

namespace Stas.GA; 

[StructLayout(LayoutKind.Explicit, Pack = 1)]
public struct ActionWrapperOffsets {
    [FieldOffset(0xE8)] public IntPtr Target; //3.16=D0
    [FieldOffset(0xB0)] public IntPtr Skills; //3.16=B8 , 3.15 0x150 
    [FieldOffset(0xF0)] public int target_gp_x; // 26C 3.16=D8
    [FieldOffset(0xF0 + 4)] public int target_gp_y;
    [FieldOffset(0x280)] public V2 ovner_gp; //owner entyty grid pos 3.16=280
}
