using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;

namespace Stas.GameAssist {
    internal class NinjaUnique {
        List<string> ninjaUrlsList;
        public NinjaUnique() {
            Console.WriteLine("To get art name from worldItem: \r\nworldItem.Item.RenderArt\r\nComes in a from of \"Art/2DItems/Belts/ElderBelt.dds\"");
            var first_part = @"https://poe.ninja/api/data/ItemOverview?league=Sentinel&type=";
            ninjaUrlsList = new List<string>(){
                first_part+"UniqueArmour&language=en",
                first_part+"UniqueAccessory&language=en",
                first_part+"UniqueFlask&language=en",
                first_part+"&type=UniqueJewel&language=en",
                first_part+"type=UniqueWeapon&language=en"
            };
            Load();
        }
        async void Load() {
            //get ninja uniques information, exclude replicas
            var items = new List<ItemJson>();
            try {
                using (var client = new HttpClient()) {
                    var itemslist = new List<ItemJson>();
                    foreach (string url in ninjaUrlsList) {
                        var response = await client.GetAsync(url);
                        response.EnsureSuccessStatusCode();
                        var resp = await response.Content.ReadAsStringAsync();
                        var tempItems = JsonSerializer.Deserialize<RespData>(resp);
                        itemslist.AddRange(tempItems.lines.Where(x => !x.Name.Contains("Replica")));
                    }

                    items = itemslist;
                }
            } catch (Exception ex) {
                ui.AddToLog("NinjaUnique.Load err:"+ex.ToString());
            }
            items = new List<ItemJson>(items.OrderByDescending(e => e.ExaltedValue).Where(c => c.ChaosValue >= 20.0));

            int k = 0;

            ui.AddToLog($"NinjaUnique total: {items.Count()}");
            foreach (var item in items) {
                item.ArtWorkPath = FindUniqueArt(item.Name);
                Console.WriteLine($"{k} {item.ToString()} ");
                k++;
            }
        }
        public class RespData {
            public List<ItemJson> lines { get; set; }
            public language language { get; set; }
        }
        public class language { 
            public string name { get; set; }
            public string translations { get; set; }
        }
        public class ItemJson {
            public string Name { get; set; }
            public int Links { get; set; }
            public double ChaosValue { get; set; }
            public double ExaltedValue { get; set; }
            public string ArtWorkPath { get; set; }
            /// <inheritdoc />
            public override string ToString() {
                StringBuilder sb = new StringBuilder();
                sb.Append($"{Name}, ");
                if (Links != 0) {
                    sb.Append($"[{Links}L], ");
                }

                sb.Append($":\t\t{ChaosValue}c/{ExaltedValue}ex\t\t");
                sb.Append($" {ArtWorkPath}");
                return sb.ToString();
            }

        }
        string FindUniqueArt(string itemName) {
            WebClient cl = new WebClient();
            Encoding utf8 = Encoding.UTF8;
            string normalizedName = itemName.Replace(" ", "_").Replace("'", "");
            var windows1250 = Encoding.GetEncoding(1250);
            var percentEncoded = WebUtility.UrlEncode(normalizedName);
            string download = cl.DownloadString($"https://poedb.tw/us/{percentEncoded}");

            var doc = new HtmlDocument();
            doc.LoadHtml(download);
            //only one table
            var table = doc.DocumentNode.SelectSingleNode("//table");
            //select tbody from it
            var tableRows = table.SelectSingleNode("tbody");
            //get child nodes (rows)
            var tableRows2 = tableRows.ChildNodes;
            //find the one with art
            var node = tableRows2.FirstOrDefault(yx => yx.InnerHtml.Contains("2DItems"));
            //node.InnerText        "IconArt/2DItems/Belts/InjectorBelt"
            string fin = node?.InnerText.Replace("Icon", "");
            return fin;
        }
    }
}
