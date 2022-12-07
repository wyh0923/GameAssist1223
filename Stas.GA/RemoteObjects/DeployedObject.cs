namespace Stas.GA;
/// <summary>
/// is this entComp?
/// </summary>
public class DeployedObject : RemoteObjectBase {
    public DeployedObject(IntPtr ptr) : base(ptr) {
    }

    internal override void Tick(IntPtr ptr, string from = null) {
        Address = ptr;
        if (Address == IntPtr.Zero)
            return;
        var data = ui.m.Read<DeployedObjectOffsets>(Address);
        DeployedObjectType TypeId = data.TypeId;
        SkillId = data.SkillId; // Look up in ActorComponent.ActorSkills
        InstanceId = data.InstanceId; // Look up in EntityList
        Padding = data.Padding;
        Entity = ui.entities.FirstOrDefault(e => e.id == InstanceId);
        //ui.me.GetComp<Actor>(out var actor);
        //Skill = actor?.actor_skills?.Find(x => x.Id == SkillId);
    }
    public DeployedObjectType TypeId { get; private set; }
    public ushort SkillId { get; private set; }// Look up in ActorComponent.ActorSkills
    public ushort InstanceId { get; private set; } // Look up in EntityList
    public ushort Padding { get; private set; }
    public Entity Entity { get; private set; } = new Entity();
    public Skill Skill { get; private set; }
    protected override void CleanUpData() {
        throw new NotImplementedException();
    }
}

