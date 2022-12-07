namespace Stas.GA;

public class GrantedEffectsPerLevel : RemoteObjectBase {
    public GrantedEffectsPerLevel(IntPtr address) : base(address) {
    }
    internal override void Tick(IntPtr ptr, string from = null) {
        Address = ptr;
        if (Address == IntPtr.Zero)
            return;
        if (Address == 0x0000025decf62f18) {

            var aaa = Convert.ToInt32(TimeSpan.FromMilliseconds(1200).Seconds);
        }
        SkillGemWrapper.Tick(ui.m.Read<IntPtr>(Address));
        Level = ui.m.Read<int>(Address + 0x10); //dpb 3.19.2
        RequiredLevel = ui.m.Read<int>(Address + 0x14);//dpb 3.19.2
        ManaMultiplier = ui.m.Read<int>(Address + 0x18);//dpb 3.19.2
        ManaCost = ui.m.Read<int>(Address + 0xA8);//dpb 3.19.2
        EffectivenessOfAddedDamage = ui.m.Read<int>(Address + 0xAC); //dpb 3.19.2
        Cooldown = ui.m.Read<int>(Address + 180L); //dpb 3.19.2
    }

    public SkillGemWrapper SkillGemWrapper { get; private set; } = new SkillGemWrapper(IntPtr.Zero);
    public int Level { get; private set; }
    public int RequiredLevel { get; private set; }
    public int ManaMultiplier { get; private set; }
    public int ManaCost { get; private set; }
    public int EffectivenessOfAddedDamage { get; private set; }
    public int Cooldown { get; private set; }
    //public int RequirementsComparer => M.Read<int>(Address + 0x80);


    internal int ReadStatValue(int index) {
        return ui.m.Read<int>(Address + 0x54 + index * 4);
    }

    internal int ReadQualityStatValue(int index) {
        return ui.m.Read<int>(Address + 0x9c + index * 4);
    }

    public override string ToString() {
        return Address.ToString("X");
    }

    protected override void CleanUpData() {
        throw new NotImplementedException();
    }
}
