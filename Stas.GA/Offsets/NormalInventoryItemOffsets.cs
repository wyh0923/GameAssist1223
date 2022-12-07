using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Stas.GA {
    [StructLayout(LayoutKind.Explicit, Pack = 1)]
    public struct NormalInventoryItemOffsets {
        [FieldOffset(0x3F8)] public IntPtr Tooltip;
        [FieldOffset(0x440)] public IntPtr Item;
        [FieldOffset(0x448)] public int InventPosX;
        [FieldOffset(0x44C)] public int InventPosY;
        [FieldOffset(0x450)] public int Width;
        [FieldOffset(0x454)] public int Height;
    }
}
