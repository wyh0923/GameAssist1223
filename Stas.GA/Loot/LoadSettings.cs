namespace Stas.GA;
public partial class Looter {
    public void LoadSett() {
        loot_items.Clear(); //after applying a new filter, the loot_items may change
        item_i_need.Clear();
        chaos_set.Clear();
        currency.Clear();
        sett = new LootSettings().Load<LootSettings>();
        currency["Portal Scroll"] = sett.b_portal;
        currency["Scroll of Wisdom"] = sett.b_wisdom;
        currency["Orb of Transmutation"] = sett.b_transmut;
        currency["Orb of Augmentation"] = sett.b_augment;
        currency["Orb of Alteration"] = sett.b_alter;
        currency["Armourer's Scrap"] = sett.b_arm_scrap;
        currency["Blacksmith's Whetstone"] = sett.b_whetstone;

        if (sett.b_league) item_i_need.Add("ArchnemesisMod");
        if (sett.b_quest) item_i_need.Add("QuestItem");
        if (sett.b_currency) item_i_need.Add("StackableCurrency");
        if (sett.b_maps) item_i_need.Add("Map");
        if (sett.b_small_bow) item_i_need.AddRange(new string[]
            { "Short Bow", "Crude Bow", "Grove Bow", "Thicket Bow" });

        if (sett.b_Claw) chaos_set.Add("Claw");
        if (sett.b_Dagger) chaos_set.Add("Dagger");
        if (sett.b_Wand) chaos_set.Add("Wand");
        if (sett.b_Body_Armour) chaos_set.Add("Body Armour");
        if (sett.b_Boots) chaos_set.Add("Boots");
        if (sett.b_Gloves) chaos_set.Add("Gloves");
        if (sett.b_Helmet) chaos_set.Add("Helmet");
        if (sett.b_Amulet) chaos_set.Add("Amulet");
        if (sett.b_Belt) chaos_set.Add("Belt");
        if (sett.b_Ring) chaos_set.Add("Ring");
    }
}

