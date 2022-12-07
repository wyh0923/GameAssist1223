using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Stas.GA {
    [StructLayout(LayoutKind.Explicit, Pack = 1)]
    public struct TargetableComponentOffsets {
        [FieldOffset(0x00)] public ComponentHeader Header;
        [FieldOffset(0x48)] public bool isTargetable;
        [FieldOffset(0x49)] public bool isHighlightable;
        [FieldOffset(0x4A)] public bool isTargeted; //its mean what toltipe element is show
    }
}
