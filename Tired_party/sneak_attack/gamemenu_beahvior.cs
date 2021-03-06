﻿using Helpers;
using MCM.Abstractions.Settings.Base.Global;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.GameMenus;

namespace Tired_party.sneak_attack
{
    class gamemenu_beahvior : CampaignBehaviorBase
    {
        public override void RegisterEvents()
        {
            CampaignEvents.OnSessionLaunchedEvent.AddNonSerializedListener(this, new Action<CampaignGameStarter>(on_session_launch));
        }

        public override void SyncData(IDataStore dataStore)
        {
        }

        private void on_session_launch(CampaignGameStarter starter)
        {
			starter.AddGameMenuOption("encounter", "sneak action", "{=ZIzKxQstKx}sneak attack", new TaleWorlds.CampaignSystem.GameMenus.GameMenuOption.OnConditionDelegate(sneak_condition)
				, new GameMenuOption.OnConsequenceDelegate(sneak_consequence));
        }

		private void sneak_consequence(MenuCallbackArgs args)
        {
			if (PlayerEncounter.Battle == null)
			{
				PlayerEncounter.StartBattle();
				PlayerEncounter.Update();
			}
			sneak_mission.open_sneak_mission(PlayerEncounter.GetBattleSceneForMapPosition(MobileParty.MainParty.Position2D));
		}

        private bool sneak_condition(MenuCallbackArgs args)
        {
			if(GlobalSettings<mod_setting>.Instance.use_sneak)
            {
				return true;
            }
			if(CampaignTime.Now.IsDayTime)
            {
				return false;
            }
			CheckEnemyAttackableHonorably(args);
            bool flag1 = MenuHelper.EncounterAttackCondition(args);
			if(!flag1)
            {
				return false;
            }
			MapEvent battle = PlayerEncounter.Battle;
			if(battle.MapEventSettlement != null || battle.IsAlleyFight || battle.IsSiegeAssault || battle.IsRaid)
            {
				return false;
            }

			if(MapEvent.PlayerMapEvent.GetLeaderParty(PartyBase.MainParty.OpponentSide).IsMobile && MapEvent.PlayerMapEvent.GetLeaderParty(PartyBase.MainParty.Side).MobileParty == Campaign.Current.MainParty
				&& PartyBase.MainParty.Side == TaleWorlds.Core.BattleSideEnum.Attacker)
            {
				MobileParty party = MapEvent.PlayerMapEvent.GetLeaderParty(PartyBase.MainParty.OpponentSide).MobileParty;
				if (Party_tired.Current.Party_tired_rate.ContainsKey(party) && Party_tired.Current.Party_tired_rate[party].reset_time > 0)
                {
					return true;
                }
            }
			return false;
		}

		private void CheckEnemyAttackableHonorably(MenuCallbackArgs args)
		{
			if (MobileParty.MainParty.Army != null && MobileParty.MainParty.Army.LeaderParty != MobileParty.MainParty)
			{
				return;
			}
			if (PlayerEncounter.PlayerIsDefender)
			{
				return;
			}
			IFaction mapFaction;
			if (PlayerEncounter.EncounteredMobileParty != null)
			{
				mapFaction = PlayerEncounter.EncounteredMobileParty.MapFaction;
			}
			else
			{
				if (PlayerEncounter.EncounteredParty == null)
				{
					return;
				}
				mapFaction = PlayerEncounter.EncounteredParty.MapFaction;
			}
			if (mapFaction != null && mapFaction.NotAttackableByPlayerUntilTime.IsFuture)
			{
				args.IsEnabled = false;
			}
		}
	}
}
