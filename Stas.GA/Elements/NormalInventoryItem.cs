using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stas.GA {
    public class NormalInventoryItem : Element {
        public NormalInventoryItem(IntPtr ptr) : base(ptr, "NormalInventoryItem") {

        }
        internal override void Tick(IntPtr ptr, string from=null) {
            if (Address == IntPtr.Zero)
                return;
            var offs = ui.m.Read<NormalInventoryItemOffsets>(Address);
            InventPosX = offs.InventPosX;
            InventPosY = offs.InventPosY;
            ItemWidth = offs.Width;
            ItemHeight = offs.Height;
            Item.Tick(offs.Item);
            ToolTip.Tick(ui.m.Read<IntPtr>(Address + 0xB20), tName);
        }
        public Entity Item { get; } = new Entity();
        public virtual int InventPosX { get; private set; }
        public virtual int InventPosY { get; private set; }
        public virtual int ItemWidth { get; private set; }
        public virtual int ItemHeight { get; private set; }

        public enum ToolTipType {
            None,
            InventoryItem,
            ItemOnGround,
            ItemInChat
        }

        public ToolTipType toolTipType => ToolTipType.InventoryItem;

        public Element ToolTip { get; } = new Element(IntPtr.Zero, "ToolTip");

        //0xB40 0xB48 some inf about image DDS
        public override string ToString() {
             Item.GetComp<Mods>(out var mods);

            return "x=" + InventPosX + " y=" + InventPosY + " w=" + ItemWidth + " h=" + ItemHeight;
        }
    }
}
