﻿using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;

namespace Tired_party.Mission_time
{
    [HarmonyPatch(typeof(MissionAgentSpawnLogic))]
    class Reinforce_sum_patch
    {
        [HarmonyPrefix]
        [HarmonyPatch("BattleSizeSpawnTick")]
        public static bool battle_size_prefix(MissionAgentSpawnLogic __instance)
        {
            if(!Party_tired.is_wish_mission)
            {
                return true;
            }
            if(__instance.NumberOfRemainingTroops > 0 && __instance.NumberOfTroopsCanBeSpawned > 0)
            {
                agent_spawn_controller controller = Mission.Current.GetMissionBehaviour<agent_spawn_controller>();
                if (controller != null && controller.ready_to_place.Count != 0)
                {
                    arrive_time_data data = controller.ready_to_place[0];
                    Type t = typeof(MissionAgentSpawnLogic).Assembly.
                        GetType("TaleWorlds.MountAndBlade.MissionAgentSpawnLogic+SpawnPhase");
                    object o = AccessTools.Property(typeof(MissionAgentSpawnLogic), "DefenderActivePhase").GetValue(__instance);
                    object o2 = AccessTools.Property(typeof(MissionAgentSpawnLogic), "AttackerActivePhase").GetValue(__instance);
                    int defender_num = (int)AccessTools.Field(t, "TotalSpawnNumber").GetValue(o);
                    int attacker_num = (int)AccessTools.Field(t, "TotalSpawnNumber").GetValue(o2);
                    int num = defender_num + attacker_num;
                    int num2 = MBMath.Round((float)__instance.NumberOfTroopsCanBeSpawned * (float)defender_num / (float)num);
                    int num3 = __instance.NumberOfTroopsCanBeSpawned - num2;
                    if (!data.is_initialize_over)
                    {
                        if(data.side == 0 && data.number - (float)(data.initial_num) / 2 > num2)
                        {
                            return false;
                        }
                        else if(data.side == 1 && data.number - (float)(data.initial_num) / 2 > num3)
                        {
                            return false;
                        }
                    }
                }
            }
            return true;
        }
    }
}
