namespace Stas.GA
{
    using System;
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Explicit, Pack = 1)]
    public struct ChestOffsets {
        [FieldOffset(0x000)] public ComponentHeader Header;
        [FieldOffset(0x158)] public IntPtr ChestsDataPtr;
        [FieldOffset(0x160)] public bool IsOpened;
        [FieldOffset(0x161)] public bool IsLocked;
        [FieldOffset(0x165)] public byte Quality;
    }

    [StructLayout(LayoutKind.Explicit, Pack = 1)]
    public struct ChestsStructInternal {
        [FieldOffset(0x20)] public bool OpeningDestroys;
        [FieldOffset(0x21)] public bool IsLarge; //IsLarge[dbp]
        [FieldOffset(0x22)] public bool Stompable;
        [FieldOffset(0x23)] public bool AxisAligned;
        [FieldOffset(0x25)] public bool OpenOnDamage;
        [FieldOffset(0x28)] public bool OpenChestWhenDemonsDie;
        [FieldOffset(0x50)] public IntPtr StrongboxDatPtr;
        [FieldOffset(0x60)] public int DropSlots;
    }
    
}
