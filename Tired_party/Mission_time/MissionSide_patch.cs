﻿using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Instrumentation;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace Tired_party.Mission_time
{
    [HarmonyPatch(typeof(MapEventSide))]
    class MissionSide_patch
    {
        [HarmonyPrefix]
        [HarmonyPatch("MakeReadyParty")]
        public static bool Prefix(MapEventSide __instance, MapEventParty battleParty, FlattenedTroopRoster priorityTroops, bool includePlayers, int sizeOfSide)
        {
			if(!Party_tired.is_wish_mission || __instance.MapEvent != MapEvent.PlayerMapEvent)
            {
				return true;
            }
			battleParty.Update();
			int numberOfHealthyMembers = battleParty.Party.NumberOfHealthyMembers;
			foreach (FlattenedTroopRosterElement rosterElement in battleParty.Troops)
			{
				int side = (int)__instance.MissionSide;
				int index = missiontime_data.current.parties[side].IndexOf(battleParty);
				int add_num = 0;
				if(index >= missiontime_data.current.initial_troop_num[side])
                {
					add_num += 100000 * (missiontime_data.current.parties[side].Count - index - 1);
                }
				else
                {
					add_num += 100000 * (missiontime_data.current.parties[side].Count - missiontime_data.current.initial_troop_num[side]);
                }
				if (!rosterElement.IsWounded && !rosterElement.IsRouted && !rosterElement.IsKilled)
				{
					int num = 1;
					if (rosterElement.Troop.IsHero && (includePlayers || !rosterElement.Troop.HeroObject.IsHumanPlayerCharacter))
					{
						num *= 150;
						if (priorityTroops != null)
						{
							UniqueTroopDescriptor descriptor = priorityTroops.FindIndexOfCharacter(rosterElement.Troop);
							if (descriptor.IsValid)
							{
								num *= 100;
								priorityTroops.Remove(descriptor);
							}
						}
						else if (rosterElement.Troop.HeroObject.IsHumanPlayerCharacter)
						{
							num *= 10;
						}
						num += add_num;
						((PriorityQueue<int, KeyValuePair<UniqueTroopDescriptor, MapEventParty>>)AccessTools.Field(typeof(MapEventSide), "_readyTroopsPriorityQueue").GetValue(__instance))
							.Enqueue(num, new KeyValuePair<UniqueTroopDescriptor, MapEventParty>(rosterElement.Descriptor, battleParty));
					}
					else if (!rosterElement.Troop.IsHero)
					{
						int num2 = 0;
						int num3 = 0;
						for (int i = 0; i < battleParty.Party.MemberRoster.Count; i++)
						{
							TroopRosterElement elementCopyAtIndex = battleParty.Party.MemberRoster.GetElementCopyAtIndex(i);
							if (!elementCopyAtIndex.Character.IsHero)
							{
								if (elementCopyAtIndex.Character == rosterElement.Troop)
								{
									num2 = i - num3;
									break;
								}
							}
							else
							{
								num3++;
							}
						}
						int num4 = (int)(100.0 / Math.Pow(1.2000000476837158, (double)num2));
						if (num4 < 10)
						{
							num4 = 10;
						}
						int num5 = numberOfHealthyMembers / sizeOfSide * 100;
						if (num5 < 10)
						{
							num5 = 10;
						}
						int num6 = 0;
						if (priorityTroops != null)
						{
							UniqueTroopDescriptor descriptor2 = priorityTroops.FindIndexOfCharacter(rosterElement.Troop);
							if (descriptor2.IsValid)
							{
								num6 = 20000;
								priorityTroops.Remove(descriptor2);
							}
						}
						num = num6 + MBRandom.RandomInt((int)((float)num4 * 0.5f + (float)num5 * 0.5f));
						num += add_num;
						((PriorityQueue<int, KeyValuePair<UniqueTroopDescriptor, MapEventParty>>)AccessTools.Field(typeof(MapEventSide), "_readyTroopsPriorityQueue").GetValue(__instance))
							.Enqueue(num, new KeyValuePair<UniqueTroopDescriptor, MapEventParty>(rosterElement.Descriptor, battleParty));
					}
				}
			}
			return false;
		}
    }
}
