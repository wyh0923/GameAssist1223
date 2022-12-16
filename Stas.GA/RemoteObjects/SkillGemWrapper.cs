using System;
using System.Net;

namespace Stas.GA {
    public class SkillGemWrapper : RemoteObjectBase {
        public SkillGemWrapper(IntPtr address) : base(address) {
        }
        internal override void Tick(IntPtr ptr, string from = null) {
            Address = ptr;
            if (Address == IntPtr.Zero)
                return;
            Name = ui.m.ReadStringU(ui.m.Read<IntPtr>(Address));
            //27CBBCA4BA0
            ActiveSkill.Tick(ui.m.Read<IntPtr>(Address + 0x6D0));
        }

        protected override void Clear() {
            ui.AddToLog(tName + "CleanUpData err: debug me", MessType.Error);
        }
        public string Name { get; private set; }
        public ActiveSkillWrapper ActiveSkill { get; private set; } = new ActiveSkillWrapper(IntPtr.Zero);
        public void tryFindeActiveSkillPtr() {
            var start = Address;
            for (var i = start; i < start + 8 * 800; i += 8) {
                var ActiveSkill = new ActiveSkillWrapper(ui.m.Read<IntPtr>(i));
                var sn = ActiveSkill.SkillName;
                var al = ActiveSkill.AmazonLink;
                var @in = ActiveSkill.InternalName;
                if (sn.Length > 0 || @in.Length > 0) {
                    var offs = (i - start).ToString("X"); //6D0  3.19
                }
            }
        }
    }
}
