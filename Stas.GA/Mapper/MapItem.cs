using System;
using System.Diagnostics;
using System.Drawing;
using System.Numerics;

namespace Stas.GA {
    public abstract class aMapItem {
        public RectangleF uv;
        /// <summary>
        /// icon size
        /// </summary>
        public float size;
        public IconPriority priority;
        public string info;
        public Entity ent;
        public Vector3 pos { get; protected private set; }
        public MapIconsIndex mii;

        public aMapItem(Entity _ent) {
            ent = _ent;
            size = ui.sett.icon_size;
        }

        public override string ToString() {
            return info;
        }
    }
    public class LootMapItem : StaticMapItem {
        public Loot loot { get; private set; }
  
        public LootMapItem(Loot _loot) :base(_loot.ent, miType.loot) {
            loot = _loot;
            info = _loot.ToString();
            ent = loot.ent; 
            pos = ent.pos;
            gpos = ent.gpos;
            key = _loot.ent.GetKey;
        }
        public void Update(LabelOnGround _log) {
            loot.log = _log;
            ent = loot.ent;
            pos = ent.pos;
            gpos = ent.gpos;
            if (key != ent.GetKey) {
                ui.AddToLog("LootMapItem.update wrong key...", MessType.Error);
            }
        }
    }

    public enum miType {
        regular, transit, door, portal, IncursionPortal, @switch, Harvest, Ultimatum, Ritual, Sulphite, Blight,
        ExpedDeton, ExpedMarker, ExpedRemnant, ExpedArtifact, PartyMember, Stash, GuildStash, Chest, Barrel, loot,
        NPC, Archnemesis,
        waypoint,
        Quest, BlightPath,
        HoDecor, LeagueEvent,
        DynamiteWall
    }
    public class StaticMapItem :aMapItem {
        public Remnant remn;
        public int version = 0;
        public bool b_done;
        public miType m_type;
        public Vector2 gpos { get; protected private set; }
        public float gdist_to_me => gpos.GetDistance(ui.me.gpos);
        /// <summary>
        /// cold be only for static items coz based on static ent pos
        /// </summary>
        public uint key { get; protected private set; }
        public StaticMapItem(Entity _ent, miType _mt, string _info=null ) :base(_ent){
            info = _info==null?_mt.ToString():_info;
            gpos = _ent.gpos;
            pos = _ent.pos;
            m_type = _mt;
            size = 16;
            key = _ent.GetKey;
        }
        public bool WasDeleted() {
            switch (m_type) {
                case miType.Archnemesis:
                    if ((ent.IsValid && ent.IsDead) || !ent.IsValid) //
                        return ui.curr_map.static_items.TryRemove(key, out _);
                    break;
                case miType.IncursionPortal:
                    if (ent.GetComp<MinimapIcon>(out var icon) && icon.IsHide) {
                        return ui.curr_map.static_items.TryRemove(key, out _);
                    }
                    break;
                case miType.Sulphite:
                case miType.portal:
                    if (!ent.IsTargetable)
                        return ui.curr_map.static_items.TryRemove(key, out _);
                    break;
                case miType.Chest:
                default:
                    break;
            }
            return false;
        }
        public override string ToString() {
            string add = "";
            if (ui.me != null)
                add = " d=" + (pos.GetDistance(ui.me.pos) * ui.worldToGridScale).ToRoundStr(0);
            return m_type.ToString() + add;
        }
    }

    public class MapItem : aMapItem {
        public MapItem(BotInfo bi) : base(bi.ent) {
            pos = bi.pos;
            info = bi.bot_name;
        }
        public MapItem(Entity _ent, string _info) : base(_ent) {
            pos = _ent.pos;
            this.info = _info; //save intitialise ent value
        }

        public override string ToString() {
            string add = "";
            if (ui.me != null)
                add = " d=" + (pos.GetDistance(ui.me.pos) * ui.worldToGridScale).ToRoundStr(0);
            if (ent != null && ent.IsValid)
                add += " dang_rt=" + ent.danger_rt;
            // text + " "+ pos.ToIntString() + "["+uv.ToString()+"] "+size;
            return info + add;
        }
    }

    public enum IconPriority : byte{
        Low,
        Medium,
        High,
        VeryHigh,
        Critical,
        Debug
    }
}
