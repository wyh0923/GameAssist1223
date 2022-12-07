
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Stas.GA {
    public class ActiveSkillWrapper : EntComp {
        public ActiveSkillWrapper(IntPtr address) : base(address) {
        }
        internal override void Tick(IntPtr ptr, string from=null) {
            Address = ptr;
            if (Address == IntPtr.Zero)
                return;
            InternalName = ui.m.ReadUnicodeString(ui.m.Read<IntPtr>(Address));
            DisplayName = ui.m.ReadUnicodeString(ui.m.Read<IntPtr>(Address + 0x8));
            Description = ui.m.ReadUnicodeString(ui.m.Read<IntPtr>(Address + 0x10));
            SkillName = ui.m.ReadUnicodeString(ui.m.Read<IntPtr>(Address + 0x18));
            Icon = ui.m.ReadUnicodeString(ui.m.Read<IntPtr>(Address + 0x20));
            LongDescription = ui.m.ReadUnicodeString(ui.m.Read<IntPtr>(Address + 0x50));
            AmazonLink = ui.m.ReadUnicodeString(ui.m.Read<IntPtr>(Address + 0x60));
        }
        public string LongDescription { get; private set; }
        public string AmazonLink { get; private set; }
        public string InternalName { get; private set; }
        public string DisplayName { get; private set; }
        public string Description { get; private set; }
        public string SkillName { get; private set; }
        public string Icon { get; private set; }
        public List<int> CastTypes {
            get {
                var result = new List<int>();
                var castTypesCount = ui.m.Read<int>(Address + 0x28);
                var readAddr = ui.m.Read<long>(Address + 0x30);

                for (var i = 0; i < castTypesCount; i++) {
                    result.Add(ui.m.Read<int>(readAddr));
                    readAddr += 4;
                }

                return result;
            }
        }

        public List<int> SkillTypes {
            get {
                var result = new List<int>();
                var skillTypesCount = ui.m.Read<int>(Address + 0x38);
                var readAddr = ui.m.Read<long>(Address + 0x40);

                for (var i = 0; i < skillTypesCount; i++) {
                    result.Add(ui.m.Read<int>(readAddr));
                    readAddr += 4;
                }

                return result;
            }
        }

        protected override void CleanUpData() {
            throw new NotImplementedException();
        }

       
        
    }
}
