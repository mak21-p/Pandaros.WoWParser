﻿using Pandaros.WoWParser.Parser.FightMonitor;
using System;
using System.Collections.Generic;
using System.Text;

namespace Pandaros.WoWParser.Parser
{
    public interface IStatsLogger
    {
        public void Output(string message);
        public void ReportPerSecondNumbers<T>(Dictionary<T, long> stats, string name, MonitoredFight fight, ICombatState state, bool important = false);
        public void Report<T>(Dictionary<T, long> stats, string name, MonitoredFight fight, ICombatState state, bool important = false);
        public void Report<T, G>(Dictionary<T, Dictionary<G, long>> stats, string name, MonitoredFight fight, ICombatState state, bool important = false);
        public void Report<T, G, B>(Dictionary<T, Dictionary<G, Dictionary<B, long>>> stats, string name, MonitoredFight fight, ICombatState state, bool important = false);
        public void ReportTable(List<List<string>> table, string name, MonitoredFight fight, ICombatState state, List<int> length = default(List<int>), bool important = false);
    }
}
