using MCM.Abstractions.Settings.Base.Global;
using MCM.Utils;
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
using Tired_party.Information_Screen;

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
            party.information.Clear();
            foreach(information_data data in Party_tired.Current.Information)
            {
                party.Information.Add(data);
            }
            mcm_data.save_data();
            /*mcm_data.is_ban = GlobalSettings<mod_setting>.Instance.is_ban;
            mcm_data.is_ban_army = GlobalSettings<mod_setting>.Instance.is_ban_army;
            mcm_data.is_ban_capture_information = GlobalSettings<mod_setting>.Instance.is_ban_capture_information;
            mcm_data.is_ban_information = GlobalSettings<mod_setting>.Instance.is_ban_information;
            mcm_data.is_ban_married_information = GlobalSettings<mod_setting>.Instance.is_ban_married_information;
            mcm_data.is_ban_release_information = GlobalSettings<mod_setting>.Instance.is_ban_release_information;
            mcm_data.limit_speed = GlobalSettings<mod_setting>.Instance.limit_speed;
            mcm_data.morale_reduce = GlobalSettings<mod_setting>.Instance.morale_reduce;
            mcm_data.persist_time = GlobalSettings<mod_setting>.Instance.persist_time;
            mcm_data.recovery_in_day_time = GlobalSettings<mod_setting>.Instance.recovery_in_day_time;
            mcm_data.recovery_in_day_time_main = GlobalSettings<mod_setting>.Instance.recovery_in_day_time_main;
            mcm_data.recovery_in_night_time = GlobalSettings<mod_setting>.Instance.recovery_in_night_time;
            mcm_data.recovery_in_night_time_main = GlobalSettings<mod_setting>.Instance.recovery_in_night_time_main;*/
        }

        private void LoadData(CampaignGameStarter starter)
        {
            foreach (KeyValuePair<MobileParty, tired_party_data> key in party.Party_tired_rate)
            {
                Party_tired.Current.Party_tired_rate[key.Key] = key.Value;
            }

            foreach(information_data data in party.Information)
            {
                Party_tired.Current.Information.Add(data);
            }
            mcm_data.load_data();
            /*GlobalSettings<mod_setting>.Instance.is_ban = mcm_data.is_ban  ;
            GlobalSettings<mod_setting>.Instance.is_ban_army = mcm_data.is_ban_army ;
            GlobalSettings<mod_setting>.Instance.is_ban_capture_information = mcm_data.is_ban_capture_information ;
            GlobalSettings<mod_setting>.Instance.is_ban_information = mcm_data.is_ban_information ;
            GlobalSettings<mod_setting>.Instance.is_ban_married_information = mcm_data.is_ban_married_information ;
            GlobalSettings<mod_setting>.Instance.is_ban_release_information = mcm_data.is_ban_release_information ;
            GlobalSettings<mod_setting>.Instance.limit_speed = mcm_data.limit_speed  ;
            GlobalSettings<mod_setting>.Instance.morale_reduce = mcm_data.morale_reduce  ;
            GlobalSettings<mod_setting>.Instance.persist_time = mcm_data.persist_time  ;
            GlobalSettings<mod_setting>.Instance.recovery_in_day_time = mcm_data.recovery_in_day_time  ;
            GlobalSettings<mod_setting>.Instance.recovery_in_day_time_main = mcm_data.recovery_in_day_time_main  ;
            GlobalSettings<mod_setting>.Instance.recovery_in_night_time = mcm_data.recovery_in_night_time  ;
            GlobalSettings<mod_setting>.Instance.recovery_in_night_time_main = mcm_data.recovery_in_night_time_main  ;*/
        }

        

        public override void SyncData(IDataStore dataStore)
        {
            try
            {
                dataStore.SyncData("tired_party_data", ref party._party_tired_rate);
                dataStore.SyncData("tired_party_information_data", ref party.information);
                dataStore.SyncData("tired_party_mcm", ref mcm_data);
                
            }
            catch (Exception)
            {
            }
            finally
            {
                
            }
        }
        public static Party_tired party = new Party_tired();
        public static MCMsetting_data mcm_data = new MCMsetting_data();
       
    }
}
