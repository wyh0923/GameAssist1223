using System.Numerics;
using V3 = System.Numerics.Vector3;

namespace Stas.GA;

public struct CameraOffsets {
    public IntPtr curs_ptr;
    public int Width;
    public int Height;
    public Matrix4x4 MatrixBytes;
    public float ZFar;
    public V3 me_pos;
    public IntPtr target_ent_ptr;
}