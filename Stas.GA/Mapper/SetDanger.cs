#region using
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using Color = System.Drawing.Color;
using V2 = System.Numerics.Vector2;
using V3 = System.Numerics.Vector3;
using sh = Stas.GA.SpriteHelper;
using System.Collections.Concurrent;
using System;

#endregion

namespace Stas.GA {
    public partial class AreaInstance {
        public float danger { get; private set; }
        List<double> d_elaps = new List<double>();
        public ConcurrentBag<Entity> danger_enemy = new ConcurrentBag<Entity>();
        public ConcurrentBag<Entity> enemy = new ConcurrentBag<Entity>();
        float curr_danger;
        void SetDanger(Entity e) {
            if (e == null || !e.IsValid || e.danger == 0 || e.gdist_to_me > 120) //|| !e.IsTargetable
                return;
          
            e.danger_k = 1; //reset it
            e.GetComp<Actor>(out var actor);

            if (e.buffs!=null && e.buffs.StatusEffects.Any(x => x.Key.EndsWith("_mark"))) {
                ui.curr_map.marked =e;
            }
            if (e.Stats != null ) {
                if (e.Stats.TryGetValue(GameStat.CannotBeDamaged, out var _cbd) && _cbd == 1)
                    return;
                if (e.Stats.TryGetValue(GameStat.CannotDie, out var _cd) && _cd == 1)
                    return;
            }
            enemy.Add(e);
            SetCell(e.gpos);
            e.GetComp<Pathfinding>(out var tpf); //test path finding
            //if (tpf != null) { 
            
            //}
            //for serching offsets run here
            //if (actor != null) {
            //    DebugActer(e);
            //    return;
            //}
            //if(!ui.nav.b_can_hit(i.ent))
            //   continue;
            iTask new_it = null;
            if (e.Path.Contains("Necromancer") && ui.nav.b_can_hit(e)) {
                new_it = new MapTask(e.id, e.gpos,  "Necro");
                danger_enemy.Add(e);
                curr_danger += e.danger_rt;
                return;
            }
           
            if (actor != null) {
                var aw = actor.CurrentAction;
                if (actor.Action == ActionFlags.unknow_512) {
                    ui.AddToLog("SetDanger action=[" + actor.Action.ToString() + "]", MessType.Warning);
                } else if (actor.Action == ActionFlags.unknow_1024) {
                    ui.AddToLog("SetDanger action=[" + actor.Action.ToString() + "]", MessType.Warning);
                }
                if (aw != null) {
                    var trg = aw.Target_ent;
                    if (actor.Action.HasFlag(ActionFlags.Moving)) {
                        var tgp = aw.tgp;// pf.TargetMovePos.ToVector2();
                        if (trg != null && trg.IsValid ) {
                            foreach (var p in frame_party) {
                                var gdist = tgp.GetDistance(p.gpos);
                                if (gdist < 6) {
                                    e.danger_k = 1.5f;
                                    new_it = new EnemyTask(e.id, e.gpos, tgp, "Move");
                                    SetCell(tgp);
                                    break;
                                }
                            }
                        }
                    }
                    if (actor.Action.HasFlag(ActionFlags.UsingAbility)) {
                        foreach (var p in frame_party) {
                            if (trg != null && trg.IsValid && trg.Address == p.Address) {
                                e.danger_k = 2;
                                danger_enemy.Add(e);
                                e.target = trg;
                                new_it ??= new EnemyTask(e.id, e.gpos, trg.gpos, "Hit");
                                var _sn = "Skill"; //skill name; cant be reading right now but we will try mb next time
                                var skill = actor.CurrentAction.skill;
                                if (actor.CurrentAction.Address!=default
                                    && skill.Address!=default) {//try get skill name
                                    if (!string.IsNullOrEmpty(skill.Name)) {
                                    }
                                    else if (!string.IsNullOrEmpty(skill.InternalName)) { 
                                    }
                                }
                                var dist = aw.tgp.GetDistance(p.gpos);
                                if (dist < 20) {
                                    new_it.info = _sn; //"Hit" => "Skill"
                                    e.danger_k = 3; //2=>3
                                    SetCell(aw.tgp);
                                }
                                break;
                            }
                        }
                    }
                }
            }
            if (new_it == null) {   //try get from path
                e.GetComp<Pathfinding>(out var pf);
                if (pf != null) {
                    var tgp = pf.TargetMovePos.ToVector2();
                    if (tgp == V2.Zero) {
                        //ui.AddToLog("pf.TargetMovePos == Zero");
                        return;
                    }
                    foreach (var p in frame_party) {
                        var gdist = tgp.GetDistance(p.gpos);
                        if (gdist < 6) {
                            e.danger_k = 1.5f;
                            new_it = new EnemyTask(e.id, e.gpos, tgp, "PF_move");
                            SetCell(tgp);
                            break;
                        }
                    }
                }
            }
            if (new_it != null) {
                frame_i_tasks.Add(new_it);
                danger_enemy.Add(e);
                curr_danger += e.danger_rt;
            }
        }
        public ConcurrentDictionary<int, Cell> danger_cells = new ConcurrentDictionary<int, Cell>();
        void SetCell(V2 gpos) {
            if (!ui.nav.b_ready || gpos.X <=0 || gpos.Y <= 0)
                return;
            var gc = ui.nav.Get_gc_by_gp(gpos);
            var cell = gc?.Get_rout_by_gp(gpos);
            if (cell == null) {
                return;
            }
            danger_cells[cell.id]= cell;
        }
        V2 trg_pos;
        List<(string, float, V2)> offsa = new List<(string, float, V2)>();
        //For finding target offset use this:
        void DebugActer(Entity e) {
            trg_pos = new V2(ui.me.pos.X, ui.me.pos.Y);
            int found_ptr = 0;
            e.GetComp<Actor>(out var actor);
            var aw = actor.CurrentAction;
            if (aw == null)
                return;
            ui.AddToLog("actor Action=[" + actor.Action + "]");
            ui.AddToLog("aw addres=[" + aw.Address + "]");
            Find_Ent_and_Skill(aw.Address);
            if (aw.Target_ent.IsValid && aw.Target_ent.id == ui.me.id) { //target must be working here this time
                if (actor.Action != ActionFlags.None) {
                    offsa.Clear();
                    sw.Restart();
                    Find_actor_tgp(aw.Address); 
                    ui.AddToLog("offsa=[" + offsa.Count + "] elap=[" + sw.Elapsed.TotalMilliseconds + "]ms");
                    foreach (var v in offsa) {
                        frame_i_tasks.Add(new EnemyTask(e.id, e.gpos, v.Item3,  v.Item1));
                    }
                }
            }
            void Find_Ent_and_Skill(IntPtr ptr) {
                var start = ptr.ToInt64();
                for (var i = start; i < start + 0x800; i += 8) {
                    var ent = new Entity(ui.m.Read<IntPtr>(i));
                    if (ent.IsValid) {
                        var offs = (i - aw.Address.ToInt64()).ToString("X"); //3.17=E8  
                    }
                    var skill = new Skill(ui.m.Read<IntPtr>(i), actor);
                    //InternalName not still NOT found 
                    if (skill.InternalName.Length > 0) {
                        var offs = (i - aw.Address.ToInt64()).ToString("X"); //3.17=B8  
                    }
                    //GetV2(start, i);

                    //no idea wehat is do
                    ////var addr = ui.m.Read<long>(i);
                    ////var a = addr >> 16;
                    ////var b = start >> 16;
                    ////if (a == b) {
                    ////    found_ptr += 1;
                    ////    FingV2(addr);
                    ////    //FingV2(addr);
                    ////}
                }
            }
           
            void Find_actor_tgp(IntPtr ptr) {
                var start = ptr.ToInt64();
                for (var i = start; i < start + 0x800; i += 4) {
                   //GetV2(start, i);
                    Test_as_Int(i);
                }
            }
            void Test_as_Int(long i) { 
                var x = ui.m.Read<int>(i);
                if (ui.me.gpos.X == x) { 
                    var y = ui.m.Read<int>(i+4);
                    if (y == ui.me.gpos.Y) {
                        var offs = (i - aw.Address.ToInt64()).ToString("X");
                        var v2 = new V2(x, y);
                        var gdist = v2.GetDistance(ui.me.gpos);
                        offsa.Add((offs, gdist, v2)); // 
                    }
                }
            }
            void GetV2(long start, long i) {
                var v2 = ui.m.Read<V2>(i);
                if (float.IsNaN(v2.X) || float.IsNaN(v2.Y))
                    return;
                var gdist = v2.GetDistance(trg_pos);
                if (float.IsInfinity(gdist) || gdist < 0 || gdist > 2000)
                    return;
                var offs = (i - start).ToString("X");
                offsa.Add((offs, gdist, v2)); // 
            }
        }
    }
}