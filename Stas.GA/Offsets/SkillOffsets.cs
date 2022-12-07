using System.Runtime.InteropServices;
namespace Stas.GA;

[StructLayout(LayoutKind.Explicit, Pack = 1)]
public struct SkillOffsets {
    [FieldOffset(0x10)] public ushort id;
    [FieldOffset(0x18)] public IntPtr GrantedEffectsPerLevelPtr;
    [FieldOffset(0x50)] public byte CanBeUsedWithWeapon;//+
    [FieldOffset(0x51)] public byte CanBeUsed;//+
    [FieldOffset(0x84)] public int TotalUses;//+
    [FieldOffset(0x88)] public int MaxUses;//+
    [FieldOffset(0x8C)] public int Cooldown;//+
    //[FieldOffset(0x90)] public int CostOLD;
    [FieldOffset(0x6C)] public int SoulsPerUse;
    [FieldOffset(0x70)] public int TotalVaalUses;
    [FieldOffset(0xD8)] public GEPL GEPL_ptr;
}

[StructLayout(LayoutKind.Explicit, Pack = 1)]
public struct GEPL {
    [FieldOffset(0x28)] public IntPtr GrantedEffectPerLevel_ptr;
    [FieldOffset(0x40)] public IntPtr GrantedEffectStatSetPerLevel_ptr;
}