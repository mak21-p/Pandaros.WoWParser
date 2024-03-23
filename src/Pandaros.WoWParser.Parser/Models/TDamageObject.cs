using Autofac.Features.OwnedInstances;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pandaros.WoWParser.Parser.Models
{
    public class DamageDetail
    {
        public int Position { get; set; }
        public string SpellName { get; set; }
        public int SpellId { get; set; }
        public double DamageOutput { get; set; }
    }
    public class DamageOutputInfo 
    {
        public string Username { get; set; }
        public double TotalDamageOutput { get; set; } 
        public List<DamageDetail> DamageDetails { get; set; }
    }

    public static class TDamageExtensions
    {
        public static void AddOrCreate(this List<DamageOutputInfo> damageOutputInfos, DamageDetail damageDetail, string owner)
        {
            var existingDetail = damageOutputInfos.FirstOrDefault(damageOutput => damageOutput.Username == owner);
            if (existingDetail != null)
            {
                existingDetail.TotalDamageOutput += damageDetail.DamageOutput;
                var existingDamageDetail = existingDetail.DamageDetails.FirstOrDefault(detail => detail.SpellId == damageDetail.SpellId);
                if (existingDamageDetail != null)
                {
                    existingDamageDetail.DamageOutput += damageDetail.DamageOutput;
                }
                else
                {
                    existingDetail.DamageDetails.Add(damageDetail);
                }
            }
            else
            {
                damageOutputInfos.Add(new DamageOutputInfo { Username = owner, TotalDamageOutput = damageDetail.DamageOutput, DamageDetails = new List<DamageDetail> { damageDetail } });
            }
        }

        public static void RankDamage(this List<DamageOutputInfo> damageOutputInfos)
        {
            foreach (DamageOutputInfo damageOutput in damageOutputInfos)
            {
                damageOutput.DamageDetails = damageOutput.DamageDetails
                .OrderByDescending(detail => detail.DamageOutput)
                .ToList();

                for (int i = 0; i < damageOutput.DamageDetails.Count; i++)
                {
                    damageOutput.DamageDetails[i].Position = i + 1;
                }
            }
        }
    }
}
