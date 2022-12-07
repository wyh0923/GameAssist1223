
namespace Stas.GA; 

public class SkillBarElement :Element{
    internal SkillBarElement() : base("SkillBarElement") {
    }
    public new SkillElement this[int k] => (SkillElement)children[k];
    Element _flare_tnt;
    /// <summary>
    /// its always is visible, but have Width=0 if not visible in game
    /// </summary>
    public Element ui_flare_tnt {
        get {
            if (_flare_tnt == null)
                _flare_tnt = GetChildFromIndices((int)chld_count - 1, 0);
            return _flare_tnt;
        }
    }
  
    Element _flare;
    public Element flare {
        get {
            if (_flare == null)
                _flare = GetChildFromIndices(13, 0, 2, 1);
            return _flare;
        }
    }
    public string flare_count => flare?.Text;
    Element _fke;
    public Element flare_key_elem {
        get {
            if (_fke == null) {
                _fke = GetChildFromIndices(13, 0, 2, 0, 0, 1);
            }
            return _fke;
        }
    }


    public Keys flare_key {
        get {
            var val = flare_key_elem.Text;
            return (Keys)Enum.Parse(typeof(Keys), val);// Keys.F9;
        }
    }
    Element _tnt;
    public Element tnt {
        get {
            if (_tnt == null)
                _tnt = GetChildFromIndices(13, 0, 3, 1);
            return _tnt;
        }
    }
    public string tnt_count => tnt?.Text;


    Element _detonate;
    public Element detonate {
        get {
            if (_detonate == null)
                _detonate = GetChildAtIndex((int)chld_count - 1).GetTextElem_by_Str("D");
            return _detonate;
        }
    }
 
    private int GetUnusedPassivePointsAmount() {
        var numberInButton = GetChildAtIndex(3)?.GetChildAtIndex(1);
        if (numberInButton == null || !numberInButton.IsVisible) {
            return 0;
        }
        int result;
        var success = Int32.TryParse(GetChildAtIndex(3)?.GetChildAtIndex(1)?.Text, out result);
        return success ? result : 0;
    }
}
