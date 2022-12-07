using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stas.GA {
    internal class PlayerInventory : Element {
        internal PlayerInventory(IntPtr address) : base(address, "PlayerInventory") {
        }
    }
}
