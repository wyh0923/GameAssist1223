using System.Diagnostics;
using System.Globalization;
using System.Xml.Linq;

namespace Stas.GA;

public partial class Skill : RemoteObjectBase {
    public Skill(IntPtr address, Actor _actor)
        : base(address, _actor) {
        actor = _actor;
    }
    public Actor actor { get; private set; }
    public void setActor(Actor _actor) {
        actor = _actor;
    }
    SkillUiStateOffsets _ui_state;
    internal override void Tick(IntPtr ptr, string from = null) { //27EFE80DE20
        Address = ptr; //1E10A8C6FB0
        if (Address == IntPtr.Zero)
            return;
        var data = ui.m.Read<SkillOffsets>(Address);
        Id = data.id;
        gepl.Tick(data.GrantedEffectsPerLevelPtr);
        Name = gepl.SkillGemWrapper.Name;
        InternalName = gepl.SkillGemWrapper.ActiveSkill.InternalName;
        CanBeUsedWithWeapon = data.CanBeUsedWithWeapon > 0;
        CanBeUsed = data .CanBeUsed == 0;
        TotalUses = data.TotalVaalUses;
        MaxUses = data.TotalVaalUses;
        Cooldown = data.Cooldown; //Converted milliseconds to seconds 
       
        SoulsPerUse = data.SoulsPerUse;
        TotalVaalUses = data.TotalVaalUses;
        SkillSlotIndex = ui.curr_map.server_data.skill_bar_ids.IndexOf(Id);
        IsOnSkillBar = SkillSlotIndex != -1;
        SkillUseStage = ui.m.Read<byte>(Address + 0x8);
        if (_ui_state.SkillId == 0 && actor != null) {
            foreach (var _ptr in actor.ui_skills_state_ptrs) {
                var state = ui.m.Read<SkillUiStateOffsets>(_ptr);
                if (state.SkillId == Id) {
                    _ui_state = ui.m.Read<SkillUiStateOffsets>(_ptr);
                    break;
                }
            }
        }
        IsOnCooldown = _ui_state.CooldownHigh - _ui_state.CooldownLow >> 4 >= _ui_state.NumberOfUses;
        RemainingUses = _ui_state.NumberOfUses - ((int)(_ui_state.CooldownHigh - _ui_state.CooldownLow) >> 4);
    }

    public ushort Id { get; private set; }
    /// <summary>
    /// GrantedEffectsPerLevel
    /// </summary>
    public GrantedEffectsPerLevel gepl { get; private set; } =new GrantedEffectsPerLevel(default);
    public bool CanBeUsedWithWeapon { get; private set; }
    public bool CanBeUsed { get; private set; }
    public int TotalUses { get; private set; }
    public int MaxUses { get; private set; }
    public int Cooldown { get; private set; }
    public int SoulsPerUse { get; private set; }
    public int TotalVaalUses { get; private set; }
    public bool IsOnSkillBar { get; private set; }
    public int SkillSlotIndex { get; private set; }

    public void TryFindGrantedEffectsPerLevel() {
        for (var i = 0; i < 512; i += 8) {
            var ptr = ui.m.Read<IntPtr>(Address + i);
            var gepl = new GrantedEffectsPerLevel(ptr);
            if (gepl.Cooldown == 2500) {
                var GrantedEffectsPerLevel_offset = i.ToString("X"); //0x18
            }
        }
    }
    public void TryFindActiveSkillWrapper() {
        var gepl = this.gepl;
        for (var i = 0; i < 1024; i += 8) {
            var ptr = ui.m.Read<IntPtr>(gepl.Address + i);
            var asg = new ActiveSkillWrapper(ptr);
            if (asg.Icon == "asd") {
                var ActiveSkillWrapper_offset = i.ToString("X");
            }
        }
    }

    public string Name { get; private set; }
    public string InternalName { get; private set; }
    internal int SlotIdentifier => Id >> 8 & 0xff;
    public int SocketIndex => SlotIdentifier >> 2 & 15;
    public bool IsUserSkill => (SlotIdentifier & 0x80) > 0;
    public bool AllowedToCast => CanBeUsedWithWeapon && CanBeUsed;
    public byte SkillUseStage { get; private set; }
    public bool IsUsing => SkillUseStage > 2;
    public bool PrepareForUsage => SkillUseStage == 1;
    public float Dps => GetStat(GameStat.HundredTimesDamagePerSecond + (IsUsing ? 4 : 0)) / 100f;
    public TimeSpan CastTime => IsInstant
        ? TimeSpan.FromMilliseconds(0)
        : TimeSpan.FromMilliseconds((int)Math.Ceiling(1000f / (HundredTimesAttacksPerSecond / 100f)));
    public int HundredTimesAttacksPerSecond {
        get {
            if (IsCry) return 60;
            if (IsInstant) return 0;

            return GetStat(
            IsSpell
                ? GameStat.HundredTimesCastsPerSecond
                : GameStat.HundredTimesAttacksPerSecond
            );
        }
    }
    public bool IsSpell => GetStat(GameStat.CastingSpell) == 1;
    public bool IsAttack => GetStat(GameStat.SkillIsAttack) == 1;
    public bool IsCry => InternalName.EndsWith("_cry");
    public bool IsInstant => GetStat(GameStat.SkillIsInstant) == 1;
    public bool IsMine => GetStat(GameStat.IsRemoteMine) == 1 || GetStat(GameStat.SkillIsMined) == 1;
    public bool IsTotem => GetStat(GameStat.IsTotem) == 1 || GetStat(GameStat.SkillIsTotemified) == 1;
    public bool IsTrap => GetStat(GameStat.IsTrap) == 1 || GetStat(GameStat.SkillIsTrapped) == 1;
    public bool IsVaalSkill => SoulsPerUse >= 1 && TotalVaalUses >= 1;
    public bool IsOnCooldown { get; private set; }
    public int RemainingUses { get; private set; }

    public Dictionary<GameStat, int> Stats {
        get {
            var statsPtr = ui.m.Read<long>(Address + 0xB0);
            var result = new Dictionary<GameStat, int>();

            ReadStats(result, statsPtr);

            return result;
        }
    }

    internal void ReadStats(Dictionary<GameStat, int> stats, long address) {
        var statPtrStart = ui.m.Read<long>(address + 0xE8);
        var statPtrEnd = ui.m.Read<long>(address + 0xF0);
        var totalStats = (int)(statPtrEnd - statPtrStart);
        var bytes = ui.m.ReadMemoryArray<byte>(new IntPtr(statPtrStart), totalStats);

        for (var i = 0; i < bytes.Length; i += 8) {
            var key = 0;
            var value = 0;
            try {
                key = BitConverter.ToInt32(bytes, i);
                value = BitConverter.ToInt32(bytes, i + 0x04);
            }
            catch (Exception e) {
                ui.AddToLog($"ActorSkill.ReadStats -> BitConverter failed, i: {i}", MessType.Error);
                ui.AddToLog($"ActorSkill.ReadStats -> {e.Message}", MessType.Error);
                continue;
            }

            stats[(GameStat)key] = value;
        }
    }

    public int GetStat(GameStat stat) {
        return !Stats.TryGetValue(stat, out var num) ? 0 : num;
    }

    public override string ToString() {
        return "n=" + Name + " id=" + Id + " in=" + InternalName + " CanUsed=" + CanBeUsed;
    }

    protected override void Clear() {
        ui.AddToLog(tName + "CleanUpData err: debug me", MessType.Error);
    }
}
