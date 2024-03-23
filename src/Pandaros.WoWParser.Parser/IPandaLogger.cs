using Pandaros.WoWParser.Parser.Models;
using System;
using System.Collections.Generic;

namespace Pandaros.WoWParser.Parser
{
    public interface IPandaLogger : IStatsLogger
    {
        void Log(string message, params object[] args);
        void LogError(Exception e);
        void LogError(Exception e, string message);
        void LogError(Exception e, string message, params object[] args);
        void AddDamageData(Dictionary<string, string> fightName, List<DamageOutputInfo> damageOutputInfos);
        void AddHealingData(Dictionary<string, string> fightName, List<HealingOutputInfo> healingOutputInfos);
        void MakeJSON();
    }
}