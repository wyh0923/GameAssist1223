using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;

namespace Stas.GA {
    public abstract class aCry : aBuff {
        public aCry(Keys _key, int _grange, int _mana_cost, int _cast_time, int _cooldown) : base(_key, _grange, _mana_cost, _cast_time, _cooldown) {

        }
        void CheckClose() {
            var close = ui.curr_map.enemy.Where(e => e.gdist_to_me <= grange).ToList();
        }
    }
    public class BattleMageCry : aBuff {
        public BattleMageCry(Keys _key) : base(_key, 60, 24, 430, 6670) {
            buff_name = "divine_cry";
        }
    }
    public class GeneralsCry : aSkill {
        aSkill corpse_gen;
        ChannelingUntilCondition generator;
        bool b_try_get_gen = false;
        public GeneralsCry(Keys key, int grange, int mana_cost, int cast_time, int cooldown)
           : base(key, grange, mana_cost, cast_time, cooldown) {
            b_can_hit = false;
        }
        int corpse_count => ui.entities.Where(e => e.IsDead && e.gdist_to_me < 0.6 * grange).ToList().Count;
        bool corpse_ok() { 
            return corpse_count >= 5;
        } 
        public override void Run(Action do_after) {
            if(corpse_gen == null) { //can't initialize in constructor - ui.worker null there
                corpse_gen = ui.worker.my_skills.FirstOrDefault(s => s is iCorpsesGenerator);
                Debug.Assert(corpse_gen != null);
            }

            if (corpse_count < 5) {
                if(corpse_gen is iChannelling) {
                    generator = new ChannelingUntilCondition(corpse_gen, corpse_ok);
                    ui.tasker.TaskPop(generator);
                    return;
                }
               
            } else {
                OneHit();
            }
        }
    }
    public class EnduringCry : aCry {
        public EnduringCry(Keys _key) : base(_key, 0, 14, 670, 8) {
            radius = 40;
            buff_name = "enduring_cry";
        }
    }
    public class AncestralCry : aCry {
        public AncestralCry(Keys _key) : base(_key, 0, 14, 670, 8) {
            radius = 40;
            buff_name = "ancestral_cry";
        }
    }
    public class RallyingCry : aCry {
        public RallyingCry(Keys _key) : base(_key, 0, 14, 670, 8) {
            radius = 40;
            buff_name = "rallying_cry";
        }
    }

}
