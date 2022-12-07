using System.Text;
namespace Stas.GA;
public class Sockets : EntComp {
    public Sockets(IntPtr address) : base(address) {
    }
    internal override void Tick(IntPtr ptr, string from=null) {
        Address = ptr;
        if (Address == IntPtr.Zero )
            return;
        IsRGB = Links.Any(current => current.Length >= 3 
                        && current.Contains(1) 
                        && current.Contains(2) 
                        && current.Contains(3));
    }
    public int NumberOfSockets => SocketList.Count;
    public bool IsRGB { get; private set; }

    public int LargestLinkSize {
        get {
            if (Address == 0) return 0;
            var pLinkStart = ui.m.Read<long>(Address + 0x60);
            var pLinkEnd = ui.m.Read<long>(Address + 0x68);
            var LinkGroupingCount = pLinkEnd - pLinkStart;
            if (LinkGroupingCount <= 0 || LinkGroupingCount > 6) return 0;
            var BiggestLinkGroupSize = 0;

            for (var i = 0; i < LinkGroupingCount; i++) {
                int LinkGroupSize = ui.m.Read<byte>(pLinkStart + i);
                if (LinkGroupSize > BiggestLinkGroupSize) BiggestLinkGroupSize = LinkGroupSize;
            }

            return BiggestLinkGroupSize;
        }
    }

    public List<int[]> Links {
        get {
            var list = new List<int[]>();
            if (Address == 0) return list;
            var pLinkStart = ui.m.Read<long>(Address + 0x60);
            var pLinkEnd = ui.m.Read<long>(Address + 0x68);
            var LinkGroupingCount = pLinkEnd - pLinkStart;
            if (LinkGroupingCount <= 0 || LinkGroupingCount > 6) return list;
            var LinkCounter = 0;
            var socketList = SocketList;

            for (var i = 0; i < LinkGroupingCount; i++) {
                int LinkGroupSize = ui.m.Read<byte>(pLinkStart + i);
                var array = new int[LinkGroupSize];

                for (var j = 0; j < LinkGroupSize; j++) {
                    array[j] = socketList[j + LinkCounter];
                }

                list.Add(array);
                LinkCounter += LinkGroupSize;
            }

            return list;
        }
    }

    public List<int> SocketList {
        get {
            var list = new List<int>();
            if (Address == 0) return list;
            var num = Address + 0x18;

            for (var i = 0; i < 6; i++) {
                var num2 = ui.m.Read<int>(num);
                if (num2 >= 1 && num2 <= 6) list.Add(ui.m.Read<int>(num));
                num += 4;
            }

            return list;
        }
    }

    public List<string> SocketGroup {
        get {
            var list = new List<string>();

            foreach (var current in Links) {
                var sb = new StringBuilder();

                foreach (var color in current) {
                    switch (color) {
                        case 1:
                            sb.Append("R");
                            break;
                        case 2:
                            sb.Append("G");
                            break;
                        case 3:
                            sb.Append("B");
                            break;
                        case 4:
                            sb.Append("W");
                            break;
                        case 5:
                            sb.Append('A');
                            break;
                        case 6:
                            sb.Append("O");
                            break;
                    }
                }

                list.Add(sb.ToString());
            }

            return list;
        }
    }

    public List<SocketedGem> SocketedGems {
        get {
            var rezult = new List<SocketedGem>();

            var startAddress = Address + 0x30;

            for (var i = 0; i < 6; i++) {
                var objAddress = ui.m.Read<long>(startAddress);

                if (objAddress != 0)
                    rezult.Add(new SocketedGem { SocketIndex = i, GemEntity = new Entity(ui.m.Read<IntPtr>(startAddress)) });

                startAddress += 8;
            }

            return rezult;
        }
    }

  

    public class SocketedGem {
        public Entity GemEntity;
        public int SocketIndex;
    }
}
