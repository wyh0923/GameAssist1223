#region using
using System.Numerics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using V2 = System.Numerics.Vector2;
using V3 = System.Numerics.Vector3; 
#endregion

namespace Stas.GA {
    public class JumpSkill: aOneHitSkill {
        protected int can_be_detected_after = 11 * 16; // ms on washington DS(ping 125ms) 8*16 in Moskow (ping 8ms)
     
        public JumpSkill(Keys key, int grange= 46, int mana_cost =5, int cast_time =150, int cooldown = 2600) 
            : base(key, grange, mana_cost, cast_time, cooldown) {
            this.name = "quick_dodge";
            me.GetComp<Actor>(out var act);
            if (act == null)
                return;
            skill = act.actor_skills.FirstOrDefault(s => s.Name == "QuickDodge");
            if (skill == null) {
                ui.AddToLog(tName + "not found JumpSkill in " + key + " slot -> check in game", MessType.Warning);
            } else {
                //askill.TryFindGrantedEffectsPerLevel();
                //askill.TryFindActiveSkillWrapper();
                //var int_name = askill.EffectsPerLevel;
            }
            b_need_target = true;
        }
        //same NPC can block jumping if thay stay on jump line
        public override void Run(Action do_after) {
            base.Run(do_after);
        }
        public override bool b_on_cooldown => skill==null?true : skill.IsOnCooldown;
    }
}
