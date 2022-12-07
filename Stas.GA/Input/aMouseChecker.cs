using V2 = System.Numerics.Vector2;
namespace Stas.GA {

    /// <summary>
    /// Detect "Manul Mouse Moving" for prevent "auto Mouse moving" upon reaching the threshold 
    /// </summary>
    public abstract class aMouseChecker {
        V2 last_mouse_pos;
        List<V2> m_deltas = new List<V2>();
        float m_delta_summ;
        public DateTime last_m_setted { get; private set; }//last mouse setted time
        public FixedSizedLog log { get; }
        public aMouseChecker() {
        }
        public void ClearDeltas() {
            m_deltas.Clear();
        }
        //TODO we need to understand whether the mouse moved manually or automatically in order to calculate the delta
        public void SetMouseDeltaSumm() {
            //ui.AddToLog("ims=[" + InputDetecting.ims.originId + "]");
            var curr_mpos = Mouse.GetCursorPosition();
            if (last_mouse_pos == V2.Zero)
                last_mouse_pos = curr_mpos;
            var delta = last_mouse_pos - curr_mpos;
            if (m_deltas.Count > 20)
                m_deltas.RemoveAt(0);
            m_deltas.Add(delta);
            m_delta_summ = Math.Abs((m_deltas.Sum(m => m.X) + m_deltas.Sum(m => m.Y)) / m_deltas.Count);
            if (m_delta_summ > 1) {
                last_m_setted = DateTime.Now;
            }
            last_mouse_pos = curr_mpos;
            //log.Add("m_delta_summ=[" + m_delta_summ.ToRoundStr(3) + "]");
        }
    }
}
