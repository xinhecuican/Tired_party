using HarmonyLib;
using MCM.Abstractions.Settings.Base.Global;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.SandBox.CampaignBehaviors;
using TaleWorlds.Core;

namespace Tired_party.Patch
{
    [HarmonyPatch(typeof(DefaultNotificationsCampaignBehavior))]
    class notification_patch
    {
        [HarmonyPatch("OnPrisonerTaken")]
        [HarmonyPrepare]
        public static bool on_prison_taken_fix(DefaultNotificationsCampaignBehavior __instance, PartyBase capturer, Hero prisoner)
        {
            return !GlobalSettings<mod_setting>.Instance.is_ban_capture_information;
        }

        [HarmonyPatch("OnPrisonerTaken")]
        [HarmonyPrefix]
        public static void prefix(DefaultNotificationsCampaignBehavior __instance, PartyBase capturer, Hero prisoner)
        {
            InformationManager.DisplayMessage(new InformationMessage("啦啦啦"));
        }

        [HarmonyPatch("OnPrisonerReleased")]
        [HarmonyPrepare]
        public static bool on_prison_release_fix(Hero hero, PartyBase party, IFaction capturerFaction, EndCaptivityDetail detail)
        {
            return !GlobalSettings<mod_setting>.Instance.is_ban_release_information || (hero.Clan != Clan.PlayerClan);
        }

        [HarmonyPatch("OnHeroesMarried")]
        [HarmonyPrepare]
        public static bool on_heros_merried_fix(Hero firstHero, Hero secondHero, bool showNotification)
        {
            return !GlobalSettings<mod_setting>.Instance.is_ban_married_information;
        }
    }
}
