using V2 = System.Numerics.Vector2;
using V3 = System.Numerics.Vector3;
using sh = Stas.GA.SpriteHelper;
namespace Stas.GA; 

public partial class AreaInstance {
    MapItem asStaticMapItem(Entity e, miType _mit, MapIconsIndex ii, string info = null,
            IconPriority preorety = IconPriority.High, Remnant remn = null) {
        var key = e.GetKey;
        if (static_items.TryGetValue(key, out var old)) {
            //old.ent.Id can be != current e.Id,
            //if old was not valid(faar distance) and reload new one
            old.ent = e;
            old.version += 1;
            if (info != null)
                old.info = info;
            return null;
        }

        var smi = new StaticMapItem(e, _mit);
        if (info != null)
            smi.info = info;
        else {
            var _d = e.pos.GetDistance(ui.me.pos).ToRoundStr(0);
            smi.info = e.eType + " id=" + e.id + " n=" + e.RenderName;
        }
        smi.mii = ii;
        smi.priority = preorety;
        smi.uv = sh.GetUV(ii);
        smi.remn = remn;
        static_items[smi.key] = smi;
        return null;
    }
}
