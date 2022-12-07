using V2 = System.Numerics.Vector2;

namespace Stas.GA;
public class ActionWrapper : EntComp {
    public ActionWrapper(IntPtr address, Actor _actor) : base(address) {
        actor = _actor;
    }

    public V2 tgp { get; private set; }
    public Entity Target_ent { get; private set; } = new Entity(IntPtr.Zero);
    public Skill skill { get; private set; }
    public Actor actor{ get; private set; }
    public void setActor(Actor _actor) {
        actor = _actor;
    }
    internal override void Tick(IntPtr ptr, string from=null) {
        Address = ptr;
        if (Address == default || actor == null)
            return;
        var data = ui.m.Read<ActionWrapperOffsets>(Address);
        Target_ent.Tick( data.Target);
        if (skill == null)
            skill = new Skill(data.Skills, actor);
        else
            skill.Tick(data.Skills);
        tgp = new V2(data.target_gp_x, data.target_gp_y);
    }   

    protected override void CleanUpData() {
        Target_ent.Tick( IntPtr.Zero);
    }
}

