using System.Runtime.InteropServices;
using V2 = System.Numerics.Vector2;
using V3 = System.Numerics.Vector3;

namespace Stas.GA;


[StructLayout(LayoutKind.Explicit, Pack = 1)]
public struct RenderOffsets {
    [FieldOffset(0x0000)] public ComponentHeader Header;
    [FieldOffset(0x80 + 0x18)] public V3 CurrentWorldPosition;
    [FieldOffset(0xA0)] public NativeStringU name;
    [FieldOffset(0x84)] public StdTuple3D<float> CharactorModelBounds;
    [FieldOffset(0xD8)] public float TerrainHeight;
}
[StructLayout(LayoutKind.Sequential, Pack = 1)]
internal struct RenderOffsetsDPB {
    public ComponentHeader Header;
    private long intptr_0;
    private long intptr_1;
    private long intptr_2;
    private long intptr_3;
    private long intptr_4;
    private long intptr_5;
    private long intptr_6;
    private long intptr_7;
    private long intptr_8;
    private long intptr_9;
    private long intptr_10;
    private long intptr_11;
    private long intptr_12;
    public StdTuple3D<float> CurrentWorldPosition;
    public StdTuple3D<float> CharactorModelBounds;
    private StdTuple2D<float> vector2_0;
    private StdTuple2D<float> vector2_1;
    public NativeStringU name_u;
    public StdTuple3D<float> vector3_2Rotation;
    private int int_16;
    private int int_17;
    private int int_18;
    public float TerrainHeight;
}

