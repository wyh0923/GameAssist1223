using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stas.GA {
    public class ArchnemesisMod : EntComp {
        public ArchnemesisMod(nint address) : base(address) {
        }

        internal override void Tick(IntPtr ptr, string from=null) {
            Address = ptr;
            if (Address == IntPtr.Zero)
                return;
           
        }
        protected override void CleanUpData() {
            throw new NotImplementedException();
        }
    }
}
