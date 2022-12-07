
namespace Stas.GA;

internal class If_I_Dead : Element {
    internal If_I_Dead() : base("If_I_Dead") {
    }

    public Element res_in_town { get; private set; } = new Element("res_in_town");
    public Element res_at_checkpoint { get; private set; } = new Element("res_at_checkpoint");

    internal override void Tick(IntPtr ptr, string from=null) {
        if (Address == IntPtr.Zero)
            return;
        res_in_town = GetTextElem_by_Str("resurrect in town");
        res_at_checkpoint = GetTextElem_by_Str("resurrect at checkpoint"); 
    }
}
