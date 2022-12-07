#region using
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using V2 = System.Numerics.Vector2;
using V3 = System.Numerics.Vector3;
#endregion

namespace Stas.GA {
    public partial class InputChecker {
        public void F2() {
            if (ui.b_alt && Keyboard.b_Try_press_key(Keys.F2, "InputChecker", 900)) {
               
              
            }
            
            if (Keyboard.b_Try_press_key(Keys.F2, "InputChecker")) {
                StaticMapItem smi = null;
                var mia = ui.curr_map.transit_close;
                //check mouse over item(in-game selection)
                foreach (var mi in mia) {
                    if (mi.ent.GetComp<Targetable>(out var res) && res.isTargeted) {
                        smi = mi;
                        break;
                    }
                }
                string name = null;
                if (smi != null && smi.ent.RenderName.Length > 0) {
                    name = smi.ent.RenderName;
                }

                if (mia.Count > 0) { //not selected transite area
                    //if (ui.curr_role == Role.Master) {
                    //    ui.SendToBots(Opcode.Transit, name.To_UTF8_Byte());
                    //}
                    //ui.tasker.Reset();
                    //ui.tasker.TaskPop(new Transit(name));
                }
            }
        }
    }
}
