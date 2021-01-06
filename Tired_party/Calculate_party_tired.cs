using MCM.Abstractions.Settings.Base.Global;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;

namespace Tired_party
{
    class Calculate_party_tired
    {
        /*
         * 和等级挂钩
         * 和伤员数量挂钩
         * 和俘虏数量挂钩
         * 和地形挂钩（过于困难）
         */
        public static float calculate_ratio(MobileParty mobile)
        {
            float persist_rate = 0f;
            if (mobile.Army == null)
            {
                TroopRoster roster = mobile.MemberRoster;
                TroopRoster prison_roster = mobile.PrisonRoster;
                int all_tier = 0;
                foreach (TroopRosterElement characterObject in roster)
                {
                    all_tier += characterObject.Character.Tier * characterObject.Number;
                }
                int hero_count = roster.TotalHeroes;
                all_tier += hero_count * 6;
                
                int total_count = roster.TotalManCount;
                float average_tier = (float)all_tier / (float)total_count; //和等级挂钩
                int wounded = roster.TotalWounded;
                int captured = prison_roster.TotalWounded + prison_roster.TotalManCount;
                float persist_hour = (average_tier * (average_tier + 1) / 20 + GlobalSettings<mod_setting>.Instance.persist_time) * 24;
                float reduce_rate = (float)((wounded * 0.5f + 1.2f * captured) / total_count >= 1 ? 0.2 : 0.2 * (wounded * 0.5f + 1.2f * captured) / total_count);
                persist_hour = persist_hour * (1 - reduce_rate);
                persist_rate = 1 / persist_hour;
            }
            else if(mobile.Army != null && mobile.Army.LeaderParty != mobile)
            {
                tired_party_data data = null;
                Party_tired.Current.Party_tired_rate.TryGetValue(mobile.Army.LeaderParty, out data);
                if(data == null)
                {
                    persist_rate = calculate_army(mobile.Army.LeaderParty);
                }
                else
                {
                    persist_rate = data.Reduce_rate;
                }
            }
            else
            {
                persist_rate = calculate_army(mobile);
            }
            return persist_rate;
        }

        private static float calculate_army(MobileParty mobile)
        {
            int all_tier = 0;
            int hero_count = 0;
            int total_count = 0;
            int wounded = 0;
            int captured = 0;
            foreach(MobileParty party in mobile.Army.LeaderPartyAndAttachedParties)
            {
                TroopRoster member_roster = party.MemberRoster;
                TroopRoster prison_roster = party.PrisonRoster;
                foreach (TroopRosterElement characterObject in member_roster)
                {
                    all_tier += characterObject.Character.Tier * characterObject.Number;
                }
                hero_count += member_roster.TotalHeroes;
                total_count += member_roster.TotalManCount;
                wounded += member_roster.TotalWounded;
                captured += prison_roster.TotalManCount;
            }
            float average_tier = (float)(all_tier + 6 * hero_count) / (float)total_count;
            float persist_hour = (average_tier * (average_tier + 1) / 20 + GlobalSettings<mod_setting>.Instance.persist_time) * 24;
            float reduce_rate = (float)((wounded * 0.5f + 1.2f * captured) / total_count >= 1 ? 0.2 : 0.2 * (wounded * 0.5f + 1.2f * captured) / total_count);
            persist_hour = persist_hour * (1 - reduce_rate);
            return 1 / persist_hour;
        }

        public static int calculate_remaining_hours(tired_party_data value)
        {
            int remaining_hour = 0;
            float temp = value.Now;
            while(temp > 0)
            {
                temp -= value.Reduce_rate;
                remaining_hour++;
            }
            return remaining_hour;
        }
    }
}
