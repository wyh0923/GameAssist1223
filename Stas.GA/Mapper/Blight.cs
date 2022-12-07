using System.Collections.Concurrent;
using V2 = System.Numerics.Vector2;
using V3 = System.Numerics.Vector3;
using sh = Stas.GA.SpriteHelper;


namespace Stas.GA;
public partial class AreaInstance {
    Entity blight_pamp = null;
    List<Entity> frame_blight = new List<Entity>();
    public ConcurrentBag<Entity> blight_beams = new();
    void GetBlight() {
        if (blight_pamp == null)
            return;
        if (blight_beams.Count != frame_blight.Count) {
            blight_beams = new(frame_blight);
        }
    }
}
