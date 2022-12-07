using System.Linq;
using System.Diagnostics;
using System;

namespace Stas.GA {
    public abstract class aOneHitSkill : aSkill {
        public aOneHitSkill(Keys key, int grange, int mana_cost, int cast_time, int cooldown)
            : base(key, grange, mana_cost, cast_time, cooldown) {

        }
      
        public override void Run(Action do_after) {
            OneHit(do_after);
        }
    }

    public class Hydrosphere : aOneHitSkill {
        public Hydrosphere(Keys key, int grange = 90, int mana_cost = 4, int cast_time = 500, int cooldown = 2500)
            : base(key, grange, mana_cost, cast_time, cooldown) {
            b_can_pull = true;
            name = "frost_bomb";
            me.GetComp<Actor>(out var _act);
            Debug.Assert(_act != null);
            skill = _act.actor_skills.FirstOrDefault(s => s.Name == name);
            radius = 30;
        }
    }
 
    public class FrostBomb : aOneHitSkill {
        public FrostBomb(Keys key, int grange=90, int mana_cost=4, int cast_time=500, int cooldown=2500)
            : base(key, grange, mana_cost, cast_time, cooldown) {
            b_can_pull = true;
            name = "frost_bomb";
            me.GetComp<Actor>(out var _act);
            Debug.Assert(_act != null);
            skill = _act.actor_skills.FirstOrDefault(s => s.Name == name);
            radius = 30;
        }
        public override bool b_on_cooldown => skill == null ? true : skill.IsOnCooldown;
    }
    public class Desecrate : aOneHitSkill, iCorpsesGenerator {
        public override bool b_on_cooldown => skill == null ? true : skill.IsOnCooldown;
        public bool b_corpses_generator => true;

        public Desecrate(Keys key, int grange = 90, int mana_cost = 11, int cast_time = 340, int cooldown = 0)
            : base(key, grange, mana_cost, cast_time, cooldown) {
            b_can_pull = true;

            name = "desecrate";
            me.GetComp<Actor>(out var _act);
            Debug.Assert(_act != null);
            skill = _act.actor_skills.FirstOrDefault(s => s.Name == name);
            radius = 30;
        }
    }
}
