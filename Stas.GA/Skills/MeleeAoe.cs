using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using V2 = System.Numerics.Vector2;
using V3 = System.Numerics.Vector3;

namespace Stas.GA {
    public class NoobAttak : aSkill {

        public NoobAttak(Keys key, int grange=18, int mana_cost=6, int cast_time= 900, int cooldown=0) 
            : base(key, grange, mana_cost, cast_time, cooldown) {
            using_time = 1000;
        }
        public override void Run(Action do_after) {
            if (!b_started) {
                StartCast();
                last_use = DateTime.Now;
            } else {
                if (ui.curr_map.danger == 0 || last_use.AddMilliseconds(using_time) < DateTime.Now) {
                    StopCast();
                    do_after?.Invoke();
                }
            }
        }
    }
  
    public class Cyclone : aSkill { 
        public Cyclone(Keys _key) : base(_key, 18, 3, 9, 0) {
            using_time = 1000;
        }
        public override void Run(Action do_after) {
            if (!b_started) {
                StartCast();
                last_use = DateTime.Now;
            }
            else {
                if (ui.curr_map.danger == 0 || last_use.AddMilliseconds(using_time) < DateTime.Now) {
                    StopCast();
                    do_after?.Invoke();
                }
            }
        }
    }
    public class OrbOfStorms : aOneHitSkill { 
        public OrbOfStorms(Keys key, int grange = 18, int mana_cost = 5, int cast_time = 500, int cd = 500)
            : base(key, grange, mana_cost, cast_time, cd) {
            radius = 30;
        }
  
    }
   
    public class Vortex : aOneHitSkill { 
        public Vortex(Keys _key) : base(_key, 1, 34, 0, 1007 ) {
            name = "frost_bolt_nova";
            radius = 20;
            me.GetComp<Actor>(out var act);
            if (act == null)
                return;
            skill = act.actor_skills.FirstOrDefault(s => s.Name == name);
            if (skill == null) {
                ui.AddToLog(tName + "not found in " + key + " slot -> check in game", MessType.Warning);
            }
        }
        public override bool b_on_cooldown => skill == null ? true : skill.IsOnCooldown;
        //public override bool b_on_cooldown {
        //    get {
        //        if (last_use == DateTime.MinValue)
        //            return false;
        //        var cd_factor = Math.Max(1010, using_time);
        //        return last_use.AddMilliseconds(cd_factor) > DateTime.Now;
        //    }
        //}

    }
   
    public class RighteousFire : aOneHitSkill { //"vaal_righteous_fire_duration"
        public RighteousFire(Keys _key) : base(_key, 34, 0, 0, 0) {

        }
   
    }
    public class VaalRF : aOneHitSkill { //"vaal_righteous_fire_duration"
        public VaalRF(Keys _key) : base(_key,1,0,0,0 ) {
          
        }
    }
}
