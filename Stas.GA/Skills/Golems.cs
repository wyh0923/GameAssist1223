namespace Stas.GA {
    public class LightningGolem : aBuff {
        public LightningGolem(Keys _key) : base(_key, 0, 34, 880, 6000) {
            radius = 26;
            buff_name = "lightning_elemental_buff";
        }
    }
    public class StoneGolem : aBuff {
        public StoneGolem(Keys _key) : base(_key, 0, 34, 880, 6000) {
            radius = 26;
            buff_name = "rock_golem_buff";
        }
    }
    public class summon_bone_golem : aBuff {
        public summon_bone_golem(Keys _key) : base(_key, 0, 34, 880, 6000) {
            radius = 26;
            name = "summon_bone_golem";
            buff_name = "bone_golem_buff";
            //me.GetComp<Actor>(out var act);
            //askill = act.ActorSkills.FirstOrDefault(s => s.InternalName == internal_name);

        }
    }
}
