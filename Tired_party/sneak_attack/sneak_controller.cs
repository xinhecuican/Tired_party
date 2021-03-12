using Messages.FromLobbyServer.ToClient;
using Mono.Cecil;
using SandBox;
using SandBox.Source.Missions;
using SandBox.Source.Objects.SettlementObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.Diamond;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;
using Tired_party.Helper;

namespace Tired_party.sneak_attack
{
    public class sneak_controller : MissionLogic, IMissionBehavior
    {
        private BattleEndLogic _battleEndLogic;
        private BattleAgentLogic _battleAgentLogic;
        private MissionAgentSpawnLogic _Spawn_Logic;
        private bool _isMissionInitialized = false;
        private bool _troopsInitialized;
        private BattleSideEnum player_side;
        public static bool is_drop_items;
        public static List<Agent> agents;
        public static int drop_num;


        public sneak_controller(BattleSideEnum playerSide)
        {
            player_side = playerSide;
        }
        public override void OnBehaviourInitialize()
        {
            agents = new List<Agent>();
            base.OnBehaviourInitialize();
            this._battleAgentLogic = base.Mission.GetMissionBehaviour<BattleAgentLogic>();
            this._battleEndLogic = base.Mission.GetMissionBehaviour<BattleEndLogic>();
            _Spawn_Logic = base.Mission.GetMissionBehaviour<MissionAgentSpawnLogic>();
            spawn_logic_patch.box_max = new Vec2(0, 0);
            spawn_logic_patch.box_min = new Vec2(0, 0);
        }

        public override void AfterStart()
        {
            int num = MBMath.Floor((float)MapEvent.PlayerMapEvent.GetNumberOfInvolvedMen(BattleSideEnum.Defender));
            int num2 = MBMath.Floor((float)MapEvent.PlayerMapEvent.GetNumberOfInvolvedMen(BattleSideEnum.Attacker));
            int defenderInitialSpawn = num;
            int attackerInitialSpawn = num2;
            this._Spawn_Logic.SetSpawnHorses(BattleSideEnum.Defender, !MapEvent.PlayerMapEvent.IsSiegeAssault);
            this._Spawn_Logic.SetSpawnHorses(BattleSideEnum.Attacker, !MapEvent.PlayerMapEvent.IsSiegeAssault);
            this._Spawn_Logic.InitWithSinglePhase(num, num2, defenderInitialSpawn, attackerInitialSpawn, true, true, 1f);
        }

        public override void OnObjectStoppedBeingUsed(Agent userAgent, UsableMissionObject usedObject)
        {
        }

        public override void OnAgentAlarmedStateChanged(Agent agent, Agent.AIStateFlag flag)
        {
            /*if(flag == Agent.AIStateFlag.Alarmed || flag == Agent.AIStateFlag.Cautious)
            {
                if(agent.IsUsingGameObject)
                {
                    agent.StopUsingGameObject(true, true);
                }
                else
                {
                    //agent.DisableScriptedMovement();
                    //agent.AIMoveToGameObjectDisable();
                }
            }
            else if (flag == Agent.AIStateFlag.None)
            {
                agent.TryToSheathWeaponInHand(Agent.HandIndex.MainHand,
                    Agent.WeaponWieldActionType.WithAnimation);
            }*/
            if(flag == Agent.AIStateFlag.Alarmed || flag == Agent.AIStateFlag.Cautious)
            {
                action_component component = agent.GetComponent<action_component>();
                if (component != null)
                {    
                    if(flag == Agent.AIStateFlag.Alarmed)
                    {
                        component.prepare_asleep = true;
                    }
                }
                
            }
            if(flag == Agent.AIStateFlag.Alarmed)
            {
                patrol_component component1 = agent.GetComponent<patrol_component>();
                if (component1 != null)
                {
                    agent.SetWantsToYell();
                    agent.DisableScriptedMovement();
                }
                else
                {
                    if(MBRandom.RandomFloat < 0.1)
                    {
                        DropWeapons(agent);
                    }
                }
                
            }
            
        }

        public override void OnMissionTick(float dt)
        {
            try
            {
                if (!_isMissionInitialized)
                {
                    InitializeMission();
                    _isMissionInitialized = true;
                    return;
                
                }
            }
            catch(Exception e)
            {
                MethodInfo methodInfo = MethodBase.GetCurrentMethod() as MethodInfo;
                debug_helper.HandleException(e, methodInfo, "submodule load error");
            }
        }


        protected override void OnEndMission()
        {
            Party_tired.is_sneak_mission = false;
            spawn_logic_patch.box_min = Vec2.Invalid;
            spawn_logic_patch.box_max = Vec2.Invalid;
        }

        private void InitializeMission()
        {
            try
            {
                base.Mission.SetMissionMode(MissionMode.Stealth, true);
                base.Mission.DoesMissionRequireCivilianEquipment = false;
            }
            catch(Exception e)
            {
                MethodInfo methodInfo = MethodBase.GetCurrentMethod() as MethodInfo;
                debug_helper.HandleException(e, methodInfo, "submodule load error");
            }
        }

        public static void DropWeapons(Agent agent)
        {
            for (EquipmentIndex equipmentIndex = EquipmentIndex.WeaponItemBeginSlot; equipmentIndex < EquipmentIndex.NumAllWeaponSlots; equipmentIndex++)
            {
                if (!agent.Equipment[equipmentIndex].IsEmpty)
                {
                    
                    WeaponClass weapon = agent.Equipment[equipmentIndex].Item.PrimaryWeapon.WeaponClass;
                    agent.DropItem(equipmentIndex, weapon);
                }
            }
        }
    }
}
