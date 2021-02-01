using MCM.Abstractions.Settings.Base.Global;
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

        public override void AddExtraAmmo(Agent agent)
        {
            base.AddExtraAmmo(agent);
        }

        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }

        public override float GetDifficultyModifier()
        {
            return base.GetDifficultyModifier();
        }

        public override float GetEffectiveMaxHealth(Agent agent)
        {
            return base.GetEffectiveMaxHealth(agent);
        }

        public override int GetEffectiveSkill(BasicCharacterObject agentCharacter, IAgentOriginBase agentOrigin, Formation agentFormation, SkillObject skill)
        {
            return base.GetEffectiveSkill(agentCharacter, agentOrigin, agentFormation, skill);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override float GetInteractionDistance(Agent agent)
        {
            return base.GetInteractionDistance(agent);
        }
        public override float GetMaxCameraZoom(Agent agent)
        {
            return base.GetMaxCameraZoom(agent);
        }
        public override string GetMissionDebugInfoForAgent(Agent agent)
        {
            return base.GetMissionDebugInfoForAgent(agent);
        }

        public override float GetWeaponInaccuracy(Agent agent, WeaponComponentData weapon, int weaponSkill)
        {
            return base.GetWeaponInaccuracy(agent, weapon, weaponSkill);
        }

        public override void InitializeAgentStats(Agent agent, Equipment spawnEquipment, AgentDrivenProperties agentDrivenProperties, AgentBuildData agentBuildData)
        {
            base.InitializeAgentStats(agent, spawnEquipment, agentDrivenProperties, agentBuildData);
        }
    }
}
