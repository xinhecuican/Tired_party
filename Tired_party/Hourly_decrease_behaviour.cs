﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace Tired_party
{
    class Hourly_decrease_behaviour : CampaignBehaviorBase
    {
        public override void RegisterEvents()
        {
            CampaignEvents.HourlyTickEvent.AddNonSerializedListener(this, new Action(this.HourlyEvent));
        }

        private void HourlyEvent()
        {
            
                float hour = CampaignTime.Now.GetHourOfDay;
                InformationManager.DisplayMessage(new InformationMessage(new TextObject("{=test_of_me_only}Time:" + hour.ToString(), null).ToString()
                        , Color.FromUint(4282569842U)));
        }

        public override void SyncData(IDataStore dataStore)
        {

        }
    }
}
