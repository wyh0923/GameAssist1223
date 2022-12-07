using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Stas.GA {
    [StructLayout(LayoutKind.Explicit, Pack = 1)]
    public struct StatsComponent {
        [FieldOffset(0x8)] public IntPtr Owner;
        [FieldOffset(0x20)] public long SubStatsPtr;
    }

    [StructLayout(LayoutKind.Explicit, Pack = 1)]
    public struct SubStatsComponent {
        [FieldOffset(0xE8)] public StdVector Stats;
    }
}
