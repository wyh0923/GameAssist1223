using System.Runtime.InteropServices;
using V2 = System.Numerics.Vector2;
namespace Stas.GA;

[StructLayout(LayoutKind.Explicit, Pack = 1)]
public struct PositionedOffsets {
    [FieldOffset(0x000)] public ComponentHeader Header;
    [FieldOffset(0x1D8)] public byte unkn_1d8; //dpb 3.19.2
    [FieldOffset(0x1D9)] public byte Reaction; //dpb 3.19.2
    [FieldOffset(0x260)] public StdTuple2D<int> GridPosition; //dpb 3.19.2
    [FieldOffset(0x268)] public float Rotation; //dpb 3.19.2
    [FieldOffset(0x278)] public float Size;//dpb 3.19.2
    [FieldOffset(0x27C)] public float SizeScale;//dpb 3.19.2
    //https://discord.com/channels/@me/904687736152293407/1043720132972773437
    [FieldOffset(0x22C)] public V2 past_pos;// 3.19.2
    [FieldOffset(0x238)] public V2 next_pos;// 3.19.2
    [FieldOffset(0x288)] public V2 curr_pos;// 3.19.2
}
