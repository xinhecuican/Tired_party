﻿using Helpers;
using NetworkMessages.FromServer;
using SandBox;
using SandBox.Source.Missions;
using SandBox.Source.Missions.Handlers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Source.Missions;
using TaleWorlds.MountAndBlade.Source.Missions.Handlers.Logic;

namespace Tired_party.sneak_attack
{
    class sneak_mission 
    {
        public static Mission open_sneak_mission(string scene)
        {
			Party_tired.is_sneak_mission = true;
			bool isPlayerSergeant = MobileParty.MainParty.MapEvent.IsPlayerSergeant();
			bool isPlayerInArmy = MobileParty.MainParty.Army != null;
			List<string> heroesOnPlayerSideByPriority = HeroHelper.OrderHeroesOnPlayerSideByPriority();
			return MissionState.OpenNew("Battle", SandBoxMissions.CreateSandBoxMissionInitializerRecord(scene, "", false), delegate (Mission mission)
			{
				MissionBehaviour[] array = new MissionBehaviour[27];
				array[0] = new MissionOptionsComponent();
				array[1] = new CampaignMissionComponent();
				array[2] = new BattleEndLogic();
				array[3] = new MissionCombatantsLogic(MobileParty.MainParty.MapEvent.InvolvedParties, PartyBase.MainParty, MobileParty.MainParty.MapEvent.GetLeaderParty(BattleSideEnum.Defender), MobileParty.MainParty.MapEvent.GetLeaderParty(BattleSideEnum.Attacker), Mission.MissionTeamAITypeEnum.FieldBattle, isPlayerSergeant);
				array[4] = new MissionDefaultCaptainAssignmentLogic();
				array[5] = new BattleMissionStarterLogic();
				array[6] = new BattleSpawnLogic("battle_set");
				array[7] = new AgentBattleAILogic();
				array[8] = new MissionAgentSpawnLogic(new IMissionTroopSupplier[]
				{
					new PartyGroupTroopSupplier(MapEvent.PlayerMapEvent, BattleSideEnum.Defender, null),
					new PartyGroupTroopSupplier(MapEvent.PlayerMapEvent, BattleSideEnum.Attacker, null)
				}, PartyBase.MainParty.Side);
				array[9] = new sneak_controller(PartyBase.MainParty.Side);
				array[10] = new AgentFadeOutLogic();
				array[11] = new BattleObserverMissionLogic();
				array[12] = new BattleAgentLogic();
				array[13] = new MainMountAgentLogic();
				array[14] = new AgentVictoryLogic();
				array[15] = new MissionDebugHandler();
				array[16] = new MissionAgentPanicHandler();
				array[17] = new MissionHardBorderPlacer();
				array[18] = new MissionBoundaryPlacer();
				array[19] = new MissionBoundaryCrossingHandler();
				array[20] = new BattleMissionAgentInteractionLogic();
				array[21] = new FieldBattleController();
				array[22] = new AgentMoraleInteractionLogic();
				array[23] = new HighlightsController();
				array[24] = new BattleHighlightsController();
				array[25] = new AssignPlayerRoleInTeamMissionController(!isPlayerSergeant, isPlayerSergeant, isPlayerInArmy, heroesOnPlayerSideByPriority, FormationClass.NumberOfRegularFormations);
				int num = 26;
				Hero leaderHero = MapEvent.PlayerMapEvent.AttackerSide.LeaderParty.LeaderHero;
				string attackerGeneralName = (leaderHero != null) ? leaderHero.Name.ToString() : null;
				Hero leaderHero2 = MapEvent.PlayerMapEvent.DefenderSide.LeaderParty.LeaderHero;
				array[num] = new CreateBodyguardMissionBehavior(attackerGeneralName, (leaderHero2 != null) ? leaderHero2.Name.ToString() : null, null, null, true);
				return array;
			}, true, true);
		}
    }
}
