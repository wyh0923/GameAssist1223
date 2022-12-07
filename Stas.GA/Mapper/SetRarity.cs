#region using
using sh = Stas.GA.SpriteHelper;
#endregion

namespace Stas.GA {
    public partial class AreaInstance  {
        void SetRarity(MapItem nmi) {
            switch (nmi.ent.rarity) {
                case Rarity.Normal:
                    nmi.priority = IconPriority.Low;
                    nmi.size = ui.sett.icon_size;
                    nmi.uv = sh.GetUV(MapIconsIndex.LootFilterLargeRedCircle); //LootFilterLargeCyanCircle
                    break;
                case Rarity.Magic:
                    nmi.priority = IconPriority.Medium;
                    nmi.size = ui.sett.icon_size + 2;
                    nmi.uv = sh.GetUV(MapIconsIndex.LootFilterLargeBlueCircle);
                    break;
                case Rarity.Rare:
                    nmi.priority = IconPriority.High;
                    nmi.size = ui.sett.icon_size + 4;
                    nmi.uv = sh.GetUV(MapIconsIndex.LootFilterLargeYellowCircle);
                    break;
                case Rarity.Unique:
                    nmi.priority = IconPriority.Critical;
                    nmi.size = ui.sett.icon_size + 6;
                    nmi.uv = sh.GetUV(MapIconsIndex.LootFilterLargePurpleCircle);
                    break;
                default:
                    ui.AddToLog("SetRarity err: " + nmi.ent.rarity);
                    break;
                    //throw new NotImplementedException();
            }
        }
    }
}
