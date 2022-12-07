namespace Stas.GA; 

public class ChatBoxElem : Element {
    internal ChatBoxElem() : base("ChatBoxElem") {
    }
    internal override void Tick(nint ptr = 0, string from = null) {
        base.Tick(ptr, from);
        isOpened = ui.m.Read<byte>(Address + 0x17f);
    }
    public byte isOpened { get; set; }
    int last_count = 0;
    List<string> _mesa = new List<string>();
    public Element up_arrow => GetChildFromIndices(1, 2, 2, 0);
    public Element down_arrow => GetChildFromIndices(1, 2, 2, 1);
    public Element open_inp_btn => GetChildFromIndices(1, 2, 2, 2);
    public Element input => GetChildAtIndex(3);
    public Element mess_elems => GetChildFromIndices(1, 2, 1);
    public Element lme; // last_message_elem;
    public Element arrows => GetChildFromIndices(1, 2, 2);
   DateTime next_mess_upd = DateTime.Now;

  

    public List<string> messages {
        get {
            if (mess_elems != null && next_mess_upd < DateTime.Now) {
                while (mess_elems.chld_count > last_count) {
                    try {
                        lme = new Element(mess_elems[last_count].Address, tName+".chld["+ last_count + "]");
                        _mesa.Add(lme.Text);
                    }
                    catch (Exception ex) {
                        ui.AddToLog(tName + " Err: " + ex.Message, MessType.Error);
                    }
                    last_count += 1;
                }
                next_mess_upd = next_mess_upd.AddSeconds(1);
            }
            return _mesa;
        }
    }
}
