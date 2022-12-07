using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Stas.GA {
    public abstract class aTimeBuff :aBuff { //need enemy for get
        public aTimeBuff(Keys key, int grange, int mana_cost, int cast_time, int cooldown) 
            : base(key, grange, mana_cost, cast_time, cooldown) {
          
        }
    }
    public class FrostGlobe : aTimeBuff {
        //Keys.RButton, 8, 28, 320, 5000
        public FrostGlobe(Keys key, int grange = 8, int mana_cost = 28, int cast_time = 320, int cooldown = 5000)
            : base(key, grange, mana_cost, cast_time, cooldown) {
            b_dont_use_in_HO = true;
            buff_name = "frostglobe_absorb_damage";
            name = "FrostGlobe";
            me.GetComp<Actor>(out var _act);
            Debug.Assert(_act != null);
            skill = _act.actor_skills.FirstOrDefault(s => s.Name == name);
            radius = 30;
        }
        public override bool b_on_cooldown => skill == null ? true : skill.IsOnCooldown;
    }
    public class Smite : aSkill {
        public Smite(Keys _key, int grange=30, int mana_cost=18, int cast_time=800  )
            : base(_key, grange, mana_cost, cast_time, 0) {
            name = "???";
        }
        public override void Run(Action do_after) {
            LikeChannelling (do_after);
        }
    }
    public class SmiteBuff : aTimeBuff {
        public SmiteBuff(Keys _key) : base(_key, 30, 12, 820, 0) {
            buff_name = "smite_buff";
        }
    
    }
    public class VigilantStrikeBuff : aTimeBuff {
        public VigilantStrikeBuff(Keys _key) : base(_key, 30, 7, 940, 4000) {
        }
     
    }
   
 
    public class LifeRegenTotem :aTimeBuff { 
        public LifeRegenTotem(Keys _key, int grange = 15, int mana_cost = 9, int cast_time = 510)
            : base(_key, grange, mana_cost, cast_time, 0) {
            buff_name = "totem_aura_life_regen";
            using_time = 1200; //55+ tiks
            b_can_pull = true;
        }
    }
   
    public class BloodRage : aTimeBuff { 
        public BloodRage(Keys _key) : base(_key, 0, 1, 1, 1000) {
            radius = 40;
            buff_name = "blood_rage";
            need_enemy_around = false;
        }
    } 
  
    //class SigilOfPower :aMeleeAOE { //OrbOfStorm
    //    public SigilOfPower(Keys _key) : base(_key) {
    //        intern_name = "circle_of_power";
    //    }
    //    public override void Set(V2 tgp) {
    //        OneHit(tgp);
    //    }
    //}
}
