﻿using Pandaros.WoWParser.Parser.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Pandaros.WoWParser.Parser.FightMonitor
{
    public class MonitoredFight : IDisposable
    {
        public Guid Id { get; } = Guid.NewGuid();
        public string BossName { get; set; } 
        public Dictionary<string, string> BossPair { get; set; } = new Dictionary<string, string>();
        public Dictionary<string, bool> MonsterID { get; set; } = new Dictionary<string, bool>();
        public DateTime FightStart { get; set; }
        public DateTime FightEnd { get; set; }
        public List<ICombatEvent> MonitoredFightEvents { get; set; } = new List<ICombatEvent>();
        public Dictionary<string, List<ICombatEvent>> PlayerCombatEvents { get; set; } = new Dictionary<string, List<ICombatEvent>>();
        ICombatEvent _lastKnownLog;
        private bool _disposedValue;

        public List<Guid> ChildIds { get; set; } = new List<Guid>();
        public Guid ParentId { get; set; }

        static string ExtractHexPortionWithRegex(string hexValue)
        {
            // Regex to find the sequence after "0xF13" and any number of zeros, capturing the hex sequence that follows
            var regex = new Regex("0xF130*([a-zA-Z0-9_]*)([[a-zA-Z0-9_]{6})");
            var match = regex.Match(hexValue); if (match.Success)
            {
                // Return the captured group which is the interesting portion
                var result = Convert.ToInt32(match.Groups[1].Value, 16);
                return result.ToString();
            }
            return string.Empty;
        }

        public bool AddEvent(ICombatEvent combatEvent, ICombatState state)
        {
            MonitoredFightEvents.Add(combatEvent);

            if (combatEvent.DestFlags.IsPlayer)
            {
                if (!PlayerCombatEvents.TryGetValue(combatEvent.DestName, out var events))
                {
                    events = new List<ICombatEvent>();
                    PlayerCombatEvents[combatEvent.DestName] = events;
                }

                events.Add(combatEvent);
            }

            if (FightMonitorFactory.CombatEventsTriggerInFight.Contains(combatEvent.EventName))
            {
                if (combatEvent.DestFlags.IsNPC && !MonsterID.ContainsKey(combatEvent.DestGuid))
                {
                    MonsterID.Add(combatEvent.DestGuid, false);

                    if (!BossName.Contains(combatEvent.DestName))
                    { 
                        BossName += ", " + combatEvent.DestName + " ID: " + ExtractHexPortionWithRegex(combatEvent.DestGuid);
                        BossPair.Add(combatEvent.DestName, ExtractHexPortionWithRegex(combatEvent.DestGuid));
                    }

                } else if (combatEvent.SourceFlags.IsNPC && !MonsterID.ContainsKey(combatEvent.SourceGuid))
                {
                    MonsterID.Add(combatEvent.SourceGuid, false);

                    if (!BossName.Contains(combatEvent.SourceName))
                        BossName += ", " + combatEvent.SourceName + " ID: " + ExtractHexPortionWithRegex(combatEvent.SourceGuid);
                }
            }



            bool combatOver = false;

            // all monsters dead
            if (combatEvent.EventName == LogEvents.UNIT_DIED && MonsterID.ContainsKey(combatEvent.DestGuid))
            {
                MonsterID[combatEvent.DestGuid] = true;
                bool allDead = true;

                foreach (var val in MonsterID.Values)
                    if (!val)
                    {
                        allDead = val;
                        break;
                    }

                combatOver = allDead;
            }

            if (_lastKnownLog != null && !MonsterID.ContainsKey(combatEvent.DestGuid) && !MonsterID.ContainsKey(combatEvent.SourceGuid))
            {
                var ts = combatEvent.Timestamp.Subtract(_lastKnownLog.Timestamp);

                if (ts.TotalSeconds > 60)
                    combatOver = true;
            }
            else if (MonsterID.ContainsKey(combatEvent.SourceGuid) || MonsterID.ContainsKey(combatEvent.DestGuid))
                _lastKnownLog = combatEvent;

            var NotMonitoredFightEvents = new List<ICombatEvent>();

            if (combatOver)
            {
                for (int i = MonitoredFightEvents.Count - 1; i != 0; i--)
                {
                    if (MonitoredFightEvents[i] == _lastKnownLog)
                    {
                        FightEnd = MonitoredFightEvents[i].Timestamp;
                        break;
                    }
                    else
                    {
                        NotMonitoredFightEvents.Add(MonitoredFightEvents[i]);
                    }
                }

                foreach (var unmonitor in NotMonitoredFightEvents)
                    MonitoredFightEvents.Remove(unmonitor);

                return false;
            }

            return true;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    
                }

                MonsterID = null;
                MonitoredFightEvents = null;
                PlayerCombatEvents = null;
                ChildIds = null;
                _disposedValue = true;
            }
        }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
        }
    }
}
