using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Stas.GA {
    [StructLayout(LayoutKind.Explicit, Pack = 1)]
    public struct ModsComponentOffsets {
        [FieldOffset(0x030)] public readonly StdVector UniqueName;
        [FieldOffset(0x0A8)] public readonly bool Identified;
        [FieldOffset(0x0AC)] public readonly int ItemRarity;
        [FieldOffset(0x0B8)] public readonly StdVector ImplicitModsArray;
        [FieldOffset(0x0D0)] public readonly StdVector ExplicitModsArray;
        [FieldOffset(0x0E8)] public readonly StdVector EnchantedModsArray;
        [FieldOffset(0x100)] public readonly StdVector ScourgeModsArray;
        [FieldOffset(0x1F0)] public readonly IntPtr ModsComponentDetailsKey; //TooltipintPtr
        [FieldOffset(0x220)] public readonly int ItemLevel;
        [FieldOffset(0x224)] public readonly int RequiredLevel;
        [FieldOffset(0x228)] public readonly long IncubatorKey;
        [FieldOffset(0x238)] public readonly short IncubatorKillCount;
        [FieldOffset(0x23D)] public readonly byte IsMirrored;
        [FieldOffset(0x23E)] public readonly byte IsSplit;
        [FieldOffset(0x23F)] public readonly byte IsUsable;
        [FieldOffset(0x241)] public readonly byte IsSynthesised;


        public const int ItemModRecordSize = 0x38;
        public const int NameOffset = 0x04;
        public const int NameRecordSize = 0x10;
        public const int StatRecordSize = 0x20;

    }

    [StructLayout(LayoutKind.Explicit, Pack = 1)]
    public readonly struct ModsComponentDetailsOffsets {
        [FieldOffset(0x008)] public readonly StdVector ImplicitStatsArray;
        [FieldOffset(0x048)] public readonly StdVector EnchantedStatsArray;
        [FieldOffset(0x088)] public readonly StdVector ScourgeStatsArray;
        [FieldOffset(0x0C8)] public readonly StdVector ExplicitStatsArray;
        [FieldOffset(0x108)] public readonly StdVector CraftedStatsArray;
        [FieldOffset(0x148)] public readonly StdVector FracturedStatsArray;

    }
}
