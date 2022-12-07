using System;
using V2 = System.Numerics.Vector2;

namespace Stas.GA {
    public class Blight : aSkill {
        public Blight(Keys _key) : base(_key, 45, 2, 300, 0) {
        }

        public override void Run(Action do_after) {
            LikeChannelling(do_after);
        }
    }
    public class SplittingSteel : aSkill {
        public SplittingSteel(Keys _key) : base(_key, 50, 2, 400, 0) {
        }

        public override void Run(Action do_after) {
            LikeChannelling(do_after);
        }
    }
   
    public class FrostBlades :aSkill {
        public FrostBlades(Keys key, int grange=30, int mana_cost=15, int cast_time=650, int cooldown=0) 
            : base(key, grange, mana_cost, cast_time, cooldown) {
        }

        public override void Run(Action do_after) {
            LikeChannelling(do_after);
        }
    }
   
    //public class LightningStrike :aTarget {
    //       public LightningStrike(Keys _key) : base(_key) {
    //           grange = 30;
    //           intern_name = "lightning_strike";
    //       }

    //       public override void Set(V2 tgp) {
    //           LikeChannelling();
    //       }
    //   }



}
