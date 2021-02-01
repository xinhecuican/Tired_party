using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using Tired_party.Helper;

namespace Tired_party.Patch
{
    [HarmonyPatch(typeof(MobileParty))]
    class mobileparty_patch
    {
        [HarmonyPatch("OnAiTickInternal")]
        [HarmonyPostfix]
        public static void ai_tick_postfix(MobileParty __instance)
        {
            if (!Party_tired.Current.Party_tired_rate.ContainsKey(__instance) || __instance.IsMainParty)
            {
                return;
            }
            if (Party_tired.Current.Party_tired_rate[__instance].is_fleeing)
            {
                __instance.SetMoveModeHold();
                return;
            }
            try
            {
                if (Party_tired.Current.Party_tired_rate[__instance].reset_time > 0)
                {
                    switch (__instance.ShortTermBehavior)
                    {
                        case AiBehavior.FleeToPoint:
                            if (is_alert(__instance))
                            {
                                
                                Party_tired.ToggleTent(__instance.Party, false);
                            }
                            else
                            {
                                __instance.SetMoveModeHold();
                            }
                            break;
                        case AiBehavior.EngageParty:
                            Party_tired.ToggleTent(__instance.Party, false);
                            break;
                        case AiBehavior.Hold:
                            break;
                        default:
                            __instance.SetMoveModeHold();
                            break;
                    }
                }
            }
            catch(Exception e)
            {
                MethodInfo methodInfo = MethodBase.GetCurrentMethod() as MethodInfo;
                debug_helper.HandleException(e, methodInfo, "ai_tick_postfix");
            }
        }

        [HarmonyPrefix]
        [HarmonyPatch("OnAiTickInternal")]
        public static void ai_tick_prefix(MobileParty __instance)
        {
            if (!Party_tired.Current.Party_tired_rate.ContainsKey(__instance))
            {
                return;
            }
            if (__instance.Army != null && Party_tired.Current.Party_tired_rate[__instance].reset_time > 0)
            {
                __instance.SetMoveModeHold();
            }
        }

        public static bool is_alert(MobileParty party)
        {
            if (party.AiBehaviorObject == null)
            {
                return false;
            }
            float party_scout = party.LeaderHero != null ? party.LeaderHero.GetSkillValue(DefaultSkills.Scouting) : 0f;
            float enemy_scout = party.AiBehaviorObject.LeaderHero != null ? party.AiBehaviorObject.LeaderHero.GetSkillValue(DefaultSkills.Scouting) : 0f;
            if (0.6f * MBRandom.RandomFloat + 0.4f * Math.Min(1f, (party_scout - enemy_scout + 200f) / 300f) > 0.5f)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
