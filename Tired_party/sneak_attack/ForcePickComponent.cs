﻿using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Net;
using System.Net.Configuration;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;

namespace Tired_party.sneak_attack
{
    class ForcePickComponent : AgentComponent
    {
        private UseObjectAgentComponent _useObjectAgentComponent;
        private Agent _agent;
        public List<WeaponClass> WeaponClass;
        public bool[] has_equip = new bool[16];
        public bool need_find;
        public SpawnedItemEntity _itemToPickUp;
        public int pick_num;
        private GameEntity[] _tempPickableEntities = new GameEntity[16];
        private UIntPtr[] _pickableItemsId = new UIntPtr[16];
        public WeaponClass second_need = TaleWorlds.Core.WeaponClass.Undefined;
        public bool can_escape = false;
        public Vec2 origin_position;
        public MissionTimer begin_time;
        public MissionTimer end_time;
        public Agent agent
        {
            get { return _agent; }
        }
        public ForcePickComponent(Agent agent) : base(agent)
        {
            _agent = agent;
            WeaponClass = new List<WeaponClass>();
            need_find = false;
            _useObjectAgentComponent = agent.GetComponent<UseObjectAgentComponent>();
            WeaponClass = new List<WeaponClass>();
        }

        public void add_weapon(WeaponClass weapon)
        {
            if (weapon != TaleWorlds.Core.WeaponClass.Undefined)
            {
                WeaponClass.Add(weapon);
                has_equip[pick_num] = false;
                pick_num++;

            }
        }

        protected override void OnTickAsAI(float dt)
        {
            if(agent.Mission.MissionEnded() || can_escape || !need_find)
            {
                return;
            }
            find_object();
            /*

            if(_itemToPickUp != null && !this._itemToPickUp.MovingAgents.ContainsKey(agent))
            {
                _itemToPickUp = null;
            }
            if (this._itemToPickUp != null && this._itemToPickUp.GameEntity == null)
            {
                this.agent.StopUsingGameObject(false, true);
            }
            if (begin_time.Check(true))
            {
                
                if (agent.IsAIControlled && !agent.IsRunningAway && agent.CanBeAssignedForScriptedMovement() && (this.agent.GetAgentFlags() & AgentFlag.CanAttack) != AgentFlag.None
                        && !this.IsInImportantCombatAction() && _itemToPickUp == null)
                {
                    float maximumForwardUnlimitedSpeed = this.agent.MaximumForwardUnlimitedSpeed ;
                    Vec3 bMin = this.agent.Position - new Vec3(maximumForwardUnlimitedSpeed, maximumForwardUnlimitedSpeed, 1f, -1f);
                    Vec3 bMax = this.agent.Position + new Vec3(maximumForwardUnlimitedSpeed, maximumForwardUnlimitedSpeed, 1.8f, -1f);
                    WeaponClass weaponClass = get_faviourate_weapon();
                    _itemToPickUp = SelectPickableItem(bMin, bMax, weaponClass);
                    if(_itemToPickUp != null)
                    {
                        InformationManager.DisplayMessage(new InformationMessage(_itemToPickUp.WeaponName));
                        RequestMoveToItem(_itemToPickUp);
                    }
                    else
                    {
                        InformationManager.DisplayMessage(new InformationMessage("escape"));
                        escape();
                        //find_object();
                    }
                    if(end_time.Check())
                    {
                        escape();
                    }
                }
            }
            if(_itemToPickUp != null && (this.agent.AIStateFlags & Agent.AIStateFlag.UseObjectMoving) != Agent.AIStateFlag.None)
            {
                float distanceSq = this.agent.Frame.origin.DistanceSquared(this._itemToPickUp.GetUserFrameForAgent(this.agent).Origin.Position);
                if (this.agent.CanReachAndUseObject(this._itemToPickUp, distanceSq))
                {
                    this.agent.UseGameObject(this._itemToPickUp, -1);
                }
            }*/
            
        }

        private bool IsInImportantCombatAction()
        {
            Agent.ActionCodeType currentActionType = this.agent.GetCurrentActionType(1);
            return currentActionType == Agent.ActionCodeType.ReadyMelee || currentActionType == Agent.ActionCodeType.ReadyRanged || currentActionType == Agent.ActionCodeType.ReleaseMelee || currentActionType == Agent.ActionCodeType.ReleaseRanged || currentActionType == Agent.ActionCodeType.ReleaseThrowing || currentActionType == Agent.ActionCodeType.DefendShield;
        }

        private WeaponClass get_faviourate_weapon()
        {
            bool need_arrow = false;
            bool need_shield = false;
            WeaponClass ans = TaleWorlds.Core.WeaponClass.Undefined;
            for(int i=0; i<WeaponClass.Count; i++)
            {
                if(!has_equip[i])
                {
                    second_need = WeaponClass[i];
                }
                if(WeaponClass[i] == TaleWorlds.Core.WeaponClass.Bow || WeaponClass[i] == TaleWorlds.Core.WeaponClass.Crossbow
                    || WeaponClass[i] == TaleWorlds.Core.WeaponClass.Pistol || WeaponClass[i] == TaleWorlds.Core.WeaponClass.Musket)
                {
                    if(has_equip[i])
                    {
                        need_arrow = true;
                    }
                    else
                    {
                        second_need = WeaponClass[i] - 3;
                        return WeaponClass[i];
                    }
                }
                if(need_arrow && (WeaponClass[i] == TaleWorlds.Core.WeaponClass.Arrow || WeaponClass[i] == TaleWorlds.Core.WeaponClass.Bolt
                    || WeaponClass[i] == TaleWorlds.Core.WeaponClass.Cartridge))
                {
                    ans = WeaponClass[i];
                }
                if(WeaponClass[i] <= TaleWorlds.Core.WeaponClass.LowGripPolearm)
                {
                    if(has_equip[i])
                    {
                        need_shield = true;
                    }
                    else
                    {
                        ans = WeaponClass[i];
                    }
                }
                if(need_shield && (WeaponClass[i] == TaleWorlds.Core.WeaponClass.LargeShield || WeaponClass[i] == TaleWorlds.Core.WeaponClass.SmallShield))
                {
                    ans = WeaponClass[i];
                }    
            }
            return ans;
        }

