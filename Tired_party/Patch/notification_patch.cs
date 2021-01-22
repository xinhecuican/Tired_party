using HarmonyLib;
using MCM.Abstractions.Settings.Base.Global;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.LogEntries;
using TaleWorlds.CampaignSystem.SandBox.CampaignBehaviors;
using TaleWorlds.Core;

namespace Tired_party.Patch
{
    [HarmonyPatch(typeof(DefaultLogsCampaignBehavior))]
    class notification_patch
    {
        [HarmonyPatch("OnPrisonerTaken")]
        [HarmonyPrefix]
        public static bool prison_taken_prefix(PartyBase party, Hero hero)
        {
            if(GlobalSettings<mod_setting>.Instance.is_ban_capture_information)
            {
                TakePrisonerLogEntry entry = new TakePrisonerLogEntry(party, hero);
                Party_tired.Current.information[1].add_information(entry.ToString(), (float)CampaignTime.Now.ToHours);
                Party_tired.Current.information[0].add_information(entry.ToString(), (float)CampaignTime.Now.ToHours);
                return false;
            }
            return true;
        }

        [HarmonyPatch("OnPrisonerReleased")]
        [HarmonyPrefix]
        public static bool prison_released_prefix(Hero hero, PartyBase party, IFaction captuererFaction, EndCaptivityDetail detail)
        {
            if(GlobalSettings<mod_setting>.Instance.is_ban_release_information)
            {
                EndCaptivityLogEntry entry = new EndCaptivityLogEntry(hero, captuererFaction, detail);
                Party_tired.Current.information[1].add_information(entry.ToString(), (float)CampaignTime.Now.ToHours);
                Party_tired.Current.information[0].add_information(entry.ToString(), (float)CampaignTime.Now.ToHours);
                return false;
            }
            return true;
        }

        [HarmonyPatch("OnHeroesMarried")]
        [HarmonyPrefix]
        public static bool hero_married_prefix(Hero marriedHero, Hero marriedTo, bool showNotification)
        {
            if(Hero.MainHero == marriedHero || Hero.MainHero == marriedTo)
            {
                return true;
            }
            if(GlobalSettings<mod_setting>.Instance.is_ban_married_information)
            {
                CharacterMarriedLogEntry entry = new CharacterMarriedLogEntry(marriedHero, marriedTo);
                Party_tired.Current.information[1].add_information(entry.ToString(), (float)CampaignTime.Now.ToHours);
                Party_tired.Current.information[0].add_information(entry.ToString(), (float)CampaignTime.Now.ToHours);
                return false;
            }
            return true;
        }
    }
}
