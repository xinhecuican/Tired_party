﻿using HarmonyLib;
using MCM.Abstractions.Settings.Base.Global;
using SandBox;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;

namespace Tired_party.Mission_time
{
    [HarmonyPatch(typeof(SandBoxMissions))]
    class using_missiontime_missions
    {
        [HarmonyPrefix]
        [HarmonyPatch("OpenBattleMission", new Type[] { typeof(MissionInitializerRecord) })]
        public static void battle_prefix()
        {
            if(GlobalSettings<mod_setting>.Instance.ban_reinforcement)
            {
                return;
            }
            Party_tired.is_wish_mission = true;
            new missiontime_data();
        }
        [HarmonyPostfix]
        [HarmonyPatch("OpenBattleMission", new Type[] { typeof(MissionInitializerRecord) })]
        public static void battle_postfix(Mission __result)
        {
            if (GlobalSettings<mod_setting>.Instance.ban_reinforcement)
            {
                return;
            }
            __result.AddMissionBehaviour(new end_behaivor());
            BaseMissionTroopSpawnHandler baseMissionTroopSpawnHandler = __result.GetMissionBehaviour<BaseMissionTroopSpawnHandler>();
            if (baseMissionTroopSpawnHandler != null)
            {
                __result.RemoveMissionBehaviour(baseMissionTroopSpawnHandler);
                __result.AddMissionBehaviour(new agent_spawn_controller());
            }
        }

        [HarmonyPrefix]
        [HarmonyPatch("OpenCaravanBattleMission")]
        public static void caravan_battle_prefix()
        {
            if (GlobalSettings<mod_setting>.Instance.ban_reinforcement)
            {
                return;
            }
            Party_tired.is_wish_mission = true;
            new missiontime_data();
        }

        [HarmonyPostfix]
        [HarmonyPatch("OpenCaravanBattleMission")]
        public static void caravan_battle_postfix(Mission __result)
        {
            if (GlobalSettings<mod_setting>.Instance.ban_reinforcement)
            {
                return;
            }
            __result.AddMissionBehaviour(new end_behaivor());
            BaseMissionTroopSpawnHandler baseMissionTroopSpawnHandler = __result.GetMissionBehaviour<BaseMissionTroopSpawnHandler>();
            if (baseMissionTroopSpawnHandler != null)
            {
                __result.RemoveMissionBehaviour(baseMissionTroopSpawnHandler);
                __result.AddMissionBehaviour(new agent_spawn_controller());
            }
        }


    }
}