        private void RequestMoveToItem(SpawnedItemEntity item)
        {
            if (item.MovingAgents.Any<KeyValuePair<Agent, UsableMissionObject.MoveInfo>>())
            {
                item.MovingAgents.First((KeyValuePair<Agent, UsableMissionObject.MoveInfo> ma) => ma.Key != null).Key.StopUsingGameObject(false, true);
            }
            this._useObjectAgentComponent.MoveToUsableGameObject(item, Agent.AIScriptedFrameFlags.NoAttack);
        }

        public SpawnedItemEntity SelectPickableItem(Vec3 bMin, Vec3 bMax, WeaponClass best_eqiupment)
        {
            int num = this.agent.Mission.Scene.SelectEntitiesInBoxWithScriptComponent<SpawnedItemEntity>(ref bMin, ref bMax, this._tempPickableEntities, this._pickableItemsId);
            float num2 = 0f;
            SpawnedItemEntity result = null;
            for (int i = 0; i < num; i++)
            {
                SpawnedItemEntity firstScriptOfType = this._tempPickableEntities[i].GetFirstScriptOfType<SpawnedItemEntity>();
                if (firstScriptOfType != null && !firstScriptOfType.WeaponCopy.IsEmpty && !firstScriptOfType.HasUser &&
                    (firstScriptOfType.MovingAgents.Count == 0 || firstScriptOfType.MovingAgents.ContainsKey(agent)) &&
                    firstScriptOfType.GameEntityWithWorldPosition.WorldPosition.GetNavMesh() != UIntPtr.Zero)
                {
                    EquipmentIndex equipmentIndex = MissionEquipment.SelectWeaponPickUpSlot(this.Agent, firstScriptOfType.WeaponCopy, firstScriptOfType.IsStuckMissile());
                    WorldPosition worldPosition = firstScriptOfType.GameEntityWithWorldPosition.WorldPosition;
                    if (equipmentIndex != EquipmentIndex.None && worldPosition.GetNavMesh() != UIntPtr.Zero && this.IsItemAvailable(firstScriptOfType) && this.agent.CanMoveDirectlyToPosition(worldPosition))
                    {
                        if (best_eqiupment != TaleWorlds.Core.WeaponClass.Undefined && firstScriptOfType.WeaponCopy.Item.PrimaryWeapon.WeaponClass == best_eqiupment)
                        {
                            second_need = TaleWorlds.Core.WeaponClass.Undefined;
                            return firstScriptOfType;
                        }
                        else if(second_need != TaleWorlds.Core.WeaponClass.Undefined && firstScriptOfType.WeaponCopy.Item.PrimaryWeapon.WeaponClass == second_need)
                        {
                            result = firstScriptOfType;
                        }
                    }
                }
            }
            second_need = TaleWorlds.Core.WeaponClass.Undefined;
            return result;
        }

        private bool IsItemAvailable(SpawnedItemEntity item)
        {
            if (!this.agent.CanReachAndUseObject(item, 0f) || !this.agent.ObjectHasVacantPosition(item) || item.MovingAgents.Count > 0)
            {
                return false;
            }
            return true;
        }

        private void find_object()
        {
            Vec2 agent_position = agent.Position.AsVec2;
            bool can_arrive = false;
            if((origin_position-agent_position).Length < 1e-8)
            {
                float x = MBRandom.RandomFloatRanged(3f, 5f) * MBRandom.RandomFloat > 0.5f ? 1f : -1f;
                float y = MBRandom.RandomFloatRanged(3f, 5f) * MBRandom.RandomFloat > 0.5f ? 1f : -1f;
                Vec2 add_position = new Vec2(1, 1);
                if((agent_position+add_position).Length < max_range)
                {
                    WorldPosition world = new WorldPosition(Mission.Current.Scene, new Vec3(agent_position+add_position));
                    agent.SetScriptedPosition(ref world, false, Agent.AIScriptedFrameFlags.ConsiderRotation);
                    can_arrive = true;
                }
            }
            else
            {
                origin_position = agent_position;
            }
        }

        public void escape()
        {
            can_escape = true;
            agent.SetAgentFlags(agent.GetAgentFlags() | AgentFlag.CanGetAlarmed);
            AgentAIStateFlagComponent component = agent.GetComponent<AgentAIStateFlagComponent>();
            if (component != null)
            {
                component.CurrentWatchState = AgentAIStateFlagComponent.WatchState.Alarmed;
            }
            agent.AddComponent(new ItemPickupAgentComponent(agent));
            agent.DisableScriptedMovement();
        }

        public const float max_range = 50f;
    }
}
