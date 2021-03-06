﻿using MCM.Abstractions.Settings.Base.Global;
using NetworkMessages.FromServer;
using SandBox;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;

namespace Tired_party.Model
{
    class combat_state_model : SandboxAgentStatCalculateModel
    {
        public override void UpdateAgentStats(Agent agent, AgentDrivenProperties agentDrivenProperties)
        {
            base.UpdateAgentStats(agent, agentDrivenProperties);
            if(!agent.IsHuman)
            {
                return;
            }
            PartyBase partyBase = (PartyBase)agent.Origin.BattleCombatant;
            if(partyBase != null && partyBase.IsMobile && Party_tired.Current.Party_tired_rate.ContainsKey(partyBase.MobileParty))
            {
                float now_rate = Party_tired.Current.Party_tired_rate[partyBase.MobileParty].Now;
                float penalty = (0.7f + 0.4f * now_rate) * GlobalSettings<mod_setting>.Instance.combat_effect_rate * 
                    (0.8f + 0.2f * (((CharacterObject)agent.Character).IsHero ? 1f : ((CharacterObject)agent.Character).Tier / 6f));
                agentDrivenProperties.MaxSpeedMultiplier *= penalty;
                agentDrivenProperties.CombatMaxSpeedMultiplier *= penalty;
                agentDrivenProperties.WeaponMaxUnsteadyAccuracyPenalty *= (2 - penalty);
                agentDrivenProperties.ThrustOrRangedReadySpeedMultiplier *= (2 - penalty);
                agentDrivenProperties.SwingSpeedMultiplier *= penalty;
            }
        }
    }
}
