using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stas.GA {
    public class NpcDialog : Element {
        internal NpcDialog() : base("NpcDialog") {
        }
        internal override void Tick(IntPtr ptr, string from) {
            base.Tick(ptr, from); //0x000002a003050ba0
            if (Address == IntPtr.Zero)
                return;
            Goodbye = GetLinkWith("Goodbye");
            Continue = GetLinkWith("Continue");
            Sell = GetLinkWith("Sell Items");
            Purchase = GetLinkWith("Purchase Items");
            Reward = GetLinkEndsWith("Reward") ?? GetLinkEndsWith("Reward 2");
          
            var ela = this.GetChildFromIndices(0, 2);
            if (ela != null) {
                var _links = new List<Element>();
                var chi = 0;
                foreach (var e in ela.children) {
                    if (e.children.Count == 1)
                        _links.Add(new Element(e.GetChildAtIndex(0).Address, tName + ".chld[" + chi + "]"));
                    chi++;
                }

                links = _links;
            }
        }
        public string npc_name => GetChildFromIndices(1, 3)?.Text;
        public List<Element> links { get; private set; }
        public Element GetLinkWith(string txt) {
            return GetTextElem_by_Str(txt);
        }

        public Element GetLinkEndsWith(string txt) {
            //return res.GetObject<LinkElement>(res.Address);
            return GetElem_ends_wit(txt);
        }

        public Element Reward { get; private set; } = new Element("Reward");
        public Element Goodbye { get; private set; } = new Element("Goodbye");
        public Element Continue { get; private set; } = new Element("Continue");
        public Element Sell { get; private set; } = new Element("Sell");
        public Element Purchase { get; private set; } = new Element("Purchase");
    }
}
