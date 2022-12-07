using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stas.GA {
    public class DelveDarknessElem : Element {
        internal DelveDarknessElem() : base("DelveDarknessElem") {
        }

        public int darkness {
            get {
                if (!this.IsVisible)
                    return 0;
                var elem = this.GetChildFromIndices(0, 0, 1, 0);
                int.TryParse(elem?.Text, out var res);
                return res;
            }
        }
    }
}
