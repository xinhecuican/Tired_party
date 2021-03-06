﻿using HarmonyLib;
using MCM.Abstractions.Settings.Base.Global;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.Core.ViewModelCollection;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade;
using Tired_party.Helper;

namespace Tired_party.Patch
{
    [HarmonyPatch(typeof(TooltipVM))]
    class InformationVM_patch
    {
        [HarmonyPatch("OpenTooltip")]
        [HarmonyPostfix]
        public static void OpenTooltipfix(TooltipVM __instance, Type type, object[] args)
        {
            try
            {
                if (GlobalSettings<mod_setting>.Instance.is_ban)
                {
                    return;
                }
                if (type == typeof(Army) && !GlobalSettings<mod_setting>.Instance.is_ban_army)
                {
                    Army army = (Army)args[0];
                    float army_now_tired = 0;

                    if (!Party_tired.Current.Party_tired_rate.ContainsKey(army.LeaderParty))
                    {
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
                    if (__instance.TooltipPropertyList.Count > 0)
                    {
                        __instance.TooltipPropertyList[0].ValueLabel = __instance.TooltipPropertyList[0].ValueLabel +
                           "[" + remain_hours +
                            " " + show_information(temp) + "]";
                    }
                }
                if (type == typeof(MobileParty))
                {
                    MobileParty mobile = (MobileParty)args[0];
                    if (!Party_tired.Current.Party_tired_rate.ContainsKey(mobile))
                    {
                        return;
                    }
                    if (__instance.TooltipPropertyList.Count > 0)
                    {
                        __instance.TooltipPropertyList[0].ValueLabel = __instance.TooltipPropertyList[0].ValueLabel 
                            + "[" + Calculate_party_tired.calculate_remaining_hours(Party_tired.Current.Party_tired_rate[mobile]).ToString() +
                            " " + show_information(Party_tired.Current.Party_tired_rate[mobile].Now) + "]";
                    }
                }
            }
            catch(Exception e)
            {
                MethodInfo methodInfo = MethodBase.GetCurrentMethod() as MethodInfo;
                debug_helper.HandleException(e, methodInfo, "harmony patch open tool tip fix error");
            }
        }

        private  static string show_information(float rate)
        {
            bool language_is_chinese = BannerlordConfig.Language.Equals("简体中文");
            if (rate > 0.8)
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
