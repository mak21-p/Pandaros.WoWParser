using Pandaros.WoWParser.Parser.FightMonitor;
using Pandaros.WoWParser.Parser.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using MongoDB.Bson;
using Newtonsoft.Json;

namespace Pandaros.WoWParser.Parser.Calculators
{
    public class DamageDoneBySpellCalculator : BaseCalculator
    {
        Dictionary<string, Dictionary<string, long>> _damageSpellByPlayer = new Dictionary<string, Dictionary<string, long>>();
        List<DamageOutputInfo> damageOutputInfos = new List<DamageOutputInfo>();

        public DamageDoneBySpellCalculator(IPandaLogger logger, IStatsLogger reporter, ICombatState state, MonitoredFight fight) : base(logger, reporter, state, fight)
        {
            ApplicableEvents = new List<string>()
            {
                LogEvents.SPELL_DAMAGE,
                LogEvents.RANGE_DAMAGE,
                LogEvents.SWING_DAMAGE,
                LogEvents.SPELL_PERIODIC_DAMAGE,
                LogEvents.DAMAGE_SHIELD
            };
        }

        public override void CalculateEvent(ICombatEvent combatEvent)
        {
            var damage = (IDamage)combatEvent;

            if (combatEvent.SourceFlags.FlagType == UnitFlags.UnitFlagType.Player)
                AddDamage(combatEvent.SourceName, combatEvent, damage);
            else if (State.TryGetSourceOwnerName(combatEvent, out var owner))
            {
                AddDamage(owner, combatEvent, damage);
            }
        }

        private void AddDamage(string owner, ICombatEvent combatEvent, IDamage damage)
        {
            switch (combatEvent.EventName)
            {
                case LogEvents.SWING_DAMAGE:
                    _damageSpellByPlayer.AddValue(owner, "Auto attack", damage.Damage);
                    damageOutputInfos.AddOrCreate(new DamageDetail { Position = 1, DamageOutput = damage.Damage, SpellId = 1, SpellName = "Auto Attack" }, owner);
                    // damageDetails.Add(new DamageDetail { Position = 1, DamageOutput = damage.Damage, SpellId = 1, SpellName = "Auto Attack" });
                    break;

                case LogEvents.RANGE_DAMAGE:
                    _damageSpellByPlayer.AddValue(owner, "Shoot", damage.Damage);
                    damageOutputInfos.AddOrCreate(new DamageDetail { Position = 1, DamageOutput = damage.Damage, SpellId = 2, SpellName = "Shoot" }, owner);
                    // damageDetails.Add(new DamageDetail { Position = 1, DamageOutput = damage.Damage, SpellId = 2, SpellName = "Shoot" });
                    break;

                case LogEvents.SPELL_DAMAGE:
                case LogEvents.SPELL_PERIODIC_DAMAGE:
                    var spell = (ISpell)combatEvent;
                    _damageSpellByPlayer.AddValue(owner, spell.SpellName + " ID: " + spell.SpellId, damage.Damage);
                    damageOutputInfos.AddOrCreate(new DamageDetail { Position = 1, DamageOutput = damage.Damage, SpellId = spell.SpellId, SpellName = spell.SpellName }, owner);
                    // damageDetails.Add(new DamageDetail { Position = 1, DamageOutput = damage.Damage, SpellId = spell.SpellId, SpellName = spell.SpellName });
                    break;
            }
        }

        public override void FinalizeFight(ICombatEvent combatEvent)
        {
            _statsReporting.Report(_damageSpellByPlayer, "Damage Rankings", Fight, State);
            damageOutputInfos.RankDamage();
            _logger.Log(JsonConvert.SerializeObject(damageOutputInfos));
            _logger.AddDamageData(Fight.BossPair, damageOutputInfos);
        }

        public override void StartFight(ICombatEvent combatEvent)
        {
            
        }
    }
}
