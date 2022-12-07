using ImGuiNET;
namespace Stas.GA;

public class Actor : EntComp {
    public Actor(IntPtr address) : base(address) {
    }
    DateTime next_upd = DateTime.Now;
    DateTime next_totems_upd = DateTime.Now;
    internal override void Tick(IntPtr ptr, string from=null) {
        Address = ptr;
        if (Address == IntPtr.Zero)
            return;
        var data = ui.m.Read<ActorOffset>(Address); //27D98FF37B0
        actor_ent.Tick(data.ent_ptr);
        Animation = (Animation)data.AnimationId;
        Action = (ActionFlags)data.ActionId;
        if (CurrentAction == null)
            CurrentAction = new ActionWrapper(data.ActionPtr, this);
        CurrentAction.Tick(CurrentAction.Address);
        if (DateTime.Now > next_upd) {
            GetUiSkillsState(data);
            GetActorSkills(data);
            next_upd = DateTime.Now.AddMilliseconds(500);
        }
        if (DateTime.Now > next_totems_upd) {
            GetDeploids(data);
            next_totems_upd = DateTime.Now.AddMilliseconds(120);
        }
      
    }
    public Entity actor_ent = new Entity();
    public List<Skill> actor_skills = new();
    public IEnumerable<IntPtr> ui_skills_state_ptrs { get; private set; }
    public Animation Animation { get; private set; } = Animation.Idle;
    public ActionFlags Action { get; private set; } = ActionFlags.None;
    public ActionWrapper CurrentAction { get; private set; } = null;
    public List<DeployedObject> DeployedObjects { get; private set; }

    void GetUiSkillsState(ActorOffset data) {
        var list = new List<IntPtr>();
        var start = data.ui_skills_state_array.First.ToInt64();
        var end = data.ui_skills_state_array.Last.ToInt64();
        int maxCount = 100;
        for (var ptr = start; ptr < end && maxCount > 0; ptr += 0x48, maxCount--) {
            list.Add(new IntPtr(ptr));
        }
        ui_skills_state_ptrs = list;
    }

    List<DeployedObject> frame_do = new List<DeployedObject>();
    void GetDeploids(ActorOffset data) {
        frame_do.Clear();
        if ((data.DeployedObjectArray.Last.ToInt64() - data.DeployedObjectArray.First.ToInt64()) / 8 > 300) {
            ui.AddToLog(tName + ".DeployedObjectArray count to big", MessType.Error);
            return;
        }

        for (var addr = data.DeployedObjectArray.First.ToInt64();
            addr < data.DeployedObjectArray.Last.ToInt64(); addr += 8) {
            frame_do.Add(new DeployedObject(new IntPtr(addr)));
        }
        DeployedObjects = frame_do;
    }
  
    int last_skills_hash;
    void GetActorSkills(ActorOffset data) {//25F7175C7E0
        var tname = this.tName; 
        var curr_hash = data.ActorSkillsArray.GetHashCode();
        if (curr_hash == last_skills_hash)
            return;
        last_skills_hash = curr_hash;
        var start = data.ActorSkillsArray.First.ToInt64();
        var last = data.ActorSkillsArray.Last.ToInt64();
        start += 8; //Don't ask me why. Just skipping first one
        var curr_skills = new List<Skill>();
        if ((last - start) / 16 > 50) { 
            ui.AddToLog("ActorSkills err: count >50", MessType.Error);
            return;
        }
        //16 because we are reading each second pointer (pointer vectors)
        for (var addr = start; addr < last; addr += 16) {
            var ptr = ui.m.Read<IntPtr>(addr);
            if (ptr == 0x0000025f7175c7e0) {
            }
            var na = new Skill(ptr, this);
            na.Tick(ptr);
            curr_skills.Add(na);
        }
        actor_skills = curr_skills; //wold be update if u add new skill to use in skill bar
       
    }
    internal override void ToImGui() {
        base.ToImGui();
        ImGui.Text($"AnimationId: {(int)this.Animation}, Animation: {this.Animation}");
    }
}
[Flags]
public enum ActionFlags {
    None = 0,
    UsingAbility = 2,

    //Actor is currently playing the "attack" animation, and therefor locked in a cooldown before any other action.
    AbilityCooldownActive = 16,
    Dead = 64,
    Moving = 128,

    /// actor is in the washed up state and false otherwise.
    WashedUpState = 256,
    unknow_512 = 512,
    unknow_1024 = 1024, //found on the Coast(2) Boss
    HasMines = 2048
}