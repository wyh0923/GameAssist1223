using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stas.GA {
    public class ModalDialog : Element { // modal, poping after logion
        internal ModalDialog() : base("ModalDialog") {
        }

        public Element open_options => GetTextElem_by_Str("open options");
        public Element later => GetTextElem_by_Str("later");
        public Element keep => GetTextElem_by_Str("keep");
        public Element yes => GetTextElem_by_Str("yes");
        public Element no => GetTextElem_by_Str("no");
        public Element ok => GetTextElem_by_Str("ok");
    }
}
