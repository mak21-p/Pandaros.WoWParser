using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pandaros.WoWParser.Parser.Models
{
    public class HealingDetail
    {
        public int Position { get; set; }
        public string SpellName { get; set; }
        public int SpellId { get; set; }
        public long HealingDone { get; set; }
    }

    public class CharacterHealed
    {
        public int Position { get; set; }
        public string Username { get; set; }
        public double HealingDone { get; set; }
        public List<HealingDetail> HealingDetails { get; set; }
    }

    public class HealingOutputInfo
    {
        public int Position { get; set; }
        public string Username { get; set; }
        public double HealingOutput { get; set; }
        public List<CharacterHealed> CharactersHealed { get; set; }
    }

    public static class THealingExtensions
    {
        public static void RankHealing(this List<HealingOutputInfo> healingOutputInfos)
        {
            for (int i = 0; i < healingOutputInfos.Count; i++)
            {
                var healingOutputInfo = healingOutputInfos[i];
                healingOutputInfo.Position = i + 1; // Assign position based on order

                // Process each CharacterHealed within HealingOutputInfo
                for (int j = 0; j < healingOutputInfo.CharactersHealed.Count; j++)
                {
                    var characterHealed = healingOutputInfo.CharactersHealed[j];
                    characterHealed.Position = j + 1; // Assign position based on order

                    // Calculate HealingDone for CharacterHealed
                    characterHealed.HealingDone = characterHealed.HealingDetails.Sum(hd => hd.HealingDone);

                    // Sort HealingDetails by HealingDone in descending order and assign positions
                    characterHealed.HealingDetails = characterHealed.HealingDetails
                        .OrderByDescending(hd => hd.HealingDone)
                        .Select((hd, index) => { hd.Position = index + 1; return hd; })
                        .ToList();
                }

                // Calculate HealingOutput for HealingOutputInfo
                healingOutputInfo.HealingOutput = healingOutputInfo.CharactersHealed.Sum(ch => ch.HealingDone);

                // Sort CharactersHealed by HealingDone in descending order and assign positions
                healingOutputInfo.CharactersHealed = healingOutputInfo.CharactersHealed
                    .OrderByDescending(ch => ch.HealingDone)
                    .Select((ch, index) => { ch.Position = index + 1; return ch; })
                    .ToList();
            }

            // Sort HealingOutputInfos by HealingOutput in descending order and assign positions
            healingOutputInfos = healingOutputInfos
                .OrderByDescending(hoi => hoi.HealingOutput)
                .Select((hoi, index) => { hoi.Position = index + 1; return hoi; })
                .ToList();
        }

        public static void AddOrCreateHealingDetail(this List<HealingDetail> healingDetails, HealingDetail healToAdd )
        {
            var existingDetail = healingDetails.FirstOrDefault(detail => detail.SpellId == healToAdd.SpellId);
            if (existingDetail != null)
            {
                existingDetail.HealingDone += healToAdd.HealingDone;
            }
            else
            {
                healingDetails.Add(healToAdd);
            }
        }

        public static void AddOrCreate(this List<HealingOutputInfo> healingOutputInfos, HealingOutputInfo healingOutputNew, string owner)
        {
            var existingHeal = healingOutputInfos.FirstOrDefault(heal => heal.Username == owner);
            if (existingHeal != null)
            {
                foreach (var character in healingOutputNew.CharactersHealed)
                {
                    var healReceiver = existingHeal.CharactersHealed.FirstOrDefault(receiver => receiver.Username == character.Username);

                    if (healReceiver != null)
                    {
                        foreach (var heal in character.HealingDetails)
                        {
                            var existingSpell = healReceiver.HealingDetails.FirstOrDefault(spell => spell.SpellId == heal.SpellId);
                            if (existingSpell != null)
                            {
                                existingSpell.HealingDone += heal.HealingDone;
                            }
                            else
                            {
                                healReceiver.HealingDetails.Add(heal);
                            }
                        }
                    }
                    else
                    {
                        existingHeal.CharactersHealed.Add(character);
                    }
                }
            }
            else
            {
                healingOutputInfos.Add(healingOutputNew);
            }
        }
    }
}
