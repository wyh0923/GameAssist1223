using V2 = System.Numerics.Vector2;
namespace Stas.GA; 

public class Pathfinding : EntComp {
    public Pathfinding(IntPtr ptr):base(ptr) {
    }
    internal override void Tick(IntPtr ptr, string from = null) {
        Address = ptr;
        if (Address == IntPtr.Zero)
            return;
        var data = ui.m.Read<PathfindingComponentOffsets>(Address);
        TargetMovePos = data.ClickToNextPosition;
        PreviousMovePos = data.WasInThisPosition;
        WantMoveToPosition = data.WantMoveToPosition;
        IsMoving = data.IsMoving == 2;
        StayTime = data.StayTime;
    }
    public Vector2i TargetMovePos { get; private set; }
    public Vector2i PreviousMovePos { get; private set; }
    public Vector2i WantMoveToPosition { get; private set; }
    public bool IsMoving { get; private set; }
    public float StayTime { get; private set; }
}
