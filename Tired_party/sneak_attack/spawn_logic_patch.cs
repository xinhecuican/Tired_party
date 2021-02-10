using HarmonyLib;
using SandBox;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;
using Tired_party.Helper;

namespace Tired_party.sneak_attack
{
    class spawn_logic_patch
    {
        private static string[] used_tent = new string[] {"empire_street_tent_04", "empire_street_tent_05", "empire_street_tent_06" };
        private static string[] environment_things = new string[] { "burning_campfire", "bd_barrel_a", "bd_barrel_b", "bd_barrel_c", "bd_barrel_d", "bd_barrel_e"
        ,"bd_cart_heap_a", "bd_cart_heap_b", "bd_sack_heap_a", "bd_sack_heap_b"};
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
                        int formation_num = 0;
                        List<Vec2> tent_location = new List<Vec2>();
                        Vec2 x_vector = Vec2.Invalid;
                        Vec2 y_vector = Vec2.Invalid;
                        foreach (IAgentOriginBase troopOrigin in list2)
                        {
                            bool is_playerside = (bool)AccessTools.Property(t, "IsPlayerSide").GetValue(__instance);
                            Formation formation = Mission.GetAgentTeam(troopOrigin, is_playerside).GetFormation(formationClass);
                            bool spawn_with_horses = (bool)AccessTools.Field(t, "_spawnWithHorses").GetValue(__instance);
                            bool isMounted = spawn_with_horses &&
                                (formationClass == FormationClass.Cavalry || formationClass == FormationClass.LightCavalry || formationClass == FormationClass.HeavyCavalry || formationClass == FormationClass.HorseArcher);
                            //bool is_initial_spawn_over = (bool)AccessTools.Property(typeof(MissionAgentSpawnLogic), "IsInitialSpawnOver").GetValue(__instance);
                            if(!is_playerside)
                            {
                                //spawn_with_horses = false;
                                isMounted = false;
                            }
                            bool is_initial_spawn_over = Mission.Current.GetMissionBehaviour<MissionAgentSpawnLogic>().IsInitialSpawnOver;
                            if (formation != null && !(bool)AccessTools.Field(typeof(Formation), "HasBeenPositioned").GetValue(formation))
                            {
                                formation.BeginSpawn(count, isMounted);
                                Mission.Current.SpawnFormation(formation, count, spawn_with_horses, isMounted, isReinforcement);
                                ((MBList<Formation>)AccessTools.Field(t, "_spawnedFormations").GetValue(__instance)).Add(formation);
                                //改变出生点
                                if (!is_playerside)
                                {
                                    
                                    
                                    x_vector = new Vec2(-formation.Direction.y, formation.Direction.x);
                                    y_vector = formation.Direction;
                                    x_vector.Normalize();
                                    y_vector.Normalize();
                                    WorldFrame formationSpawnFrame = Mission.Current.GetFormationSpawnFrame(formation.Team.Side, formation.FormationIndex, 
                                        isReinforcement, -1, 0f, spawn_with_horses);
                                    WorldPosition origin = formationSpawnFrame.Origin;
                                    WorldPosition order_position = formation.OrderPosition;
                                    float add_x = order_position.X - origin.X < 0 ? -10 : 10;
                                    float add_y = order_position.Y - origin.Y < 0 ? -30 : 30;
                                    add_y -= 30;
                                    Vec2 add_vec = x_vector * add_x + y_vector * add_y;
                                    origin.SetVec2(origin.AsVec2 + add_vec);
                                    Mat3 identity = Mat3.Identity;
                                    identity.RotateAboutUp(formation.Direction.RotationInRadians);
                                    WorldFrame value = new WorldFrame(identity, origin);
                                    formation.SetPositioning(new WorldPosition?(origin), null, null);
                                    formation.SetSpawnFrame(new WorldFrame?(value));
                                    WorldFrame worldFrame = new WorldFrame(identity, origin);
                                    MatrixFrame matrix_frame = worldFrame.ToGroundMatrixFrame();
                                    matrix_frame.rotation.Orthonormalize();
                                    matrix_frame.rotation.Orthonormalize();
                                    Mission.Current.CreateMissionObjectFromPrefab(environment_things[0], matrix_frame);
                                    //添加帐篷
                                    int k = 1;
                                    while(k <= list2.Count / 5 + 1)
                                    //for(int k=1; k<=list2.Count / 5 + 1; k++)
                                    {
                                        Vec2 add_vec2 = x_vector * (k % 2 > 0 ? 1 : -1) * k * 10 + y_vector * (MBRandom.RandomFloat * (MBRandom.RandomFloat > 0.5 ? 1 : -1) - (int)(k / 10) * 5);
                                        origin.SetVec2(origin.AsVec2 + add_vec2);
                                        
                                        if (Mission.Current.IsFormationUnitPositionAvailable(ref origin, formation.Team))
                                        {
                                            tent_location.Add(origin.AsVec2);
                                            WorldFrame value2 = new WorldFrame(identity, origin);
                                            MatrixFrame frame = value2.ToGroundMatrixFrame();
                                            frame.rotation.Orthonormalize();
                                            //frame.rotation.OrthonormalizeAccordingToForwardAndKeepUpAsZAxis();
                                            Mission.Current.CreateMissionObjectFromPrefab(used_tent[MBRandom.RandomInt() % 3], frame);
                                            if(MBRandom.RandomFloat > 0.6)
                                            {
                                                float random_x = 2 * MBRandom.RandomFloat;
                                                float random_y = 4 + MBRandom.RandomFloat;
                                                Vec2 vec2 = random_x * x_vector + random_y * y_vector;
                                                origin.SetVec2(origin.AsVec2 + vec2);
                                                WorldFrame frame1 = new WorldFrame(identity, origin);
                                                frame = frame1.ToGroundMatrixFrame();
                                                frame.rotation.Orthonormalize();
                                                Mission.Current.CreateMissionObjectFromPrefab(environment_things[MBRandom.RandomInt() % environment_things.Length], frame);
                                            }
                                            
                                        }
                                        k++;
                                    }
                                    
                                }
                                /*if(Mission.Current.IsPositionInsideBoundaries(origin.AsVec2))
                                {
                                    ((IFormationArrangement)AccessTools.Field(typeof(Formation), "_arrangement").GetValue(formation)).BeforeFormationFrameChange();
                                    AccessTools.Field(typeof(Formation), "_orderPosition").SetValue(formation, origin);
                                    AccessTools.Method(typeof(ArrangementOrder), "OnOrderPositionChanged", 
                                        new Type[] { typeof(Formation), typeof(Vec3) }, null).Invoke(formation.ArrangementOrder, new object[] { formation, order_position.Position });
                                }*/
                                //AccessTools.Property(typeof(Formation), "Width").SetValue(formation, formation.Width * 2);
                                formation.FiringOrder = FiringOrder.FiringOrderHoldYourFire;
                                if(isMounted)
                                {
                                    if ((int)AccessTools.Field(typeof(MovementOrder), "OrderEnum").GetValue(formation.MovementOrder) !=  0xA)
                                    {
                                        WorldPosition orderPosition = formation.OrderPosition;
                                        formation.MovementOrder = MovementOrder.MovementOrderStop;
                                    }
                                    formation.RidingOrder = RidingOrder.RidingOrderDismount;
                                }
                            }
                            if (!is_initial_spawn_over)
                            {
                                
                                if (is_playerside)
                                {
                                    Mission.Current.SpawnTroop(troopOrigin, is_playerside, true, spawn_with_horses, isReinforcement, enforceSpawningOnInitialPoint, count, num, true, true, false, null, null);
                                }
                                else
                                {
                                  
                                    Team agentTeam = Mission.GetAgentTeam(troopOrigin, is_playerside);
                                    MatrixFrame frame = Mission.Current.GetFormationSpawnFrame(agentTeam.Side, FormationClass.NumberOfRegularFormations, false, -1, 0f, true).ToGroundMatrixFrame();
                                    Vec2 tent_vec = tent_location[formation_num / 5];
                                    tent_vec += x_vector * (formation_num % 2 > 0 ? 1 : -1) * ((formation_num % 5) / 2 + 0.7f) + y_vector;
                                    frame.origin.x = tent_vec.x;
                                    frame.origin.y = tent_vec.y;
                                    frame.origin.z = frame.origin.ToWorldPosition().GetGroundZ();
                                    MatrixFrame matrixFrame = new MatrixFrame(Mat3.Identity, frame.origin);
                                    matrixFrame.rotation.Orthonormalize();
                                    //matrixFrame.rotation.OrthonormalizeAccordingToForwardAndKeepUpAsZAxis();
                                    if(formation_num % 10 != 0)
                                    {
                                        spawn_with_horses = false;
                                    }
                                    Agent agent = Mission.Current.SpawnTroop(troopOrigin, false, true, spawn_with_horses, isReinforcement, false, count, num, false, false, false, "as_human_hideout_bandit", new MatrixFrame?(matrixFrame));
                                    
                                    AccessTools.Property(typeof(Agent), "InitialFrame").SetValue(agent, matrixFrame);
                                    agent.WieldInitialWeapons(Agent.WeaponWieldActionType.InstantAfterPickUp);
                                    AgentFlag agentFlags = agent.GetAgentFlags();
                                    agent.SetAgentFlags((agentFlags | AgentFlag.CanGetAlarmed));

                                    if (formation_num % 10 != 0)
                                    {
                                        ActionIndexCache first_action = ActionIndexCache.Create("act_dungeon_prisoner_sleep");
                                        agent.SetActionChannel(0, first_action, false, 0UL, 0f, 1f, -0.2f, 10000f, 0, false, -0.2f, 1, true);
                                        agent.AddComponent(new action_component(agent));
                                        //agent.AddComponent(new ForcePickComponent(agent));
                                    }
                                    else
                                    {
                                        agent.AddComponent(new patrol_component(agent));
                                    }
                                    agent.GetComponent<CampaignAgentComponent>().CreateAgentNavigator();
                                    
                                    sneak_controller.agents.Add(agent);
                                    sneak_controller.drop_num++;
                                    
                                }
                            }
                            else
                            {
                                Mission.Current.SpawnTroop(troopOrigin, is_playerside, true, spawn_with_horses, isReinforcement, enforceSpawningOnInitialPoint, count, num, true, true, false, null, null);
                            }
                            formation_num++;
                            num++;
                        }
                    }
                }
                sneak_controller.is_drop_items = true;

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
