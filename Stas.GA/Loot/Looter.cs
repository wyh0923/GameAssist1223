using System.Collections.Concurrent;
using System.Diagnostics;
using V2 = System.Numerics.Vector2;
namespace Stas.GA;

public partial class Looter:IDisposable {
    public string fname => @"loot_settings.sett";
    public LootSettings sett;
    public string debug_info= "Looter info";
    int loot_w8 = 300;
    Stopwatch sw = new Stopwatch();
    List<double> elaps = new List<double>();
    HashSet<long> bad_labels = new HashSet<long>();
    
    /// <summary>
    /// static loot cash
    /// </summary>
    public ConcurrentDictionary<uint, LootMapItem> loot_items  = new ConcurrentDictionary<uint, LootMapItem>();
    int loot_dist = 60;
    public Looter() {
        LoadSett();
    //TODO loot here
    }
    List<uint> frame_keys = new ();
    List<string> item_i_need = new();
    List<string> chaos_set = new();
    Dictionary<string, bool> currency = new();
    int qms = 18; //question mark icon size
    (string, int) GetCurrencyIcon(Loot loot) {
        if(ui.ninja.prices.Count == 0) {
            ui.AddToLog("GetLootIcon err: prices.Count == 0");
            return ("question_mark", qms);
        }
        if(loot.BaseName == null) {
            ui.AddToLog("GetLootIcon err: BaseName==null");
            return ("question_mark", qms);
        }
        if (!ui.ninja.prices.ContainsKey(loot.BaseName)) {
            if (loot.BaseName.EndsWith("Shard")) {
                var sb = loot.BaseName.Split(' ')[0];
                switch (loot.BaseName) {
                    case "Chaos Shard":
                        return GetIconByPrice(loot.stack_size * 1f / 20);
                    case "Alchemy Shard":
                    case "Transmutation Shard":
                    case "Alteration Shard":
                    case "Binding Shard":
                        var sv = ui.ninja.prices["Orb of " + sb][0].value;
                        return GetIconByPrice(loot.stack_size * sv / 20);
                    case "Regal Shard":
                    case "Engineer's Shard":
                        sv = ui.ninja.prices[sb + " Orb"][0].value;
                        return GetIconByPrice(loot.stack_size * sv / 20);
                    case "Horizon Shard":
                        sv = ui.ninja.prices["Orb of Horizons"][0].value;
                        return GetIconByPrice(loot.stack_size * sv / 20);
                    default:
                        ui.AddToLog("unknow Shard base==" + sb);
                        return ("question_mark", qms);
                }
            } else if (loot.BaseName == "Chaos Orb")
                return GetIconByPrice(loot.stack_size * 1f);
            else {
                ui.AddToLog("Get price err for:" + loot.BaseName);
                return ("question_mark", qms);
            }
        }
        var price = ui.ninja.prices[loot.BaseName];
        Debug.Assert(loot.ClassName != null);
        float val = 0f;
        switch(loot.ClassName) {
            case "StackableCurrency":
                val = loot.stack_size * price[0].value;
                break;
            default:
                ui.AddToLog("unknow ClassName==" + loot.ClassName);
                break;
        }
        return GetIconByPrice(val);
    }
    public (string, int) GetIconByPrice(float val) {
        var ex = ui.ninja.exa_rate;
        var dr = ui.ninja.divine_rate;
        var alch = ui.ninja.alchemy_rate;
        Debug.Assert(ex != 0 && dr!=0 &&alch != 0);
        if(val >= 3f * ex) //3ex+
            return ("mirror", 26);
        if(val < 3f * ex && val >= 1f * ex)  //1-3ex
            return ("currency0", 23);
        if(val < 1 * ex  && val >= dr) //  div-ex
            return ("currency1", 18);
        if( val < dr && val >= 5) //5с - div
            return ("currency2", 15);
        if(val < 5 && val >=3) // 3-5 chaos
            return ("currency3", 12);
        if(val < 3f && val >= 1) // 1-3 chaos
            return ("currency4", 10);
        if(val < 1 && val >= alch) // orb alchemy
            return ("currency5", 8);
        if(val < alch && val > 0f)
            return ("currency6", 8);
        return ("question_mark", qms);
    }
   

    public void AreaChanged() {
        loot_items.Clear();
        bad_labels.Clear();
    }

    public void Dispose() {
       
    }
}
