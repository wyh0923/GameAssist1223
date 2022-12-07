using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stas.GA;
internal class CursorElement:Element {
    internal CursorElement() : base("CursorElement") {
    }
    public bool IsOpened {
        get {
            return this.mode > CursorItemModes.None;
        }
    }
    public CursorItemModes mode {
        get {
            return (CursorItemModes)ui.m.Read<byte>(Address + 896L);
        }
    }
    public Vector2i ItemSize {
        get {
            byte x = ui.m.Read<byte>(base.Address + 968L);
            byte y = ui.m.Read<byte>(base.Address + 972L);

            return new Vector2i((int)x, (int)y);
        }
    }
    public ItemDPB Item {
        get {
            switch (mode) {
                case CursorItemModes.None:
                    return null;
                case CursorItemModes.PhysicalMove:
                    var address = ui.m.Read<IntPtr>(children[0].Address + 1088L);
                    return new ItemDPB(address, false, Vector2i.One, Vector2i.One);
                case CursorItemModes.VirtualUse:
                case CursorItemModes.VirtualMove:
                    var address2 = ui.m.Read<IntPtr>(children[0].children[0].Address + 928L);
                    return new ItemDPB(address2, false, Vector2i.One, Vector2i.One);
                default:
                    return null;
            }
        }
    }
    public MouseActionType Action => (MouseActionType)ui.m.Read<int>(Address + 0x380);
    public int Clicks => ui.m.Read<int>(Address + 0x24C);
    public string ItemName => ui.m.ReadNativeString(Address + 0x420); //

    Dictionary<string, string> founds = new();

    

    public void FindString() {
        founds.Clear();
        for (int i = 0; i < 1600; i += 8) {
            var ptr = ui.m.Read<NativeStringU>(Address + i);
            if (ptr.Size > 0) {
            }
            var str = ptr.GetVaue();

            //var str = M.ReadNativeString(Address + i);
            if (!string.IsNullOrEmpty(str))
                founds.Add(i.ToString("x"), str);
        }
    }

    public enum MouseActionType {
        Free,
        HoldItem,
        UseItem,
        HoldItemForSell
    }

    public enum CursorItemModes { //dpb doc Version: 0.4.5634.1760
        None = 0,
        PhysicalMove = 1,
        VirtualUse = 2,
        VirtualMove = 3,
        InGameInteraction = 5

    }
}