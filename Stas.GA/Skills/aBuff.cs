using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Stas.GA {
    public abstract class aBuff : aOneHitSkill {
        public bool need_enemy_around=false;
        public bool b_dont_use_in_HO = false;
        public aBuff(Keys _key, int _grange, int _mana_cost, int _cast_time, int _cooldown)
            : base(_key, _grange, _mana_cost, _cast_time, _cooldown) {
            using_time = 100; //time for detect a aura only
            b_can_hit = false;
        }
        public string buff_name { get; protected private set; }
        //TODO need check here
        public virtual bool b_already_have {
            get {
                if (!ui.me.IsValid || ui.me.buffs == null)
                    return true; //for not cheking
                return ui.me.buffs.StatusEffects.ContainsKey(buff_name);
            }
        }
        public override bool b_on_cooldown {
            get {
                //we need use ranfom delay here for not use after cd same time
                var delay = R.Next(30, 60);
                if (this is aBuff)
                    delay = R.Next(50, 250);
                var cd_factor = Math.Max(cooldown + delay, using_time);
                return last_use.AddMilliseconds(cd_factor) > DateTime.Now;
            }
        }
        public override bool b_ready {
            get {
                return !b_already_have && !b_on_cooldown;
            }
        }

        Stopwatch sw = new Stopwatch();
        bool started = false;
        public void how_to_detect() { //used for debug only
            if(started  && b_already_have) {
                var need = sw.Elapsed.TotalMilliseconds.ToRoundStr(0);
                ui.AddToLog(tname+ " ms_to_be_detected="+ need);
                started = false;
            }

            if(Keyboard.b_Try_press_key(Keys.F1,"HKP F1", 900) && !started) {
                ui.log.Clear();
                Keyboard.KeyPress(key);
                started = true;
                sw.Restart();
            }
        }
        public override string ToString() {
            return tname + " ready=" + b_ready + " have="+b_already_have+" oncd=" + b_on_cooldown;
        }
    }
   
    public class Spellsinger : aBuff {
        public Spellsinger(Keys _key) : base(_key, 0, 0, 0, 600) {
            buff_name = "spellslinger";
        }
    }
    public class Wrath :aBuff {
        public Wrath(Keys _key) : base(_key, 0, 0, 0, 1000) {
            buff_name = "player_aura_lightning_damage";
        }
    }
    public class Hatred :aBuff {
        public Hatred(Keys _key) : base(_key, 0, 0, 0, 1200) {
            radius = 26;
            buff_name = "player_aura_cold_damage";
        }

    }
    public class Anger :aBuff {
        public Anger(Keys _key) : base(_key, 0, 0, 0, 1200) {
            radius = 26;
            buff_name = "player_aura_fire_damage";
        }
    }
    public class Malovalence : aBuff {
        public Malovalence(Keys _key) : base(_key, 0, 0, 0, 1200) {
            radius = 26;
            buff_name = "player_aura_damage_over_time";
        }
    }
    public class Grace :aBuff {
        public Grace(Keys _key) : base(_key, 0, 0, 0, 1200) {
            radius = 26;
            buff_name = "player_aura_evasion";
        }
    }
   
    public class ArcticArmour : aBuff {
        public ArcticArmour(Keys _key) : base(_key, 1, 0, 0, 1000) { //19.8f
            buff_name = "new_arctic_armour";
        }
   
    }
    public class DespairAura : aBuff { 
        public DespairAura(Keys _key) : base(_key, 1, 0, 0, 1200) {
            buff_name = "curse_chaos_weakness";
        }
    }
    public class SummonHolyRelic : aBuff {  //no itea how to detect it
        public SummonHolyRelic(Keys _key) : base(_key, 1, 0, 0, 1000) {
            buff_name = "player_aura_life_regen";
        }
    
    } 
    public class SummonSkitterbots : aBuff { 
        public SummonSkitterbots(Keys _key) : base(_key, 1, 0, 0, 1000) {
            buff_name = "skitterbots_buff";
        }
    } 

    public class BloodEndSand : aBuff {
        public BloodEndSand(Keys _key) : base(_key, 0, 0, 0, 1000) {
            radius = 26;
            buff_name = "blood_stance";
        }
    }
    public class DreadBanner : aBuff {
        public DreadBanner(Keys _key) : base(_key, 0, 0, 0, 1000) {
            radius = 26;
            buff_name = "puresteel_banner_buff_aura";
        }
    }

    public class DefianceBaner :aBuff {
        public DefianceBaner(Keys _key) : base(_key, 0, 0, 0, 1000) {
            radius = 26;
            buff_name = "armour_evasion_banner_buff_aura";
        }
    }
    public class Determination :aBuff {
        public Determination(Keys _key) : base(_key, 0, 0, 0, 1200) {
            radius = 26;
            buff_name = "player_aura_armour";
        }
    }
    public class Precision :aBuff {
        public Precision(Keys _key) : base(_key, 0, 0, 0, 1200) {
            radius = 26;
            buff_name = "player_aura_accuracy_and_crits";
        }
    }
    public class Zealotry :aBuff {
        public Zealotry(Keys _key) : base(_key, 0, 0, 0, 1200) {
            radius = 26;
            buff_name = "player_aura_spell_damage";
        }
    }
    
    public class PetrifiedBlood : aBuff { 
        public PetrifiedBlood(Keys _key) : base(_key, 0, 0, 0, 1000) {
            buff_name = "petrified_blood";
        }
    } 
  
    public class Haste :aBuff {
        public Haste(Keys _key) : base(_key, 0, 0, 0, 1200) {
            radius = 26;
            buff_name = "player_aura_speed";
        }
       
    }
    public class Discipline :aBuff {
        public Discipline(Keys _key) : base(_key, 0, 0, 0, 1200) {
            radius = 26;
            buff_name = "player_aura_energy_shield";
        }
     
    }
    public class FleshAndStone : aBuff {
        public FleshAndStone(Keys _key) : base(_key, 0, 0, 0, 1200) {
            radius = 26;
            buff_name = "sand_armour";
        }
    }
    public class MoltenShell : aBuff {//molten_shell
        public MoltenShell(Keys _key, int grange = 0, int mana_cost = 9)//, int cooldown=4000
            : base(_key, 0, 9, 0, 4000) {
            radius = 26;
            buff_name = "asdasd";
        }
    }
    public class Vitality :aBuff {
        public Vitality(Keys _key) : base(_key, 0, 0, 0, 1200) {
            radius = 26;
            buff_name = "player_aura_life_regen";
        }
    }
    public class Pride : aBuff {
        public Pride(Keys _key) : base(_key, 0, 0, 0, 1200) {
            radius = 26;
            buff_name = "player_physical_damage_aura";
        }
       
    }
    public class WarBanner : aBuff {
        public WarBanner(Keys _key) : base(_key, 0, 0, 0, 1200) {
            radius = 26;
            buff_name = "bloodstained_banner_buff_aura";
        }
    }
  public class TempestShield : aBuff {
        public TempestShield(Keys _key) : base(_key, 0, 0, 0, 1200) {
            radius = 26;
            buff_name = "lightning_shield";
        }
    }
    public class Clarity :aBuff {
        public Clarity(Keys _key) : base(_key, 0, 0, 0, 1200) {
            radius = 26;
            buff_name = "player_aura_mana_regen";
        }
    }
   
   
  
}
