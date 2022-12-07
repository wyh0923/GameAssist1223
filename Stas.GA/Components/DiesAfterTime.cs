namespace Stas.GA; 
/// <summary>
///     The <see cref="DiesAfterTime" /> component in the entity.
/// </summary>
public class DiesAfterTime : EntComp {
    public DiesAfterTime(IntPtr address) : base(address) { 
    }
    internal override void Tick(IntPtr ptr, string from = null) {
        Address = ptr;
        if (Address == IntPtr.Zero)
            return;
    }
}