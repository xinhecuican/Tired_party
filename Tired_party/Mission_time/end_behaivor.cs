﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.MountAndBlade;

namespace Tired_party.Mission_time
{
    class end_behaivor : MissionLogic
    {
        public override void OnBattleEnded()
        {
            Party_tired.is_wish_mission = false;
            missiontime_data.current = null;
        }
    }
}
