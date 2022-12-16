using System.Collections.Generic;
using System;

namespace Stas.GA {
    public class ItemsOnGroundLabelElement : RemoteObjectBase {
        public ItemsOnGroundLabelElement(IntPtr ptr):base(ptr) { 
        }
        internal override void Tick(IntPtr ptr, string from=null) {
            if (Address == IntPtr.Zero)
                return;

        }
        protected override void Clear() { }
           
        public Element LabelOnHover {
            get {
                var readObjectAt = new Element(Address+ 0x290, "LabelOnHover");
                return readObjectAt.Address == IntPtr.Zero ? null : readObjectAt;
            }
        }

        public Entity ItemOnHover {
            get {
                var readObjectAt = new Entity(Address + 0x298);
                return readObjectAt.Address == IntPtr.Zero ? null : readObjectAt;
            }
        }

        public string ItemOnHoverPath => ItemOnHover != null ? ItemOnHover.Path : "Null";
        public string LabelOnHoverText => LabelOnHover != null ? LabelOnHover.Text : "Null";
        public int CountLabels => ui.m.Read<int>(Address + 0x2B0);
        public int CountLabels2 => ui.m.Read<int>(Address + 0x2F0);

        public List<LabelOnGround> LabelsOnGround {
            get {
                var address = ui.m.Read<long>(Address + 0x2A8);

                var result = new List<LabelOnGround>();

                if (address <= 0)
                    return new List<LabelOnGround>();

                var limit = 0;

                for (var nextAddress = ui.m.Read<long>(address); nextAddress != address; nextAddress = ui.m.Read<long>(nextAddress)) {
                    var labelOnGround = new LabelOnGround(new IntPtr(nextAddress));

                    if (labelOnGround?.Label?.IsValid ?? false)
                        result.Add(labelOnGround);

                    limit++;

                    if (limit > 100000)
                        return new List<LabelOnGround>();
                }

                return result;
            }
        }
       

      

    }
}