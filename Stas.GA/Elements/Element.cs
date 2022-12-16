using System.Diagnostics;
using System.Drawing;
using System.Text;
using V2 = System.Numerics.Vector2;
namespace Stas.GA;
/// <summary>
///     Points to the Ui Element of the game and reads its data.
/// </summary>
public partial class Element : RemoteObjectBase {
    internal Element(string _tname) : this(IntPtr.Zero, _tname) {
    }
    internal Element(IntPtr ptr, string tname) : base(ptr) {
        Address = ptr;
        Debug.Assert(!string.IsNullOrEmpty(tname));
        //if (tname.Length > 32)
        //    tname = tname.Substring(0, 32) + "..skip";
        _tname = tname;
        if (ptr != default)
            Init(tname + "()");
    }
    internal bool b_init { get; private set; } = false;
    void Init(string from) {
        ui.elements[Address] = this;
        var data = ui.m.Read<ElemOffsets>(Address, tName);
        var _nam = tName;
        if (_nam.Length > 32)
            _nam = _nam.Substring(0, 32) + "..skip.";
        if (data.SelfPointer == Address && Address != default) {
            IsValid = true;
        }
        else {
            if (tName != "open_left_panel" && tName != "open_right_panel") {
                ui.AddToLog(_nam + " NOT Valid", MessType.Error);
                return;
            }
        }
        b_selected_tab = ui.m.Read<Byte>(Address + 0x230) == 1;
        b_goodbye_selected = ui.m.Read<uint>(Address + 0x1C8) == 0xFFFFCA96;
        TextBoxOverlayColor = Color.FromArgb((int)data.TextBoxOverlayColor);
        b_link_selected = (TextBoxOverlayColor == ((uint)0xFFFFFFF).ToColor());
        b_mouse_over = ui.m.Read<byte>(Address + 0x1E0 + 0x1B) == 1;
        X = data.X;
        Y = data.Y;
        if (data.Parent != default) {
            if (!ui.elements.ContainsKey(data.Parent))
                ui.elements[data.Parent] = new Element(data.Parent, tName + ".parent");
            Parent = ui.elements[data.Parent];
        }
        chld_count = data.chld_ptr.Size / 8;
        children_pointers = ui.m.ReadStdVector<IntPtr>(data.chld_ptr);
        //var id = ui.m.ReadStdWString(data.Id);//only root?
        positionModifier.X = data.PositionModifier.X;
        positionModifier.Y = data.PositionModifier.Y;

        scaleIndex = data.ScaleIndex;
        localScaleMultiplier = data.LocalScaleMultiplier;
        flags = data.Flags;
        Text = GetText();// ui.m.ReadNativeString(data.text_or_textptr);
        unScaledSize.X = Width = data.UnscaledSize.X;
        unScaledSize.Y = Height = data.UnscaledSize.Y;
        Scale = data.Scale;
        IsVisibleLocal = (data.IsVisibleLocal & 8) == 8;// ==(byte) 0x2E;  //0x26 is hidden
        b_init = true;
    }
    internal override void Tick(IntPtr ptr, string from) {
        var nam = this.tName;
        Address = ptr;
        if (Address == default) {
            Clear();
            return;
        }
        if (!b_init)
            Init(from);
        //GetChildren(from);
        IsVisibleLocal = (ui.m.Read<byte>(Address + ui.IsVisibleLocalOffs) & 8) == 8;// ==(byte) 0x2E;  //0x26 is hidden
    }
    protected override void Clear() {
        b_init = false;
        Parent = null;
        children_pointers = Array.Empty<IntPtr>();
        flags = 0x00;
        localScaleMultiplier = 0x00;
        unScaledSize = V2.Zero;
        scaleIndex = 0x00;
    }
    string GetText() {
        var curr_elem = tName;
        var offs = Address + ui.elem_text_offs;
        if (!ui.texts.ContainsKey(offs)) {
            var length = ui.m.Read<long>(offs + 0x10);
            var Capacity = (int)ui.m.Read<long>(offs + 0x18);
            var addr = Capacity < 8 ? offs : ui.m.Read<long>(offs);

            if (addr <= 0 || length > 5120 || length <= 0)
                ui.texts[offs] = "";
            else
                ui.texts[offs] = ui.m.ReadStringU(addr, (int)length * 2);
        }
        return ui.texts[offs];
    }


