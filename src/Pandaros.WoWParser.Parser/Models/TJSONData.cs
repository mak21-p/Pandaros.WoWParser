using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pandaros.WoWParser.Parser.Models
{
    public class TJSONData
    {
        public Dictionary<string, string> fightName {  get; set; }
        public List<DamageOutputInfo> damageOutputInfos { get; set; }
        public List<HealingOutputInfo> healingOutputInfos { get; set; }
    }

    public static class TJSONDataExtensions
    {
        public static bool EqualsCustom(this Dictionary<string, string> x, Dictionary<string, string> y)
        {
            return x.Count == y.Count && !x.Except(y).Any();
        }
    }
}
