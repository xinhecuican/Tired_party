﻿using HarmonyLib;
using MCM.Abstractions.Settings.Base.Global;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.CampaignSystem;
using TaleWorlds.DotNet;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;
using Tired_party.Mission_time;

namespace Tired_party.sneak_attack
{
    [HarmonyPatch]
    class isUnderPlayerCommendGetter_patch
    {
        public static MethodBase TargetMethod()
        {
            return AccessTools.PropertyGetter(typeof(PartyGroupAgentOrigin), "IsUnderPlayersCommand");
        }

        public static bool Prefix(PartyGroupAgentOrigin __instance, ref bool __result)
        {
            if(!Party_tired.is_wish_mission || __instance.Party.Owner == Hero.MainHero || __instance.Party == PartyBase.MainParty ||
                __instance.Troop == Hero.MainHero.CharacterObject)
            {
                return true;
            }
            arrive_time_data data;
            if(GlobalSettings<mod_setting>.Instance.reinforcement_mode == 0)
            {
                return true;
            }
            if((data = search(__instance.Party.MobileParty)) != null)
            {
                
                if(GlobalSettings<mod_setting>.Instance.reinforcement_mode == 2)
                {
                    __result = false;
                    return false;
                }
                else
                {
                    Vec2 main_party_direction = MapEvent.PlayerMapEvent.Position - Campaign.Current.MainParty.Position2D;
                    float angle = main_party_direction.AngleBetween(data.enter_direction);
                    if((angle >= Math.PI / 2 ) || (angle <= -Math.PI / 2))
                    {
                        __result = false;
                        return false;
                    }
                }
            }
            return true;
        }

        private static arrive_time_data search(MobileParty party)
        {
            foreach(arrive_time_data data in controller.ready_to_place)
            {
                if(party == data.party.Party.MobileParty)
                {
                    return data;
                }
            }
            return null;
        }

        private static agent_spawn_controller controller
        {
            get
            {
                if(_controller == null)
                {
                    _controller = Mission.Current.GetMissionBehaviour<agent_spawn_controller>();
                }
                return _controller;
            }
        }

        public static agent_spawn_controller _controller;

    }
}
