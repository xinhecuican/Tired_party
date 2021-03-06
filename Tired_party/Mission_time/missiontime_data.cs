﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;
using Tired_party.Helper;
using Tired_party.sneak_attack;

namespace Tired_party.Mission_time
{
    class missiontime_data : MissionLogic
    {
        public List<arrive_time_data>[] time_and_direction;
        public List<MapEventParty>[] parties;
        public static missiontime_data current;
        public static float pass_time;
        public int[] initial_num;
        public int[] initial_troop_num;
        public missiontime_data()
        {
            time_and_direction = new List<arrive_time_data>[2];
            parties = new List<MapEventParty>[2];
            for(int i=0; i<2; i++)
            {
                time_and_direction[i] = new List<arrive_time_data>();
                parties[i] = new List<MapEventParty>();
            }
            current = this;
            initial_num = new int[2];
            initial_troop_num = new int[2];
            pass_time = 0f;
            on_init();
            spawn_logic_patch.enemy_origin_position = Vec2.Invalid;
            spawn_logic_patch.radius = 0;
            spawn_logic_patch.center_point = Vec2.Invalid;
            spawn_logic_patch.party_origin_position = Vec2.Invalid;
            isUnderPlayerCommendGetter_patch._controller = null;
        }

        private void on_init()
        {
            try
            {
                for (int i = 0; i < 2; i++)
                {
                    MapEventSide side = MapEvent.PlayerMapEvent.GetMapEventSide((TaleWorlds.Core.BattleSideEnum)i);
                    Vec2 position = MapEvent.PlayerMapEvent.Position;
                    foreach (MapEventParty party in side.Parties)
                    {
                        float distance;
                        float time = 0;
                        Campaign.Current.Models.MapDistanceModel.GetDistance(party.Party.MobileParty, Campaign.Current.MainParty.Position2D, float.MaxValue, out distance);
                        time = 2 * distance / party.Party.MobileParty.SpeedExplanation.ResultNumber;
                        if (time < 0.5f || (party.Party.IsMobile ? party.Party.MobileParty.IsMainParty : false))
                        {
                            /*party.Update();
                            int count = 0;
                            using (IEnumerator<FlattenedTroopRosterElement> enumerator = party.Troops.GetEnumerator())
                            {
                                while (enumerator.MoveNext())
                                {
                                    FlattenedTroopRosterElement rosterElement = enumerator.Current;
                                    if (!rosterElement.IsWounded && !rosterElement.IsRouted && !rosterElement.IsKilled)
                                    {
                                        count++;
                                    }
                                }
                            }*/
                            initial_num[i] += party.Party.NumberOfHealthyMembers;
                            parties[i].Add(party);
                            initial_troop_num[i]++;
                        }
                        else
                        {
                            Vec2 direction = position - party.Party.Position2D;
                            time_and_direction[i].Add(new arrive_time_data(direction, time, party, i));
                        }
                    }
                    if(initial_troop_num[i] == 0)
                    {
                        initial_num[i] += time_and_direction[i][0].number;
                        initial_troop_num[i]++;
                        parties[i].Add(time_and_direction[i][0].party);
                        time_and_direction[i].RemoveAt(0);
                    }
                    time_and_direction[i].Sort((x, y) => x.arrive_time.CompareTo(y.arrive_time));
                    foreach (arrive_time_data data in time_and_direction[i])
                    {
                        parties[i].Add(data.party);
                    }
                }
            }
            catch(Exception e)
            {
                MethodInfo methodInfo = MethodBase.GetCurrentMethod() as MethodInfo;
                debug_helper.HandleException(e, methodInfo, "submodule load error");
            }
        }

    }
}
