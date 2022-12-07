using System.Runtime.InteropServices;
using System.Text;

namespace Stas.GA;
// TODO what is this? consider chooisng a better name?
[StructLayout(LayoutKind.Explicit, Pack = 1)]
public struct NativeStringU {
    //its totale 16byte buffer to read words shorter than 1-8 letters, 2 bytes per letter
    [FieldOffset(0x0)] public long buf;
    [FieldOffset(0x8)] public long buf2;
    [FieldOffset(0x10)] public uint Size;
    [FieldOffset(0x18)] public uint Capacity;

    public string GetVaue() {
        if (Capacity >= 8) {
            if (Size < 256)
                return ui.m.ReadStringU(buf, (int)Size * 2);

            return ui.m.ReadStringU(buf);
        }

        return Encoding.Unicode.GetString(BitConverter.GetBytes(buf).Concat(BitConverter.GetBytes(buf2))
            .Take((int)Size * 2).ToArray());
    }
}

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct NativeUnicodeText {
    public long Buffer;
    //// There is an optimization in POE (or the framework in which POE is created in), where
    //// if a UnicodeText.LengthWithNullTerminator is less than or equal to 8
    //// then the string is stored locally (without a pointer).
    //// Since the pointer takes 8 bytes and 8 LengthWithNullTerminator takes 16
    //// We have a Reserved8Bytes over here which is then used to store the string.
    public long Reserved8Bytes;

    ////  Length or LengthWithNullTerminator have to be multiplied by 2 for UTF-16 format.
    public long Length;
    //// https://www.fileformat.info/info/unicode/char/0000/index.htm
    public long LengthWithNullTerminator;
}