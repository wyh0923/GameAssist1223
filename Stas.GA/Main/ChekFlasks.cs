using V2 = System.Numerics.Vector2;
using V3 = System.Numerics.Vector3;
namespace Stas.GA;

public partial class ui {
    static void CheckFlasks(bool debug = true) {
       
        var fi = server_data.FlaskInventory;
       
        
        //var mana = fi?.Items.FirstOrDefault(i => i.Path != null && i.Path.Contains("FlaskMana"));
        //if (mana != null && mana.GetComp<Charges>(out var mana_ch)) {
        //    if (debug)
        //        AddToLog("mana=[" + mana_ch.NumCharges + "]");
        //}
        //if (worker != null && !worker.b_use_low_life) {
        //    var life = flasks?.Items.FirstOrDefault(i => i.Path != null && i.Path.Contains("FlaskLife"));
        //    if (life != null && life.GetComp<Charges>(out var life_ch)) {
        //        life.GetComp<Flask>(out var life_flask);
        //        if (debug)
        //            AddToLog("life=[" + life_ch.NumCharges + "]");
        //    }
        //}
    }
}
