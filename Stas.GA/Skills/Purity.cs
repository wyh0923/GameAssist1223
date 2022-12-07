namespace Stas.GA {
    public class Purity_of_elements : aBuff {
        public Purity_of_elements(Keys _key) : base(_key, 0, 0, 0, 1200) {
            radius = 26;
            buff_name = "player_aura_resists";
        }

    }
    public class Purity_of_lightning : aBuff {
        public Purity_of_lightning(Keys _key) : base(_key, 0, 0, 0, 1200) {
            radius = 26;
            buff_name = "player_aura_lightning_resist";
        }
    }
    public class Purity_of_ace : aBuff {
        public Purity_of_ace(Keys _key) : base(_key, 0, 0, 0, 1200) {
            radius = 26;
            buff_name = "player_aura_cold_resist";
        }
    }
    public class Purity_of_fire : aBuff {
        public Purity_of_fire(Keys _key) : base(_key, 0, 0, 0, 1200) {
            radius = 26;
            buff_name = "player_aura_fire_resist";
        }
    }
}
