using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Stas.GA {
    internal class LimitedLifespan : RemoteObjectBase {
        Entity e88 { get; } = new Entity();
        Entity e168 { get; } = new Entity();
        Entity e248 { get; } = new Entity();
        Entity e328 { get; } = new Entity();
        public Entity parent { get; } = new Entity();
        public LimitedLifespan(IntPtr ptr):base(ptr) {
        }
        public void TryFindeLInkedEnt() {
            var start = Address.ToInt64();
            for (var i = start; i < start + 1024; i += 8) {
                var ent = new Entity( ui.m.Read<IntPtr>(i));
                if (ent.IsValid ) {
                    var offs = i - start;
                }
            }
        }

        internal override void Tick(IntPtr ptr, string from=null) {
            Address = ptr;
            if (Address == IntPtr.Zero)
                return;
            //parent.Address = M.Read<IntPtr>(Address + 88);
            //e168.Address = M.Read<IntPtr>(Address + 168);
            //e248.Address = M.Read<IntPtr>(Address + 248); //wrong 3.19
            //e328.Address = M.Read<IntPtr>(Address + 328);
        }
        
        protected override void CleanUpData() {
            ui.AddToLog(tName + ".CleanUpData need implement", MessType.Critical);
        }
    }
}
