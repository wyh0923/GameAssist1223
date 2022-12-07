using System;
using System.Diagnostics;
using System.Threading;
using V2 = System.Numerics.Vector2;

namespace Stas.GA {
    public class aMine : aSkill {
        protected int min_hit = 1;
        protected int max_hit = 3;
        public const int mrange = 69;
        public aMine(Keys _key, int grange, int mcost, int _cast_time, int cooldown, int _min = 1, int _max = 2)
            : base(_key, grange, mcost, _cast_time, cooldown) {
            min_hit = _min;
            max_hit = _max;
            using_time = cast_time * R.Next(min_hit, max_hit) + R.Next(40, 120);
        }
        DateTime last_detonate = DateTime.MinValue;
        public override void Reset() {
            last_detonate = DateTime.MinValue;
            base.Reset();
        }
        public override void Run(Action do_after) {
            if (ui.gui.SkillBar.detonate.IsVisible
                && last_detonate.AddMilliseconds(50 + R.Next(0, 60)) < DateTime.Now) {
                last_detonate = DateTime.Now;
                Keyboard.KeyPress(Keys.D);
            }
            if (!b_started) {
                //Debug.Assert(ui.gc_ui.GameUI.D);
                Keyboard.KeyDown(key);
                last_use = DateTime.Now;
            } else {
                if (DateTime.Now > last_use.AddMilliseconds(using_time) || b_must_stop) {
                    StopCast();
                    do_after?.Invoke();
                }
            }
        }
    }

    public class NoobMine : aMine {
        public NoobMine(Keys key, int grange = mrange, int mana_cost = 5, int cast_time = 650, int cooldown = 0)
            : base(key, grange, mana_cost, cast_time, 0) { }
    }
    public class StormBlastMine : aMine {
        public StormBlastMine(Keys key, int grange = mrange, int mana_cost = 5, int cast_time = 650, int cooldown = 0)
            : base(key, grange, mana_cost, cast_time, cooldown) { }
    }


    public class ReapMine : aMine {
        public ReapMine(Keys _key) : base(_key, mrange, 1, 550, 0) {
        }
    }
    public class IcicleMine : aMine {
        public IcicleMine(Keys _key, int grange = mrange, int mcost = 3, int cast_time = 690, int cooldown = 1)
            : base(_key, grange, mcost, cast_time, cooldown) {
        }
    }
    public class ToxicRaneMine : aMine {
        public ToxicRaneMine(Keys _key) : base(_key, mrange, 14, 650, 0) {
        }
    }

    public class ExsanguinateMine : aMine {//"player_glacial_cascade"
        public ExsanguinateMine(Keys _key, int grange = mrange, int mcost = 1, int cast_time = 690, int cooldown = 1)
            : base(_key, grange, mcost, cast_time, cooldown) {
        }
    }
    public class GlacialCcascadeMine : aMine {//"player_glacial_cascade"
        public GlacialCcascadeMine(Keys _key, int grange = mrange, int mcost = 15, int cast_time = 270)
            : base(_key, grange, mcost, cast_time, 1) {
        }
    }
    public class ShtormblastMine : aMine {
        public ShtormblastMine(Keys _key) : base(_key, mrange, 2, 610, 0) {
        }
    }
}