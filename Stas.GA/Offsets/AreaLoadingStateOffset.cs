namespace Stas.GA
{
    using System.Runtime.InteropServices;
    [StructLayout(LayoutKind.Explicit, Pack = 1)]
    public struct AreaLoadingStateOffset
    {
        //[FieldOffset(0x0B)] public byte game_state; //GameStateEnum 
        [FieldOffset(0xC8)] public int IsLoading; //correct 
        [FieldOffset(0x368)] public uint TotalLoadingScreenTimeMs; //adds here during loading
        [FieldOffset(0x3A8)] public StdWString CurrentAreaName; // use isloading/totalLoadingScreenTimeMs offset diff
    }
}
