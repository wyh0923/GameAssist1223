using NAudio.Utils;
using System;


namespace Stas.GA {
    public class LabelOnGround : RemoteObjectBase {
        internal LabelOnGround(IntPtr address) : base(address) {
        }
        IntPtr labelInfo = IntPtr.Zero;//for debug only
        internal override void Tick(IntPtr ptr, string from=null) {
            if (Address == IntPtr.Zero)
                return;
            var elem = new Element(Address + 0x10, "LabelOnGround");
            Label = elem.Address == IntPtr.Zero ? null : elem;

            var ent = new Entity(Address + 0x18);
            ItemOnGround = ent.Address == IntPtr.Zero ? null : ent;
            if (Label != null && Label.Address != IntPtr.Zero)
                labelInfo = ui.m.Read<IntPtr>(Label.Address + 0x398);
            else
                labelInfo = IntPtr.Zero;
        }

        public bool IsVisible => Label?.IsVisible ?? false;

        public Entity ItemOnGround { get; private set; }

        public Element Label { get; private set; }

        ////Temp solution for pick it, need test PickTest and PickTest2
        //public bool CanPickUp {
        //    get {
        //        var label = Label;

        //        if(label != null)
        //            return ui.M.Read<long>(label.Address + 0x420) == 1;

        //        return true;
        //    }
        //}

        //public TimeSpan TimeLeft {
        //    get {
        //        if(CanPickUp) return new TimeSpan();
        //        if(labelInfo.Value == IntPtr.Zero) return MaxTimeForPickUp;
        //        var futureTime = ui.M.Read<int>(labelInfo.Value + 0x38);
        //        return TimeSpan.FromMilliseconds(futureTime - Environment.TickCount);
        //    }
        //}

        //Temp solution for pick it
        //public TimeSpan MaxTimeForPickUp =>
        //    TimeSpan.Zero; // !CanPickUp ? TimeSpan.FromMilliseconds(M.Read<int>(labelInfo.Value + 0x34)) : new TimeSpan();

        //long GetLabelInfo() {
        //    return Label != null ? Label.Address != IntPtr.Zero ?
        //        ui.M.Read<long>(Label.Address + 0x398) : 0 : 0;
        //}

        public override string ToString() {
            if (ItemOnGround == null)
                return "ItemOnGround==null";
            else {
                if (ItemOnGround.GetComp<WorldItem>(out var wi)) {
                    var ie = wi.ItemEntity;
                    if (ie == null) {
                        return "ItemEntity==null";
                    }
                    if (ie.GetComp<Base>(out var _b)) {
                        return _b.ItemBaseName + " [" + ItemOnGround.GetKey + "]";
                    }
                    else return "Base==null";
                }
                else
                    return "WorldItem==null";
            }
        }
        
        protected override void Clear() {
            ui.AddToLog(tName + ".CleanUpData need implement", MessType.Critical);
        }

    }
}