    #region Props
    public float X { get; private set; }
    public float Y { get; private set; }
    public bool b_link_selected { get; private set; }
    public Color TextBoxOverlayColor { get; private set; }
    public bool b_mouse_over { get; private set; }
    public bool b_goodbye_selected { get; private set; }
    public bool b_selected_tab { get; private set; }
    public bool IsValid { get; private set; }
    public virtual string Text { get; private set; }
    public long chld_count { get; private set; }
    public Element Parent { get; private set; }
    public float Width { get; private set; }
    public float Height { get; private set; }
    public float Scale { get; private set; }
    public bool b_selected { get; private set; }
    /// <summary>
    ///     Flags associated with the UiElement.
    ///     They contains IsVisible and ShouldModifyPostion information.
    /// </summary>
    public uint flags;
    /// <summary>
    ///     Local multiplier to apply to the scale value.
    /// </summary>
    public float localScaleMultiplier;
    /// <summary>
    ///     Index of the List of scale values.
    /// </summary>
    public byte scaleIndex;
    /// <summary>
    ///     Size of the ui element without applying the scale multiplier/modifier.
    /// </summary>
    public V2 unScaledSize = V2.Zero;

    V2 positionModifier = V2.Zero;

    #endregion

    public bool b_can_click {
        get {
            var dict = ui.w8ting_click_until;
            if (dict.ContainsKey(Address) && dict[Address] > DateTime.Now)
                return false;
            return true;
        }
    }

    private static string Sanitize(string text) {
        return !string.IsNullOrWhiteSpace(text) ? text.Replace("\u00A0\u00A0\u00A0\u00A0", "{{icon}}") : string.Empty;
    }

    public Element GetChildAtIndex(int index) {
        if (index >= chld_count)
            return null;
        return new Element(ui.m.Read<IntPtr>(Address + ui.first_children_offset, index * 8), tName + ".chld[" + index + "]");
    }


    /// <summary>
    ///     Gets the position of the Ui Element w.r.t the game UI.
    /// </summary>
    public virtual V2 Postion {
        get {
            var (widthScale, heightScale) = ui.GameScale.GetScaleValue(
                this.scaleIndex, this.localScaleMultiplier);
            var pos = this.GetUnScaledPosition();
            pos.X *= widthScale;
            pos.Y *= heightScale;
            if (float.IsNaN(pos.X) || float.IsNaN(pos.Y)) {
                ui.AddToLog(tName + ".Postion is NaN", MessType.Error);
            }//debug here
            return pos;
        }
        private set { }
    }

    /// <summary>
    ///     Gets the size of the Ui Element w.r.t the game UI.
    /// </summary>
    public virtual V2 Size {
        get {
            var (widthScale, heightScale) = ui.GameScale.GetScaleValue(
                this.scaleIndex, this.localScaleMultiplier);
            var size = this.unScaledSize;
            size.X *= widthScale;
            size.Y *= heightScale;
            return size;
        }
        private set { }
    }
    public bool IsVisibleLocal { get; private set; }
    public bool IsVisible {
        get {
            if (Address >= 1770350607106052 || Address <= 0) return false;
            var parent = GetParentChain().All(current => current.IsVisibleLocal);
            return IsVisibleLocal && parent;
        }
    }


