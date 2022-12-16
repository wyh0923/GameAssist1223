using System.Collections.Concurrent;
using System.Diagnostics;
using V2 = System.Numerics.Vector2;
namespace Stas.GA;

public abstract partial class aTasker {
    #region bool flags
    /// <summary>
    ///dont do quest/looting/etc if no danger, but fight and protect the Leader
    /// </summary>
    public bool b_fallow_hard { get; private set; }
    public bool b_hold => task != null && task.GetType() == typeof(Hold);
    #endregion
    List<aSkill> need_stop_cast;
    public aTask task { get; private set; }
    public ConcurrentBag<iTask> i_tasks = new ConcurrentBag<iTask>();
    protected ConcurrentStack<aTask> tasks = new ConcurrentStack<aTask>();
    public Dictionary<V2, Entity> checked_npc = new Dictionary<V2, Entity>();
    public Thread tasker_thread;
    protected abstract void MakeRoleTask();
    Stopwatch sw = new Stopwatch();
    List<double> elapsed = new();
    public aTasker() {
        tasker_thread = new Thread(() => {
            while (ui.b_running) {
                while (ui.worker == null) {//it's possible right after the program start
                    if (ui.curr_role != Role.None)
                        ui.AddToLog("Tasker: worker==null", MessType.Critical);
                    Thread.Sleep(200);
                }
                need_stop_cast ??= new List<aSkill>() { ui.worker.main, ui.worker.totem };
                #region tick timer & w8ting for relax CPU
                var d_elaps = sw.Elapsed.TotalMilliseconds;
                elapsed.Add(d_elaps);
                if (elapsed.Count > 60)
                    elapsed.RemoveAt(0);
                var frame_time = elapsed.Sum() / elapsed.Count;
                if (frame_time < ui.w8) {
                    Thread.Sleep(ui.w8 - (int)frame_time);
                }
                else {
                    Thread.Sleep(1);
                    ui.AddToLog("Tasker: Big Tick Time", MessType.Error);
                }
                #endregion
            }
        });
        tasker_thread.IsBackground= true;
        tasker_thread.Start();
    }
    public void Add_iTask(iTask it) {
        i_tasks.Add(it);
    }
    public void Hold(string from = null) {
        //_fh = true;
        Reset("Hold... " + from);
        ui.nav.SaveVisited();
        ui.tasker.TaskPop(new Hold());
    }
    public void Unhold(string from = null) {
        Reset("Unhold... " + from);
        b_fallow_hard = false;
    }
    public void SetFallowHard(bool b_set) {
        b_fallow_hard = b_set;
        ui.AddToLog("SetFallowHard.. b_set=[" + b_set + "]");

        if (b_hold) {
            TaskDone(task, "SetFallowHard");
        }
        if (!b_set)
            StopCastAll();
        else {
            if (this is Slave) {
                var ll = ui.leader;
                //For remove grace aura(if present) and visualise bot ready
                if (ll.b_OK && ll.gdist_to_me < 20 && ui.worker.totem != null) {
                   // ui.tasker.TaskPop(new UseTotems(ll.pos));
                }
            }
        }
    }
   
    float min_dist = 8; //if party have 4x ; 6 if party have 2-3
    public float GetFallowDist { //max buff dist = 50
        get {
            if (ui.b_home)
                return 60;
            else {
                if (b_fallow_hard) {
                    return min_dist;
                }
                var jr = 46f;
                if (ui.worker.jump != null)
                    jr = ui.worker.jump.grange;
                if (ui.curr_map.danger == 0)
                    return 0.9f * jr + ui.sett.buff_gdist; //for faster return to buff distance
                else
                    return ui.sett.buff_gdist;
            }
        }
    }
    public void CleareDebug(aTask task) {
        Remove_iTaskById(task.id);
        ui.nav.debug_res = null;
        i_tasks.Clear();
        ui.test?.gpa?.Clear();
    }
    public void Remove_iTaskById(uint _id) {
        var tmp_list = i_tasks.Where(t => t.id != _id);
        i_tasks = new ConcurrentBag<iTask>(tmp_list);
    }
    DateTime last_mfu; //last mana flask used
    public bool UseManaFlask() {
        if (last_mfu.AddSeconds(3) < DateTime.Now) {
            if (ui.worker.b_use_low_life)
                Keyboard.KeyPress(Keys.E, "Use mana flask");
            else
                Keyboard.KeyPress(Keys.F6, "Use mana flask");
            last_mfu = DateTime.Now;
            return true;
        }
        return false;
    }
    DateTime last_lfu;//last life flask used
    public void UseLifeFlask(bool dont_check = false) {
        if (ui.worker == null || ui.worker.b_use_low_life || ui.life == null)
            return;
        var low_life = ui.life.Health.CurrentInPercent < 40;
        bool can_use = last_lfu.AddMilliseconds(ui.worker.life_flask_ms) < DateTime.Now;
        if (dont_check || (low_life && can_use)) {
            Keyboard.KeyPress(Keys.E, "CheckLife");
            last_lfu = DateTime.Now;
            ui.warning = "Use Life potion was OK";
        }
    }
  
}
