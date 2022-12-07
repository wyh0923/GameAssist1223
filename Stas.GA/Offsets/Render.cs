using System.Runtime.InteropServices;
using V2 = System.Numerics.Vector2;
using V3 = System.Numerics.Vector3;

namespace Stas.GA;


[StructLayout(LayoutKind.Explicit, Pack = 1)]
public struct RenderOffsets {
    [FieldOffset(0x0000)] public ComponentHeader Header;

    // Same as Positioned Component CurrentWorldPosition,
    // but this one contains Z axis; Z axis is where the HealthBar is.
    // If you want to use ground Z axis, swap current one with TerrainHeight.
    [FieldOffset(0x78)] public V3 CurrentWorldPosition;
    [FieldOffset(0xA0)] public NativeStringU name;
    // Changing this value will move the in-game healthbar up/down.
    // Not sure if it's really X,Y,Z or something else. They all move
    // healthbar up/down. This might be useless.
    [FieldOffset(0x84)] public StdTuple3D<float> CharactorModelBounds;
    // [FieldOffset(0x00A0)] public StdWString ClassName;

    // Exactly the same as provided in the Positioned component.
    // [FieldOffset(0x00C0)] public float RotationCurrent;
    // [FieldOffset(0x00C4)] public float RotationFuture;
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

