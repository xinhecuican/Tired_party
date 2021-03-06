﻿using HarmonyLib;
using MCM.Abstractions.Settings.Base.Global;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Library;

namespace Tired_party.Mission_time
{
    [HarmonyPatch(typeof(MapEvent), "GetNearbyFreeParties")]
    class check_radius_patch
    {

        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator il)
        {
            /*if(GlobalSettings<mod_setting>.Instance.ban_reinforcement)
            {
                return instructions;
            }*/
            List<CodeInstruction> list = new List<CodeInstruction>(instructions);
            for(int i=0; i<list.Count; i++)
            {
                if(list[i].opcode == OpCodes.Ldc_R4)
                {
                    list[i].operand = SubModule.battle_radius;
                }
            }
            return list.AsEnumerable<CodeInstruction>();
        }
    }
}
