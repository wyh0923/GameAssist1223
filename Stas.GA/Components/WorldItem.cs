using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stas.GA {
    public class WorldItem : EntComp {
        public WorldItem(IntPtr address) : base(address) { //1B1DD150C10
        }

        //Size 0x28
        public Entity ItemEntity { get; private set; } = new ();
        internal override void Tick(IntPtr ptr, string from=null) {
            Address = ptr;
            ItemEntity.Tick(ui.m.Read<IntPtr>(Address + 0x28));
        }
        protected override void Clear() {
            ItemEntity.Tick(IntPtr.Zero);
        }

    

    }
}
