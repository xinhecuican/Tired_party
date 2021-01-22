﻿/*using HarmonyLib;
using MCM.Abstractions.Settings.Base.Global;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade;
using Tired_party.Helper;

namespace Tired_party.Patch
{
    [HarmonyPatch(typeof(InformationManager))]
    class Informationmanager_patch
    {
        [HarmonyPatch("DisplayMessage")]
        [HarmonyPostfix]
        public static void display_postfix(InformationMessage message)
        {
            Party_tired.Current.information[0].add_information(message.ToString(), (float)CampaignTime.Now.ToHours);
            
        }

        [HarmonyPatch("AddQuickInformation")]
        [HarmonyPostfix]
        public static void quickinformation_postfix(TextObject message, int priorty = 0, BasicCharacterObject announcerCharacter = null, string soundEventPath = "")
        {
            Party_tired.Current.information[0].add_information(message.ToString(), (float)CampaignTime.Now.ToHours);
            Party_tired.Current.information[2].add_information(message.ToString(), (float)CampaignTime.Now.ToHours);
        }

        [HarmonyPatch("AddTooltipInformation")]
        [HarmonyPostfix]
        public static void tooltip_postfix(Type type, params object[] args)
        {
            add_information(type, args);
        }

        private static void add_information(Type type, object[] args)
        {
            if (GlobalSettings<mod_setting>.Instance.is_ban || GlobalSettings<mod_setting>.Instance.is_ban_information)
            {
                return;
            }
            if (type == typeof(Army) && !GlobalSettings<mod_setting>.Instance.is_ban_army)
            {
                Army army = (Army)args[0];
                float army_now_tired = 0;
                if (last_army == army && Campaign.CurrentTime - last_see_hour_army <= 1)
                {
                    return;
                }
                if (!Party_tired.Current.Party_tired_rate.ContainsKey(army.LeaderParty))
                {
                    if (BannerlordConfig.Language.Equals("简体中文"))
                        message_helper.ErrorMessage(army.LeaderParty.Name.ToString() + "没有加入");
                    else
                        message_helper.ErrorMessage(army.LeaderParty.Name.ToString() + " don't add");
                    return;
                }
                foreach (MobileParty party in army.LeaderPartyAndAttachedParties)
                {
                    if (Party_tired.Current.Party_tired_rate.ContainsKey(party))
                    {
                        army_now_tired += Party_tired.Current.Party_tired_rate[party].Now;
                    }
                }
                army_now_tired /= army.Parties.Count;
                float temp = army_now_tired;
                int remain_hours = 0;
                while (army_now_tired > 0)
                {
                    remain_hours++;
                    army_now_tired -= Party_tired.Current.Party_tired_rate[army.LeaderParty].Reduce_rate;
                }
                last_army = army;
                last_see_hour_army = Campaign.CurrentTime;
                if (BannerlordConfig.Language.Equals("简体中文"))
                    message_helper.TechnicalMessage(army.Name.ToString() + "还剩" + remain_hours + "小时(" + show_information(temp) + ")");
                else
                    message_helper.TechnicalMessage(army.Name.ToString() + " remain " + remain_hours + " hours(" + show_information(temp) + ")");
            }
            if (type == typeof(MobileParty))
            {
                MobileParty mobile = (MobileParty)args[0];
                if (last_party == mobile && Campaign.CurrentTime - last_see_hour <= 1)
                {
                    return;
                }
                last_party = mobile;
                last_see_hour = Campaign.CurrentTime;

                if (Party_tired.Current.Party_tired_rate.ContainsKey(mobile))
                {
                    if (BannerlordConfig.Language.Equals("简体中文"))
                    {
                        message_helper.TechnicalMessage(mobile.Name + "还剩" + Calculate_party_tired.calculate_remaining_hours(Party_tired.Current.Party_tired_rate[mobile]).ToString() +
                        "小时(" + show_information(Party_tired.Current.Party_tired_rate[mobile].Now) + ")");
                    }
                    else
                    {
                        message_helper.TechnicalMessage(mobile.Name + " remain " + Calculate_party_tired.calculate_remaining_hours(Party_tired.Current.Party_tired_rate[mobile]).ToString() +
                        " hours(" + show_information(Party_tired.Current.Party_tired_rate[mobile].Now) + ")");
                    }
                }
                else if (!mobile.IsCaravan && !mobile.IsVillager)
                {
                    if (BannerlordConfig.Language.Equals("简体中文"))
                        message_helper.ErrorMessage(mobile.Name.ToString() + "没有加入");
                    else
                        message_helper.ErrorMessage(mobile.Name.ToString() + " don't add");
                }
            }
        }
        private static Army last_army;
        private static float last_see_hour_army = 0;
        private static MobileParty last_party;
        private static float last_see_hour = 0;

        private static string show_information(float rate)
        {
            bool language_is_chinese = BannerlordConfig.Language.Equals("简体中文");
            if (rate > 0.9)
            {
                if (language_is_chinese)
                {
                    return "高昂";
                }
                else
                    return " excited";
            }
            else if (rate > 0.3)
            {
                if (language_is_chinese)
                    return "正常";
                else
                    return "normal";
            }
            else if (rate > 0)
            {
                if (language_is_chinese)
                    return "疲惫";
                else
                    return "tired";
            }
            else
            {
                if (language_is_chinese)
                    return "濒临崩溃";
                else
                    return "Near collapse";
            }
        }
    }
}
*/