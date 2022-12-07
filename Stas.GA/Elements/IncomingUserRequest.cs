using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stas.GA {
    public class IncomingUserRequest : Element {
        internal IncomingUserRequest() : base("IncomingUserRequest") {
        }

        //how to finde?
        //ui.test_elem = ui.gui.GetTextElemWithStr("sent you a party invite").Parent.Parent.Parent;
        public bool b_trade => sent_you_elem?.Text == "sent you a trade request" && accept != null;
        public bool b_invate => sent_you_elem?.Text == "sent you a party invite" && accept != null;
        public Element sent_you_elem => GetTextElem_with_Str("sent you a");
        public Element trade_elem => GetTextElem_with_Str("trade"); // 
        public string from {
            get {
                if (sent_you_elem != null)
                    return sent_you_elem.Parent.GetChildFromIndices(0, 1).Text;
                return null;
            }
        }
        public Element accept => GetTextElem_by_Str("accept");
        public Element decline => GetTextElem_by_Str("decline");
    }
}
