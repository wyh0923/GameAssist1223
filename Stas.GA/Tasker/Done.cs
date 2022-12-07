namespace Stas.GA; 

public abstract partial class aTasker {
    public void TaskDone(aTask tdone, string info, MessType _mt = MessType.Ok) {
        //Debug.Assert(task!=null &&  tdone.id == task.id);
        tdone.b_done = true;
        //if (tdone.GetType() != typeof(ClickOnElem)) { //debug here
        //}
        if (!tdone.b_debug)
            CleareDebug(tdone);
        //Debug.Assert(task != null && task.id == tdone.id, "Check tasks making order");
        if (tdone.b_need_auto_stop)
            ui.worker?.StopMoving(tdone.id_name + "=>b_need_auto_stop");
        StopCastAll();
        task = null;
        if (tdone.b_debug || _mt != MessType.Ok)
            ui.AddToLog(tdone.id_name + ".. " + info, _mt);
        tdone.do_after?.Invoke();
    }

    public void StopCastAll() {
        if (need_stop_cast == null)//worker not init jet
            return;
        foreach (var s in need_stop_cast) {
            s?.StopCast();//debug here
        }
    }
    public void Reset(string info = null, bool dell_curr_task = true,
      bool reset_id = true, bool cleare_i_tasks = true) {
        StopCastAll();
        if (reset_id)
            ui.ResetNextTaskId();
        if (cleare_i_tasks)
            i_tasks.Clear();
        checked_npc.Clear();
        if (dell_curr_task && task != null)
            TaskDone(task, "aTasker.Reset", MessType.Warning);
        tasks.Clear();
    }
    /// <summary>
    /// for restart need make new ()
    /// </summary>
    /// <param name="info"></param>
    public void Stop(string info) {
        tasks.Clear();
        if (task != null)
            TaskDone(task, info);
        ui.worker?.ReliseKeys();
    }

}
