using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stas.GA {
    public abstract class aTrap:aSkill {
        public aTrap(Keys key, int grange, int mana_cost, int cast_time, int cooldown)
            : base(key, grange, mana_cost, cast_time, cooldown) {

        }
        public override void Run(Action do_after) {
            HittingUntilNeedStop(do_after);
        }
    }
    public class LightningTrap : aTrap {
        public LightningTrap(Keys key, int grange = 75, int mana_cost = 15, int cast_time = 650, int cooldown = 0)
            : base(key, grange, mana_cost, cast_time, cooldown) {
        }
    }
    public class ExsanguinateTrap : aTrap {//"player_glacial_cascade"
        public ExsanguinateTrap(Keys key, int grange = 75, int mana_cost=1, int cast_time=740, int cooldown=0)
            : base(key, grange, mana_cost, cast_time, cooldown) {
        }
    }
  public class FlameThrowerTrap : aOneHitSkill {
        public FlameThrowerTrap(Keys key, int grange = 80, int mana_cost = 31, int cast_time = 780, int cooldown = 5330)
            : base(key, grange, mana_cost, cast_time, cooldown) {
            b_can_pull = true;
            name = "frost_bomb";
            me.GetComp<Actor>(out var _act);
            Debug.Assert(_act != null);
            skill = _act.actor_skills.FirstOrDefault(s => s.Name == name);
            radius = 30;
        }

    }
    public class SeismicTrap : aOneHitSkill {
        public SeismicTrap(Keys key, int grange = 80, int mana_cost = 4, int cast_time = 500, int cooldown = 5330)
            : base(key, grange, mana_cost, cast_time, cooldown) {
            b_can_pull = true;
            name = "frost_bomb";
            me.GetComp<Actor>(out var _act);
            Debug.Assert(_act != null);
            skill = _act.actor_skills.FirstOrDefault(s => s.Name == name);
            radius = 30;
        }

    }
}
