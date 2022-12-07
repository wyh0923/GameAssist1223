using System.Runtime.InteropServices;

namespace Stas.GA {
    [StructLayout(LayoutKind.Explicit, Pack = 1)]
    public struct DeployedObjectOffsets {
        [FieldOffset(0x0)] public DeployedObjectType TypeId;//4 for totems, 22 for golems
        [FieldOffset(0x2)] public ushort SkillId;// Look up in ActorComponent.ActorSkills
        [FieldOffset(0x4)] public ushort InstanceId;// Look up in EntityList
        [FieldOffset(0x6)] public ushort Padding;//Always 0
    }
    public enum DeployedObjectType : ushort {
        Spectre = 0,
        Totem = 4,
        Trap,
        Zombie,
        Skeleton,
        Mine = 10,
        Skitterbots = 14, // Share same id as mirrors/clones
        Clone = 14,
        AnimatedGuardian = 16,
        AnimatedWeapon = 17,
        RagingSpirit = 18,
        Golem = 22,
        AgonyCrawler = 36,
        HolyRelic = 38,
        AbsolutionSentinel = 47,
        Reaper = 48
    }
}
