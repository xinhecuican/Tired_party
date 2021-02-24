using MCM.Abstractions.Settings.Base.Global;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.SandBox.GameComponents.Party;
using TaleWorlds.Localization;

namespace Tired_party.Model
{
    class Morale_model : DefaultPartyMoraleModel
    {
        public override float GetDefeatMoraleChange(PartyBase party)
        {
            return base.GetDefeatMoraleChange(party);
        }

        public override int GetDailyNoWageMoralePenalty(MobileParty party)
        {
            return base.GetDailyNoWageMoralePenalty(party);
        }

        public override ExplainedNumber GetEffectivePartyMorale(MobileParty mobileParty, bool includeDescription = false)
        {
            ExplainedNumber base_ans = base.GetEffectivePartyMorale(mobileParty, includeDescription);
            if (Party_tired.Current.Party_tired_rate.ContainsKey(mobileParty) && !GlobalSettings<mod_setting>.Instance.is_ban)
            {

                
                if (GlobalSettings<mod_setting>.Instance.is_ban_army && mobileParty.Army != null)
                {
                    return base_ans;
                }
                float morale_num = Party_tired.Current.Party_tired_rate.ContainsKey(mobileParty) ? Party_tired.Current.Party_tired_rate[mobileParty].Morale : 0f;
                TextObject text = null;
                if(includeDescription)
                {
                    text = new TextObject("tired party");
                }
                base_ans.Add(-(morale_num * GlobalSettings<mod_setting>.Instance.morale_reduce), text) ;
                return base_ans;
            }
            return base_ans;
        }

        public override float HighMoraleValue => base.HighMoraleValue;

        public override int GetTroopDesertionThreshold(MobileParty party)
        {
            return base.GetTroopDesertionThreshold(party);
        }

        public override int NumberOfDesertersDueToPaymentRatio(MobileParty mobileParty)
        {
            return base.NumberOfDesertersDueToPaymentRatio(mobileParty);
        }

        public override int GetDailyStarvationMoralePenalty(PartyBase party)
        {
            return base.GetDailyStarvationMoralePenalty(party);
        }

        public override float GetStandardBaseMorale(PartyBase party)
        {
            return base.GetStandardBaseMorale(party);
        }

        public override float GetVictoryMoraleChange(PartyBase party)
        {
            return base.GetVictoryMoraleChange(party);
        }
    }
}