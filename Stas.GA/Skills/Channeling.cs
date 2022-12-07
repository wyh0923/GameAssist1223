using System;
using System.Linq;

namespace Stas.GA {
    public interface iCorpsesGenerator {
        public bool b_corpses_generator {get;}
    }
    public interface iChannelling {
        public bool b_channelling { get; }
    }
    public abstract class aChannelling : aSkill, iChannelling {
        public bool b_channelling => true;
        public aChannelling(Keys key, int grange, int mana_cost, int cast_time, int cooldown)
           : base(key, grange, mana_cost, cast_time, cooldown) {

        }
    }
    public class WinterOrbCorpseGenerator : WinterOrb, iCorpsesGenerator {
        public WinterOrbCorpseGenerator(Keys _key, int gdist = 70, int cost = 7, int cast_time = 130,
            int cooldown = 0) : base(_key, gdist, cost, cast_time, cooldown) {
        }

        public bool b_corpses_generator => true;

    }
    public class WinterOrb : aChannelling { //"winter_orb"
        string buff_name = "frost_fury_stage";
        public WinterOrb(Keys _key, int gdist = 70, int cost = 7, int cast_time = 130, int cooldown = 0)
         : base(_key, gdist, cost, cast_time, cooldown) {
            b_can_pull = true;
            name = "winter_orb";
        }
        public int stage {
            get {
                if(ui.me == null || !ui.me.IsValid)
                    return 0;
                if(ui.me.buffs.StatusEffects.ContainsKey(buff_name))
                    return ui.me.buffs.StatusEffects[buff_name].Charges;
                return 0;
            }
        }

        public override void Run(Action do_after) {
            if ( MustBeStop()) {
                StopCast();
                do_after?.Invoke();
                return;
            }
            if (!b_started && stage < 8) {
                StartCast();
                return;
            }
        }
     
    }
    public class DesecrateChanneling : aChannelling, iCorpsesGenerator {
        public DesecrateChanneling(Keys key, int grange=30, int mana_cost=6, int cast_time=200)
         : base(key, grange, mana_cost, cast_time, 0) {
            b_can_hit = true;
        }

        public bool b_corpses_generator => true;
        public override void Run(Action do_after = null) {
            if (!b_started) {
                StartCast();
                last_use = DateTime.Now;
            } else {
                if (last_use.AddMilliseconds(using_time) < DateTime.Now) {
                    StopCast();
                    do_after?.Invoke();
                }
            }
        }
    }
}
