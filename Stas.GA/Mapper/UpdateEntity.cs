﻿using System.Collections.Concurrent;
using System.Diagnostics;
using sh = Stas.GA.SpriteHelper;
using V2 = System.Numerics.Vector2;
using V3 = System.Numerics.Vector3;
namespace Stas.GA;
public partial class AreaInstance  {
    SW sw_elist = new SW("Make ent list");
    SW sw_ue = new SW("Ent update");
    Stopwatch sw_etm = new Stopwatch();
    List<double> sw_etm_elapsed = new List<double>();
    void UpdateEntities(StdMap ePtr, bool addToCache = true) {
        FrameClear();
        sw_ue.Restart();

        var entities = ui.m.ReadStdMapAsList<EntityNodeKey, EntityNodeValue>(ePtr, EntityFilter.IgnoreVisualsAndDecorations);
        if (ui.b_contrl && ui.sett.b_draw_misk && ui.sett.b_develop)
            entities = ui.m.ReadStdMapAsList<EntityNodeKey, EntityNodeValue>(ePtr, null);
        //entities = entities.OrderBy(s => s.Key.id).ToList();
        //sw_cash.Print("ReadStdMapAsList");
        sw_ue.Restart();
        var data = AwakeEntities;
        TryRemoveOldEntyty();
        TryGetEntToDebug();
        if (mi_debug != null)
            return;
        sw_elist.Restart();
        var e_added = 0;
        sw_etm_elapsed.Clear();

        var ent_lodas = new List<double>();
#if DEBUG
        for (var i = 0; i < entities.Count; i++) {//for step-by-step debugging use here
            calc(i);
        }
#else
        Parallel.For(0, entities.Count, (i, state) => { //use for prodaction here
            calc(i);
        });
#endif
        void calc(int i) {
            var (key, value) = entities[i];
            if (key.id == ui.me.id) {
                frame_items.Add(AddMapItem(ui.me));
                return;
            }
            if (data.TryGetValue(key, out var e)) {
                e.Tick( value.EntityPtr);
            }
            else {
                e = new Entity(value.EntityPtr);
                e.Tick(value.EntityPtr);
                e_added += 1;
                if (!string.IsNullOrEmpty(e.Path)) {
                    data[key] = e;
                    if (addToCache) {
                        AddToCacheParallel(key, e.Path);
                    }
                }
                else {
                    e = null;
                }
            }

            if (e != null) {
                if (e.eType == eTypes.Useless && !ui.b_contrl)
                    return;
                if (frame_di != null)
                    return;
                sw_etm.Restart();
                Debug.Assert(e.eType != eTypes.Unidentified);
                var nmi = AddMapItem(e);//new map item
                sw_etm_elapsed.Add(sw_etm.Elapsed.TotalMilliseconds);
                if (nmi != null) {
                    frame_items.Add(nmi);
                    SetDanger(e);
                }
            }
        }
        GetBlight();
        //GetExped();
        GetParty();
        //ui.AddToLog("ent to mi time=[" + sw_etm_elapsed.Sum().ToRoundStr(5) + "]ms ");
        //sw_elist.Print("e_added=[" + e_added + "] new/old=[" + entities.Count + "/" + data.Count + "]");
        triggers = new ConcurrentBag<Cell>(frame_trigger);//рисуются отдельно
        var sorted = frame_items.OrderBy(e => e.ent.id).ToList();
        map_items = new ConcurrentBag<MapItem>(sorted);
        debug_info = ("ent=[" + data.Count + "/" + entities.Count + "/" + map_items.Count + "]");

        void TryRemoveOldEntyty() {
            if (ui.b_contrl)
                return;
            foreach (var kv in data) {
                var e = kv.Value;
                var so_faar = e.CanMoove && e.gdist_to_me > ui.sett.max_entyty_valid_gdistance;
                var is_dead = e.eType == eTypes.Monster && e.IsDead;
                if (!e.IsValid || so_faar || is_dead) { //dont delete misk etc for prevent remake it
                    var done = AwakeEntities.TryRemove(kv.Key, out _);
                    if (!done) {
                        ui.AddToLog("cant delete ent from cash", MessType.Error);
                    }
                }
            }
        }
    }
    void GetParty() {
        foreach (var b in ui.bots.Where(b => b.map_hash == ui.curr_map_hash)) {
            var bot_icon = MapIconsIndex.PartyMember;
            if (b.b_i_died)
                bot_icon = MapIconsIndex.I_died;
            var mi = new MapItem(b);
            mi.uv = sh.GetUV(bot_icon);
            frame_items.Add(mi);
        }
    }
    void TryGetEntToDebug() {
        if (ui.b_contrl || ui.b_alt) {//try pick debug entyty from last draw list
            if (frame_di == null) {
                foreach (var mi in static_items.Values) {
                    var _cd = mi.gpos.GetDistance(ui.MapPixelToGP);
                    if (_cd < 2) {
                        frame_di = mi;
                        break;
                    }
                }
            }
            if (frame_di == null) {
                foreach (var mi in map_items) {
                    var _cd = mi.ent.gpos.GetDistance(ui.MapPixelToGP);
                    if (_cd < 2) {
                        frame_di = mi;
                        break;
                    }
                }
            }
        }
        if (frame_di == null) {
            mi_debug = null;
        }
        else {
            mi_debug = frame_di;
        }
    }
    void FrameClear() {
       
        frame_i_tasks.Clear();
        frame_party.Clear();
        frame_party.Add(ui.me);
        frame_items.Clear();
        frame_blight.Clear();
        //exped_key_frame.Clear();
        //exped_beams_frame.Clear();
        frame_trigger.Clear();
        danger_enemy.Clear();
        enemy.Clear();
        curr_danger = 0;
        //curr_marked = null;
        frame_di = null;
        danger_cells.Clear();
        //frame_debug.Clear();
    }
}