    /// <summary>
    ///     Gets the child Ui Element at specified index.
    ///     returns null in case of invalid index.
    /// </summary>
    /// <param name="i">index of the child Ui Element.</param>
    /// <returns>the child Ui Element.</returns>
    [SkipImGuiReflection]
    public Element this[int i] {
        get {
            if (children_pointers == null) {
                ui.AddToLog(tName + " children_pointers==null", MessType.Error);
                return null;
            }
            if (children_pointers.Length <= i) {
                return null;
            }
            return new Element(children_pointers[i], tName + ".chld[" + i + "]");
        }
    }



    /// <summary>
    /// Must be corrected for current window position and game scale like: this result / ui.screen_k + ui.w_offs
    /// </summary>
    /// <returns></returns>
    public virtual RectangleF get_client_rectangle {
        get {
            if (Address == default) return RectangleF.Empty;
            var vPos = GetParentPos();
            float width = ui.camera.Width;
            float height = ui.camera.Height;
            var ratioFixMult = width / height / 1.6f;
            var xScale = width / 2560f / ratioFixMult;
            var yScale = height / 1600f;

            var rootScale = ui.states.ingame_state.UiRoot.Scale;
            var num = (vPos.X + X * Scale / rootScale) * xScale;
            var num2 = (vPos.Y + Y * Scale / rootScale) * yScale;
            return new RectangleF(num, num2, xScale * Width * Scale / rootScale, yScale * Height * Scale / rootScale);
        }
    }

    private List<Element> GetParentChain() {
        var list = new List<Element>();

        if (Address == IntPtr.Zero)
            return list;
        var ui_root = ui.states.ingame_state.UiRoot;
        if (ui_root == null || ui_root.Address == IntPtr.Zero) {
            ui.AddToLog("GetParentChain err: ui_root err...", MessType.Error);
            return list;
        }

        var hashSet = new HashSet<Element>();
        var parent = Parent;

        while (parent != null && !hashSet.Contains(parent)
                                && ui_root.Address != parent.Address
                                && parent.Address != IntPtr.Zero) {
            if (parent.tName != "gui")
                parent.Tick(parent.Address, tName);
            list.Add(parent);
            hashSet.Add(parent);
            parent = parent.Parent;
        }

        return list;
    }
    public V2 GetParentPos() {
        float num = 0;
        float num2 = 0;
        var rootScale = ui.states.ingame_state.UiRoot.Scale;

        foreach (var current in GetParentChain()) {
            //tofo checl relativePosition here
            num += current.X * current.Scale / rootScale;
            num2 += current.Y * current.Scale / rootScale;
        }

        return new V2(num, num2);
    }



    /// <summary>
    ///     This function was basically parsed/read/decompiled from the game.
    ///     To find this function in the game, follow the data used in this function.
    ///     Although, this function haven't changed since last 3-4 years.
    /// </summary>
    /// <returns>Returns position without applying current element scaling values.</returns>
    private V2 GetUnScaledPosition() {
        if (this.Parent == null) {
            return new V2(X, Y);
        }

        var parentPos = this.Parent.GetUnScaledPosition();
        if (UiElementBaseFuncs.ShouldModifyPos(this.flags)) {
            parentPos += this.Parent.positionModifier;
        }

        if (this.Parent.scaleIndex == this.scaleIndex &&
            this.Parent.localScaleMultiplier == this.localScaleMultiplier) {
            return parentPos + new V2(X, Y);
        }

        var (parentScaleW, parentScaleH) = ui.GameScale.GetScaleValue(
            this.Parent.scaleIndex, this.Parent.localScaleMultiplier);
        var (myScaleW, myScaleH) = ui.GameScale.GetScaleValue(
            this.scaleIndex, this.localScaleMultiplier);
        V2 myPos;
        myPos.X = parentPos.X * parentScaleW / myScaleW
                  + this.X;
        myPos.Y = parentPos.Y * parentScaleH / myScaleH
                  + this.Y;
        return myPos;
    }

    public override string ToString() {
        return tName + " ptr=[" + Address.ToString("X") + "]";
    }
}