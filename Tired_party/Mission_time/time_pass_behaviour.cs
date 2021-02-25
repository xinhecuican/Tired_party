using MCM.Abstractions.Settings.Base.Global;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.SaveSystem;
using Tired_party.Helper;

namespace Tired_party.Mission_time
{
    class time_pass_behaviour : CampaignBehaviorBase
    {
        public override void RegisterEvents()
        {
            CampaignEvents.MapEventEnded.AddNonSerializedListener(this, new Action<MapEvent>(time_pass));
            CampaignEvents.HourlyTickEvent.AddNonSerializedListener(this, new Action(end_time));
            CampaignEvents.MapEventStarted.AddNonSerializedListener(this, new Action<MapEvent, PartyBase, PartyBase>(mapevent_start));
        }

        public override void SyncData(IDataStore dataStore)
        {
            dataStore.SyncData("tired_party_time_pass", ref pass_time);
            dataStore.SyncData("tired_party_enable_pass", ref enable_pass);
            dataStore.SyncData("tired_party_active_party", ref active_party);
            dataStore.SyncData("time_pass_main_party_position", ref main_party_position);
        }

        private void mapevent_start(MapEvent mapevent, PartyBase attackerparty, PartyBase defenderparty)
        {
            if (mapevent == MapEvent.PlayerMapEvent && !GlobalSettings<mod_setting>.Instance.is_ban_time_pass)
            {
                Campaign.Current.SetTimeControlModeLock(false);
            }
        }

        private void time_pass(MapEvent mapevent)
        {
            if (mapevent == MapEvent.PlayerMapEvent && !GlobalSettings<mod_setting>.Instance.is_ban_time_pass)
            {
                try
                {
                    enable_pass = true;
                    pass_time = (int)missiontime_data.pass_time;
                    main_party_position = Campaign.Current.MainParty.Position2D;
                    active_party = new List<MobileParty>();
                    if (mapevent.InvolvedParties != null)
                    {
                        foreach (PartyBase party in mapevent.InvolvedParties)
                        {
                            if (party.IsActive && party.IsMobile && party.MobileParty.IsActive && !party.MobileParty.IsMainParty)
                            {
                                active_party.Add(party.MobileParty);
                            }
                        }
                    }


                    Campaign.Current.SetTimeControlModeLock(false);
                    Campaign.Current.TimeControlMode = CampaignTimeControlMode.UnstoppableFastForward;
                    Campaign.Current.SetTimeControlModeLock(true);
                }
                catch(Exception e)
                {
                    MethodInfo methodInfo = MethodBase.GetCurrentMethod() as MethodInfo;
                    debug_helper.HandleException(e, methodInfo, "submodule load error");
                }
            }
        }

        private void end_time()
        {
            try
            {
                if (enable_pass)
                {
                    pass_time--;
                    if (pass_time <= 0)
                    {
                        foreach (MobileParty party in active_party)
                        {
                            party.Ai.RethinkAtNextHourlyTick = true;
                        }
                        active_party.Clear();
                        Campaign.Current.SetTimeControlModeLock(false);
                        Campaign.Current.TimeControlMode = CampaignTimeControlMode.Stop;
                        enable_pass = false;
                        return;
                    }
                    foreach (MobileParty party in active_party)
                    {
                        party.SetMoveModeHold();
                    }
                    Campaign.Current.MainParty.SetMoveGoToPoint(main_party_position);
                }
            }
            catch(Exception e)
            {
                MethodInfo methodInfo = MethodBase.GetCurrentMethod() as MethodInfo;
                debug_helper.HandleException(e, methodInfo, "submodule load error");
            }
        }
        [SaveableField(1)]
        float pass_time = 0f;
        [SaveableField(2)]
        public bool enable_pass = false;
        [SaveableField(3)]
        public Vec2 main_party_position;
        [SaveableField(4)]
        public List<MobileParty> active_party = new List<MobileParty>();
    }
}
