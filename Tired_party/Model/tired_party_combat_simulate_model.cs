﻿using MCM.Abstractions.Settings.Base.Global;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.SandBox.GameComponents.Map;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade;
using TaleWorlds.TwoDimension;

namespace Tired_party.Model
{
    class tired_party_combat_simulate_model : DefaultCombatSimulationModel
    {
        public override int SimulateHit(CharacterObject strikerTroop, CharacterObject strikedTroop, PartyBase strikerParty, PartyBase strikedParty, float strikerAdvantage, MapEvent battle)
        {
            float num = base.SimulateHit(strikerTroop, strikedTroop, strikerParty, strikedParty, strikerAdvantage, battle);
            ExplainedNumber explainedNumber = new ExplainedNumber((float)num, false);
            float strikeradvantage = 1f;
            float strikedadvantage = 1f;
            if (strikerParty.IsMobile)
            {
                strikeradvantage = Party_tired.Current.Party_tired_rate.ContainsKey(strikerParty.MobileParty)
                    ? Party_tired.Current.Party_tired_rate[strikerParty.MobileParty].Now : 1f;
            }
            if (strikedParty.IsMobile)
            {
                strikedadvantage = Party_tired.Current.Party_tired_rate.ContainsKey(strikedParty.MobileParty)
                    ? Party_tired.Current.Party_tired_rate[strikedParty.MobileParty].Now : 1f;
            }
            if (Math.Abs(strikerAdvantage - strikedadvantage) < 0.2f)
            {
                return (int)explainedNumber.ResultNumber;
            }
            else if (strikeradvantage - strikedadvantage > 0.2f)
            {
                explainedNumber.AddFactor((float)Math.Pow((strikerAdvantage - strikedadvantage - 0.2) / 1, 4), new TextObject("tired influence"));
            }

            return (int)explainedNumber.ResultNumber;
        }


    }
}