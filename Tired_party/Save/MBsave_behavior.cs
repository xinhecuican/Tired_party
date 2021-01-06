﻿using MCM.Utils;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;
using TaleWorlds.SaveSystem;
using Tired_party.Helper;

namespace Tired_party.Save
{
    class MBsave_behavior : CampaignBehaviorBase
    {
        public override void RegisterEvents()
        {
            
            CampaignEvents.OnBeforeSaveEvent.AddNonSerializedListener(this, new Action(saveData));
            CampaignEvents.OnGameLoadedEvent.AddNonSerializedListener(this, new Action<CampaignGameStarter>(LoadData));
        }

        private void saveData()
        {
            foreach (KeyValuePair<MobileParty, tired_party_data> key in Party_tired.Current.Party_tired_rate)
            {
                party.Party_tired_rate[key.Key] = key.Value;
            }
        }

        private void LoadData(CampaignGameStarter starter)
        {
            foreach (KeyValuePair<MobileParty, tired_party_data> key in party.Party_tired_rate)
            {
                Party_tired.Current.Party_tired_rate[key.Key] = key.Value;
            }
        }

        

        public override void SyncData(IDataStore dataStore)
        {
            try
            {
                dataStore.SyncData("tired_party_data", ref party._party_tired_rate);
                
            }
            catch (Exception)
            {
            }
            finally
            {
                
            }
        }
        public static Party_tired party = new Party_tired();
       
    }
}