﻿using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade.GauntletUI.Mission.Singleplayer;
using TaleWorlds.MountAndBlade.ViewModelCollection.HUD.KillFeed;
using TaleWorlds.MountAndBlade.ViewModelCollection.HUD.KillFeed.Personal;
using Tired_party.Helper;

namespace Tired_party.Mission_time
{
    [HarmonyPatch(typeof(MissionGauntletKillNotificationSingleplayerUIHandler))]
    class Nearlycome_notification
    {
        [HarmonyPostfix]
        [HarmonyPatch("OnMissionScreenInitialize")]
        public static void postfix(MissionGauntletKillNotificationSingleplayerUIHandler __instance)
        {
            try
            {
                notification_list = ((SPKillFeedVM)AccessTools.Field(typeof(MissionGauntletKillNotificationSingleplayerUIHandler), "_dataSource").GetValue(__instance)).PersonalFeed.NotificationList;
            }
            catch(Exception e)
            {
                MethodInfo methodInfo = MethodBase.GetCurrentMethod() as MethodInfo;
                debug_helper.HandleException(e, methodInfo, "submodule load error");
            }
        }
        
        [HarmonyPostfix]
        [HarmonyPatch("OnMissionScreenFinalize")]
        public static void HarmonyFinalizer()
        {
            notification_list = null;
        }
        public static MBBindingList<SPPersonalKillNotificationItemVM> notification_list;
    }

    
}
