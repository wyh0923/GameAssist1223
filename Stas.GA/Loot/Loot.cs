using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Stas.GA {
    public class Loot {
        static List<string> weapons = new List<string>
                {
                    "One Hand Mace",
                    "Two Hand Mace",
                    "One Hand Axe",
                    "Two Hand Axe",
                    "One Hand Sword",
                    "Two Hand Sword",
                    "Thrusting One Hand Sword",
                    "Bow",
                    "Claw",
                    "Dagger",
                    "Rune Dagger",
                    "Sceptre",
                    "Staff",
                    "Wand"
                };
        public string BaseName { get; } = "";
        public string ClassName { get; } = "";
        public LabelOnGround log;
        public float gdist_to_player => ent.gdist_to_me;
        public Entity ent => log?.ItemOnGround; // item on ground entity
        public Element label => log.Label;
        public int Height { get; }
        public bool IsElder { get; }
        public bool IsIdentified { get; }
        public bool IsRGB { get; }
        public bool IsShaper { get; }
        public bool IsHunter { get; }
        public bool IsRedeemer { get; }
        public bool IsCrusader { get; }
        public bool IsWarlord { get; }
        public bool IsWeapon { get; }
        public int ItemLevel { get; }
       // public Mods mods { get; }
        public readonly Base @base;
       // public readonly Sockets sockets;
        public int MapTier { get; }
        public string Path { get; }
        public int Quality { get; }
        //public ItemRarity Rarity { get; }
        public int Sockets { get; }
        public int Width { get; }
        public bool IsFractured { get; }
        public bool IsMetaItem { get; }
        public int LargestLink { get; }
        public int stack_size { get; }
        public override string ToString() {
            return log.ToString();
        }
        public Loot(LabelOnGround _label, Entity item_ent ) {
            log = _label;
            Path = item_ent.Path;
            //var baseItemType = ui.gc.Files.BaseItemTypes.Translate(Path);
            //if(baseItemType != null) {
            //    ClassName = baseItemType.ClassName;
            //    BaseName = baseItemType.BaseName;
            //    Width = baseItemType.Width;
            //    Height = baseItemType.Height;
            //    //if(weightsRules.TryGetValue(BaseName, out var w)) Weight = w;
            //}
        
            //if(item_ent.GetComp<Quality>(out var quality)) {
            //    Quality = (quality!=null)? quality.ItemQuality:0;
            //}
            //if(item_ent.GetComp<Stack>( out var _stack)) {
            //    stack_size = _stack.Size;
            //}
            //if(item_ent.GetComp<Base>(out @base)) {
            //    IsElder = @base.isElder;
            //    IsShaper = @base.isShaper;
            //    IsHunter = @base.isHunter;
            //    IsRedeemer = @base.isRedeemer;
            //    IsCrusader = @base.isCrusader;
            //    IsWarlord = @base.isWarlord;
            //}
            //if(item_ent.GetComp<Mods>(out var _mods)) {
            //    Rarity = _mods.ItemRarity;
            //    IsIdentified = _mods.Identified;
            //    ItemLevel = _mods.ItemLevel;
            //    IsFractured = _mods.IsFractured;
            //}
            //if(item_ent.GetComp<Sockets>(out sockets)) {
            //    IsRGB = sockets.IsRGB;
            //    Sockets = sockets.NumberOfSockets;
            //    LargestLink = sockets.LargestLinkSize;
            //}

            if(weapons.Any(ClassName.Equals)) 
                IsWeapon = true;
        }
    }

}
