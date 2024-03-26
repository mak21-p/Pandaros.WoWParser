using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pandaros.WoWParser.Parser.Models
{
    public class TJSONData
    {
        public List<FightName> fightNames { get; set; }
        public List<DamageOutputInfo> damageOutputInfo { get; set; }
        public List<HealingOutputInfo> healingOutputInfo { get; set; }
    }

    public class FightName
    {
        public string Name { get; set; }
        public string ID { get; set; }
    }
        

    public static class TJSONDataExtensions
    {
        public static bool EqualsCustom(this List<FightName> x, List<FightName> y)
        {
            return x.Count == y.Count && !x.Except(y).Any();
        }
    }
}
