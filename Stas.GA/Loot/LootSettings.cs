using System.Text.Json.Serialization;
namespace Stas.GA {
    public class LootSettings : iSett {
        [JsonInclude]
        public bool b_quest = true;
        [JsonInclude]
        public bool b_league = true;
        [JsonInclude]
        public bool b_portal = true;
        [JsonInclude]
        public bool b_wisdom = true;
        [JsonInclude]
        public bool b_transmut = true;
        [JsonInclude]
        public bool b_augment = true;
        [JsonInclude]
        public bool b_alter = true;
        [JsonInclude]
        public bool b_arm_scrap = true;
        [JsonInclude]
        public bool b_whetstone = true;
        [JsonInclude]
        public bool b_maps = true; 
        [JsonInclude]
        public bool b_currency;
        [JsonInclude]
        public bool b_Claw;
        [JsonInclude]
        public bool b_Dagger;
        [JsonInclude]
        public bool b_Wand;
        [JsonInclude]
        public bool b_Body_Armour;
        [JsonInclude]
        public bool b_Boots;
        [JsonInclude]
        public bool b_Gloves;
        [JsonInclude]
        public bool b_Helmet;
        [JsonInclude]
        public bool b_Amulet;
        [JsonInclude]
        public bool b_Belt;
        [JsonInclude]
        public bool b_Ring;
        [JsonInclude]
        public bool b_small_bow;
        [JsonInclude]
        public bool b_6s_big;
        [JsonInclude]
        public bool b_6s_small;
        [JsonInclude]
        public bool b_6l_any;
    }
    //"Claw", "Dagger", "Wand", "Body Armour", "Boots", "Gloves", "Helmet", "Amulet", "Belt", "Ring"
    /*
Portal Scroll
Scroll of Wisdom
Orb of Transmutation
Orb of Augmentation
Orb of Alteration
Armourer's Scrap
Blacksmith's Whetstone	
     */
}
