using System;
using System.Diagnostics;

namespace Stas.GA {
    public partial class aMark : aCurse {
        public aMark(Keys key, int grange, int mana_cost, int cast_time) 
            : base(key, grange, mana_cost, cast_time) {
        }
    }
    public class SnipersMark : aMark {
        public SnipersMark(Keys key, int grange = 80, int mana_cost = 30, int cast_time = 270)
            : base(key, grange, mana_cost, cast_time) {
            curse_name = "curse_snipers_mark";
        }
    }
    public class AssasianMark : aMark {
        public AssasianMark(Keys key, int grange = 80, int mana_cost = 30, int cast_time = 270)
            : base(key, grange, mana_cost, cast_time) {
            curse_name = "AssassinsMark";
        }
    }
    public abstract class aCurse : aSkill {
        public string curse_name { get; protected private set; }
        //TODO not used now at all
        public int detect_time { get; protected private set; }
        public override void Reset() {
            base.Reset();
        }
        public override void Run(Action do_after) {
            OneHit(do_after);
        }
      
        public aCurse(Keys key, int grange, int mana_cost, int cast_time)
           : base(key, grange, mana_cost, cast_time, 0) {
         
        }
    }
    public class BurningArrow : aCurse { 
        public BurningArrow(Keys key, int grange = 80, int mana_cost = 5, int cast_time = 790) 
            : base(key, grange, mana_cost, cast_time) {
            curse_name = "curse_snipers_mark";
        }
    }
  
}
