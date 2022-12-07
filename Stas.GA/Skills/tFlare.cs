using System;
using System.Diagnostics;
using System.Threading;
using V2 = System.Numerics.Vector2;
using V3 = System.Numerics.Vector3;

namespace Stas.GA {
    public class tFlare : aSkill {
        public int get_count { get {//"flare"
                var elem = ui.gui.SkillBar.flare_count;
                return (elem==null || !elem.Contains("/")) ? 0 : int.Parse(elem.Split('/')[0]);
            } }
        public tFlare(Keys _key) : base(_key,88, 0, 0, 900) {
            Debug.Assert(ui.gui.SkillBar.flare_key == _key);
        }
      
        public override void Run(Action do_after=null) {
            if (get_count > 0) {
                Keyboard.KeyPress(key);
                last_use = DateTime.Now;
                do_after?.Invoke();
            } else
                ui.AddToLog("tFlare: cant use=> count==0", MessType.Error);
        }
    }

    public class tTNT : aSkill {//"dynamite"
        int start = 0;
        public int get_count {
            get {
                var elem = ui.gui.SkillBar.tnt_count;
                return (elem == null || !elem.Contains("/")) ? 0 : int.Parse(elem.Split('/')[0]);
            }
        }
        public tTNT(Keys _key) : base(_key, 88, 0, 0, 900) {
            start = get_count;
        }

        public override void Run(Action do_after=null) {
            var curr = get_count;
            if(curr == start && curr != 0) {
                Keyboard.KeyPress(key);
                last_use = DateTime.Now;
                do_after?.Invoke();
            }
        }
    }
}
