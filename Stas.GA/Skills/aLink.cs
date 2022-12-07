using System;
using V2 = System.Numerics.Vector2;
using V3 = System.Numerics.Vector3;

namespace Stas.GA {
    public abstract class aLink : aBuff { 
        /// <summary>
        /// Name of party member for link use
        /// </summary>
        public string l_name { get; protected private set; }
        public override bool b_already_have {
            get {
                if (ui.curr_link == null)
                    return true;
                return ui.curr_link.buffs.StatusEffects.ContainsKey(buff_name);
            }
        }

        public aLink(Keys _key, int _grange, int _mana_cost,
            int _cast_time, int _cooldown)
            : base(_key, _grange, _mana_cost, _cast_time, _cooldown) {
            

        }
        public void SetOnTarget(V3 pos) {
           // var lp = ui.leader.pos;
            var nlp = V3.Lerp(ui.me.pos, pos, 0.8f);
            var ssp = ui.WorldTPToSP_safe(nlp);
            Mouse.SetCursor(ssp, "set for linking", 3, false);
        }
    }
    public class SoulLink : aLink {
        public SoulLink(Keys _key, int grange = 70, int mana_cost = 27, int cast_time = 280, string _l_name = null )
            : base(_key, grange, mana_cost, cast_time,  60) {
            buff_name = "soul_link_target";
            l_name = _l_name;
        }
      
        public override void Run(Action do_after) {
            base.OneHit();
        }
    }
}
