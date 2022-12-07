using ImGuiNET;
using System.Reflection;
namespace Stas.GA;
/// <summary>
///     Points to a Memory location and reads/understands all the data from there.
///     CurrentAreaInstance in remote memory location changes w.r.t time or event. Due to this,
///     each remote memory object requires to implement a time/event based coroutine.
/// </summary>
public abstract class RemoteObjectBase {
    protected string _tname;
    /// <summary>
    /// this.Type.Name for show in debug
    /// </summary>
    public string tName => _tname;
    /// <summary>
    ///     Initializes a new instance of the <see cref="RemoteObjectBase" /> class.
    /// </summary>
    /// <param name="address">address of the remote memory object.</param>
    internal RemoteObjectBase(IntPtr ptr, object obj=null) { 
        Address = ptr;
        _tname = GetType().Name;
        if (tName == "ActorSkill" && obj != null) {
            ((Skill)this).setActor((Actor)obj);
        }else if (tName == "ActionWrapper" && obj != null) {
            ((ActionWrapper)this).setActor((Actor)obj);
        }
        //if (ptr != default)
        //    Tick(ptr, tName + "()");
    }
    internal RemoteObjectBase() {
        var here = this.GetType().FullName;
        ui.AddToLog(tName + "() err: debug me", MessType.Error);
    }
    virtual protected void Init(string from) { }
    public IntPtr Address { get; protected private set; }

    /// <summary>
    ///     Knows how to clean up the object.
    /// </summary>
    protected abstract void CleanUpData();
    internal abstract void Tick(IntPtr ptr, string from=null);
       
    #region ImGui old trash
    /// <summary>
    ///     Converts the <see cref="RemoteObjectBase" /> to ImGui Widget via reflection.
    ///     By default, only knows how to convert <see cref="address" /> field
    ///     and <see cref="RemoteObjectBase" /> properties of the calling class.
    ///     For details on which specific properties are ignored read
    ///     <see cref="RemoteObjectBase.GetToImGuiMethods" /> method description.
    ///     Any other properties or fields of the derived <see cref="RemoteObjectBase" />
    ///     class should be handled by that class.
    /// </summary>
    internal virtual void ToImGui() {
        var propFlags = BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance;
        var properties = RemoteObjectBase.GetToImGuiMethods(this.GetType(), propFlags, this);
        ImGuiExt.IntPtrToImGui("Address", this.Address);
        foreach (var property in properties) {
            if (ImGui.TreeNode(property.Name)) {
                property.ToImGui.Invoke(property.Value, null);
                ImGui.TreePop();
            }
        }
    }

    /// <summary>
    ///     Iterates over properties of the given class via reflection
    ///     and yields the <see cref="RemoteObjectBase" /> property name and its
    ///     <see cref="RemoteObjectBase.ToImGui" /> method. Any property
    ///     that doesn't have both the getter and setter method are ignored.
    /// </summary>
    /// <param name="classType">Type of the class to traverse.</param>
    /// <param name="propertyFlags">flags to filter the class properties.</param>
    /// <param name="classObject">Class object, or null for static class.</param>
    /// <returns>Yield the <see cref="RemoteObjectBase.ToImGui" /> method.</returns>
    internal static IEnumerable<RemoteObjectPropertyDetail> GetToImGuiMethods(Type classType,
                    BindingFlags propertyFlags, object classObject) {
        var methodFlags = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public;
        var properties = classType.GetProperties(propertyFlags).ToList();
        for (var i = 0; i < properties.Count; i++) {
            var property = properties[i];
            if (Attribute.IsDefined(property, typeof(SkipImGuiReflection))) {
                continue;
            }

            var propertyValue = property.GetValue(classObject);
            if (propertyValue == null) {
                continue;
            }

            var propertyType = propertyValue.GetType();

            if (!typeof(RemoteObjectBase).IsAssignableFrom(propertyType)) {
                continue;
            }

            yield return new RemoteObjectPropertyDetail {
                Name = property.Name,
                Value = propertyValue,
                ToImGui = propertyType.GetMethod("ToImGui", methodFlags)
            };
        }
    }
    //public bool b_ready { get; private set; }
    //public void OnPerFrame(bool need_w8=false) {
    //    if (this.Address != IntPtr.Zero) {
    //        if(need_w8)
    //            b_ready = false;
    //        this.UpdateData(false);
    //        b_ready = true;
    //    }
    //}
    /// <summary>
    /// Attribute to put on the properties that you want to skip in <see cref="GetToImGuiMethods"/> method.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    protected class SkipImGuiReflection : Attribute {
    }
    #endregion
}

public abstract class EntComp : RemoteObjectBase {
    public EntComp(IntPtr address) : base(address) { }
    IntPtr _optr = IntPtr.Zero;
    /// <summary>
    /// ovner entity ptr
    /// </summary>
    public IntPtr owner_ptr { get {
            if (_optr == IntPtr.Zero) {
                _optr = ui.m.Read<IntPtr>(Address + 8);//entity ptr
            }
            return _optr;
        } }

    DateTime last_meta = DateTime.MinValue;
    string _md;
    public string metadata {
        get {
            if (last_meta == DateTime.MinValue) {
                var ent = new Entity(owner_ptr);
                //var _optr = ui.m.Read<IntPtr>(Address + 8);//entity ptr
                var m_ptr = ui.m.Read<IntPtr>(this.owner_ptr + 8);
                _md = ui.m.ReadStdWString(ui.m.Read<StdWString>(m_ptr + 8L));
                last_meta = DateTime.Now;
            }
            return _md;
        }
    }
    protected override void CleanUpData() {
        _md = null;
        last_meta = DateTime.MinValue;
        _optr = IntPtr.Zero;
    }
}