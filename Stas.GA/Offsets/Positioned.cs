using System.Runtime.InteropServices;
using V2 = System.Numerics.Vector2;
namespace Stas.GA;

[StructLayout(LayoutKind.Explicit, Pack = 1)]
public struct PositionedOffsets {
    [FieldOffset(0x000)] public ComponentHeader Header;
    [FieldOffset(0x1D8)] public byte unkn_1d8; 
    [FieldOffset(0x1D9)] public byte Reaction; 
    [FieldOffset(0x268)] public float Rotation; 
    [FieldOffset(0x278)] public float Size;
    [FieldOffset(0x27C)] public float SizeScale;

    [FieldOffset(0x1e0 + 0x1c)] public V2 past_pos;// 3.20
    [FieldOffset(0x200 + 0x14)] public V2 next_pos;// 3.20
    [FieldOffset(0x280 + 0x4)] public StdTuple2D<int> GridPosition; //3.20
    [FieldOffset(0x288)] public V2 curr_pos; // 3.20
}
