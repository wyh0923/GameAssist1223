using System.Runtime.InteropServices;
using V2 = System.Numerics.Vector2;
namespace Stas.GA;

[StructLayout(LayoutKind.Explicit, Pack = 1)]
public struct ElemOffsets {
    [FieldOffset(0x988)] public IntPtr AreaInstanceUi; //pass 3.16
    [FieldOffset(0x328)] public IntPtr Text_or_TextPtr;
    [FieldOffset(0x26C)] public uint link_color;
    [FieldOffset(0x28)] public IntPtr SelfPointer;
    [FieldOffset(0x30)] public StdVector chld_ptr;
    [FieldOffset(0xA8)] public StdTuple2D<float> PositionModifier;
    [FieldOffset(0xD2)] public byte ScaleIndex;
    //[FieldOffset(0xA8)] public float horizontal_scroll;//for root tabpanel with list of tabs=>like sub tab folder in stash]
    [FieldOffset(0xD8)] public IntPtr Root;
    [FieldOffset(0xB0)] public StdWString Id;
    [FieldOffset(0xE0)] public IntPtr Parent;
    [FieldOffset(0xE8)] public V2 Position;
    [FieldOffset(0xE8)] public float X;
    [FieldOffset(0xEC)] public float Y;
    [FieldOffset(0x100)] public float LocalScaleMultiplier;
    [FieldOffset(0x158)] public float Scale;
    [FieldOffset(0x160)] public uint Flags; // variable
    [FieldOffset(0x161)] public byte IsVisibleLocal;
    [FieldOffset(0x160)] public uint ElementBorderColor;
    [FieldOffset(0x164)] public uint ElementBackgroundColor;
    [FieldOffset(0x1C8)] public uint ElementOverlayColor;
    [FieldOffset(0x180)] public StdTuple2D<float> UnscaledSize;
    [FieldOffset(0x180)] public float Width;
    [FieldOffset(0x184)] public float Height;
    [FieldOffset(0x190)] public uint TextBoxBorderColor;
    [FieldOffset(0x1C4)] public uint TextBoxBackgroundColor; //pass 3.16
    [FieldOffset(0x194)] public uint TextBoxOverlayColor;
    [FieldOffset(0x1C0)] public uint HighlightBorderColor;
    [FieldOffset(0x1C3)] public bool isHighlighted;

    [FieldOffset(0x3E8)] public StdWString std_text;
    [FieldOffset(0x378)] public IntPtr text_or_textptr; //5E0 || 316+18
    //0x280=0x18 = 2x float cursor position(relative element top left?
}

public static class UiElementBaseFuncs {
    private const int SHOULD_MODIFY_BINARY_POS = 0x0A;
    private const int IS_VISIBLE_BINARY_POS = 0x0B;

    public const int SCALE_INDEX_1 = 0x002;
    public const int SCALE_INDEX_3 = 0x004;
    public const int SCALE_INDEX_2 = 0x000;

    public static Func<uint, bool> IsVisibleChecker = param =>
    {
        return Util.isBitSetUint(param, IS_VISIBLE_BINARY_POS);
    };

    public static Func<uint, bool> ShouldModifyPos = param =>
    {
        return Util.isBitSetUint(param, SHOULD_MODIFY_BINARY_POS);
    };

    public static Func<uint, bool> Unknown1 = param => { return Util.isBitSetUint(param, 0x02); };

    public static Func<uint, bool> Unknown2 = param => { return Util.isBitSetUint(param, 0x03); };

    public static Func<uint, bool> Unknown3 = param => { return Util.isBitSetUint(param, 0x13); };
}
