using System.Runtime.InteropServices;
namespace Stas.GA;

[StructLayout(LayoutKind.Explicit, Pack = 1)]
public struct ActorOffset {
    [FieldOffset(0x68)] public IntPtr ptr_68;
    [FieldOffset(0x190)] public IntPtr ent_ptr;
    [FieldOffset(0x1A8)] public IntPtr ActionPtr; //dpb=ActionWrapperStruct276
    [FieldOffset(0x1D8)] public StdVector Effects; //dpb 3.19.2
    [FieldOffset(0x208)] public short ActionId;//dpb=Flags
    [FieldOffset(0x234)] public int AnimationId; 
    [FieldOffset(0x258)] public float TimeSinceLastMove; //dpb 3.19.2
    [FieldOffset(0x25C)] public float TimeSinceLastAction;//dpb 3.19.2
    [FieldOffset(0x690)] public StdVector ActorSkillsArray;
    [FieldOffset(0x6A8)] public StdVector ui_skills_state_array; //CoolDownSkills
    [FieldOffset(0x6C0)] public StdVector ActorVaalSkills;
    [FieldOffset(0x6D8)] public StdVector DeployedObjectArray;
  
}