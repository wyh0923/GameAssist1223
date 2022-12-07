namespace Stas.GA;
/// <summary>
///     Points to the LargeMap UiElement.
///     It is exactly like any other element, except its in-memory position is its center
/// </summary>
internal class LargeMap : aMapElemet {
    internal LargeMap(IntPtr address) : base(address) {
    }
    internal override void Tick(IntPtr ptr, string from = null) {
        base.Tick(ptr, from);
        if (Address == IntPtr.Zero)
            return;
        var debug = this; //1E152F19FA0
        var vis = this.IsVisible;
    }

}
