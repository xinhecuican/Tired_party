﻿using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;
using Tired_party.Helper;

namespace Tired_party.sneak_attack
{
    class spawn_logic_patch
    {
        public static bool Prefix(object __instance, ref int __result, int number, bool isReinforcement, bool enforceSpawningOnInitialPoint = false )
        {
            Type t = typeof(MissionAgentSpawnLogic).Assembly.
                    GetType("TaleWorlds.MountAndBlade.MissionAgentSpawnLogic+MissionSide");
            //Type t = AccessTools.TypeByName("TaleWorlds.MountAndBlade.MissionAgentSpawnLogic.MissionSide");
            if (Party_tired.Current == null || !Party_tired.is_sneak_mission)
            {
                return true;
            }
            if (number <= 0)
            {
                __result = 0;
                return false;
            }

            try
            {
                
                int num = 0;
                //list: 要生成的军队信息
                List<IAgentOriginBase> list = ((IMissionTroopSupplier)AccessTools.Field(t, "_troopSupplier").GetValue(__instance)).SupplyTroops(number).ToList<IAgentOriginBase>();
                //list2: 对应阵营信息
                List<IAgentOriginBase> list2 = new List<IAgentOriginBase>();
                Mission.Current.ResetTotalWidth();
                for (int i = 0; i < 8; i++)
                {
                    list2.Clear();
                    IAgentOriginBase agentOriginBase = null;
                    FormationClass formationClass = (FormationClass)i;
                    foreach (IAgentOriginBase agentOriginBase2 in list)
                    {
                        if (formationClass == agentOriginBase2.Troop.GetFormationClass(agentOriginBase2.BattleCombatant)) //对应阵营
                        {
                            if (agentOriginBase2.Troop == Game.Current.PlayerTroop)
                            {
                                agentOriginBase = agentOriginBase2;
                            }
                            else
                            {
                                list2.Add(agentOriginBase2);
                            }
                        }
                    }
                    if (agentOriginBase != null)
                    {
                        list2.Add(agentOriginBase);
                    }
                    int count = list2.Count;
                    if (count > 0)
                    {
                        float num2 = (i == 2 || i == 7 || i == 6 || i == 3) ? 3f : 1f;
                        float num3 = (i == 2 || i == 7 || i == 6 || i == 3) ? 0.75f : 0.6f;
                        Mission.Current.SetTotalWidthBeforeNewFormation(num2 * (float)Math.Pow((double)count, (double)num3));
                        foreach (IAgentOriginBase troopOrigin in list2)
                        {
                            bool is_playerside = (bool)AccessTools.Property(t, "IsPlayerSide").GetValue(__instance);
                            Formation formation = Mission.GetAgentTeam(troopOrigin, is_playerside).GetFormation(formationClass);
                            bool spawn_with_horses = (bool)AccessTools.Field(t, "_spawnWithHorses").GetValue(__instance);
                            bool isMounted = spawn_with_horses &&
                                (formationClass == FormationClass.Cavalry || formationClass == FormationClass.LightCavalry || formationClass == FormationClass.HeavyCavalry || formationClass == FormationClass.HorseArcher);
                            if (formation != null && !(bool)AccessTools.Field(typeof(Formation), "HasBeenPositioned").GetValue(formation))
                            {
                                formation.BeginSpawn(count, isMounted);
                                Mission.Current.SpawnFormation(formation, count, spawn_with_horses, isMounted, isReinforcement);
                                ((MBList<Formation>)AccessTools.Field(t, "_spawnedFormations").GetValue(__instance)).Add(formation);
                            }
                            Mission.Current.SpawnTroop(troopOrigin, is_playerside, true, spawn_with_horses, isReinforcement, enforceSpawningOnInitialPoint, count, num, true, true, false, null, null);
                            num++;
                        }
                    }
                }
                if (num > 0)
                {
                    foreach (Team team in Mission.Current.Teams)
                    {
                        AccessTools.Method(typeof(TeamQuerySystem), "Expire", null, null).Invoke(team.QuerySystem, null);
                    }
                }
                foreach (Team team2 in Mission.Current.Teams)
                {
                    foreach (Formation formation2 in team2.Formations)
                    {
                        AccessTools.Field(typeof(Formation), "GroupSpawnIndex").SetValue(formation2, 0);
                    }
                }
                __result = num;
                return false;
            }
            catch(Exception e)
            {
                MethodInfo methodInfo = MethodBase.GetCurrentMethod() as MethodInfo;
                debug_helper.HandleException(e, methodInfo, "submodule load error");
                return false;
            }
        }
    }
}
