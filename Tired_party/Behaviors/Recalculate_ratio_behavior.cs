﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.Library;
using Tired_party.Helper;

namespace Tired_party.Behaviors
{
    class Recalculate_ratio_behavior : CampaignBehaviorBase
    {
        public override void RegisterEvents()
        {
            CampaignEvents.OnPartySizeChangedEvent.AddNonSerializedListener(this, new Action<PartyBase>(on_party_size_changed));
            CampaignEvents.MobilePartyDestroyed.AddNonSerializedListener(this, new Action<MobileParty, PartyBase>(on_mobile_party_destroyed));
            CampaignEvents.DailyTickEvent.AddNonSerializedListener(this, new Action(daily_tick));
            CampaignEvents.OnNewGameCreatedEvent9.AddNonSerializedListener(this, new Action(new_game_behavior));
            CampaignEvents.OnPartyRemovedEvent.AddNonSerializedListener(this, new Action<PartyBase>(on_party_remove));
            CampaignEvents.WeeklyTickEvent.AddNonSerializedListener(this, new Action(weekly_tick));
            CampaignEvents.MobilePartyCreated.AddNonSerializedListener(this, new Action<MobileParty>(mobile_party_create));
        }

        public override void SyncData(IDataStore dataStore)
        {
        }

        private void mobile_party_create(MobileParty parties)
        {
            if (parties == null || parties.IsCaravan || parties.IsVillager)
            {
                return;
            }
            if (!Party_tired.Current.Party_tired_rate.ContainsKey(parties))
            {
                Party_tired.add_to_dict(parties);
            }
        }
        private void weekly_tick()
        {
            for(int i=0; i<Party_tired.Current.information.Count; i++)
            {
                Party_tired.Current.information[i].delete_outdated_information();
            }
        }

        private void on_party_remove(PartyBase partyBase)
        {
            if (partyBase.IsMobile &&partyBase.MobileParty != null && Party_tired.Current.Party_tired_rate.ContainsKey(partyBase.MobileParty))
            {
                Party_tired.Current.Party_tired_rate.Remove(partyBase.MobileParty);
            }
        }

       private void new_game_behavior()
        {
            MBReadOnlyList<MobileParty> parties = Campaign.Current.MobileParties;
            for (int i = 0; i < parties.Count; i++)
            {
                if (parties[i] == null || parties[i].IsCaravan || parties[i].IsVillager)
                {
                    continue;
                }
                if (!Party_tired.Current.Party_tired_rate.ContainsKey(parties[i]))
                {
                    Party_tired.add_to_dict(parties[i]);
                }

            }
        }

        private void visibility_change(PartyBase party)
        {
            if(party != null && Party_tired.Current != null && party.IsMobile)
            {
                MobileParty mobile = party.MobileParty;
                if(mobile != null && Party_tired.Current.Party_tired_rate.ContainsKey(mobile))
                {
                    if(!mobile.IsVisible)
                    {
                        Party_tired.Current.Party_tired_rate.Remove(mobile);
                    }
                }
            }
        }

        private void daily_tick()
        {
            MBReadOnlyList<MobileParty> parties = Campaign.Current.MobileParties;
            for(int i=0; i<parties.Count; i++)
            {
                if(parties[i] == null || parties[i].IsCaravan || parties[i].IsVillager)
                {
                    continue;
                }
                if(!Party_tired.Current.Party_tired_rate.ContainsKey(parties[i]))
                {
                    Party_tired.add_to_dict(parties[i]);
                }
                
            }
            var keys = new List<MobileParty>(Party_tired.Current.Party_tired_rate.Keys);
            for(int i=0; i<keys.Count; i++)
            {
                if(!keys[i].IsActive)
                {
                    Party_tired.Current.Party_tired_rate.Remove(keys[i]);
                }
            }
        }

        private void on_mobile_party_destroyed(MobileParty mobile, PartyBase destroyerparty)
        {
            if(mobile != null && Party_tired.Current.Party_tired_rate.ContainsKey(mobile))
            {
                Party_tired.Current.Party_tired_rate.Remove(mobile);
            }
        }

        private void on_party_size_changed(PartyBase party)
        {
            try
            {
                if (party == null || Party_tired.Current == null || !party.IsMobile)
                {
                    return;
                }
                if (party.MobileParty != null && Party_tired.Current.Party_tired_rate.ContainsKey(party.MobileParty))
                {               
                    if (Math.Abs(party.MobileParty.MemberRoster.TotalManCount - Party_tired.Current.Party_tired_rate[party.MobileParty].Number) >= 5)
                    {
                        
                        Party_tired.Current.Party_tired_rate[party.MobileParty].Reduce_rate = Calculate_party_tired.calculate_ratio(party.MobileParty);
                        Party_tired.Current.Party_tired_rate[party.MobileParty].Number = party.MobileParty.MemberRoster.TotalManCount;
                    }
                }
            }
            catch(Exception e)
            {
                MethodInfo methodInfo = MethodBase.GetCurrentMethod() as MethodInfo;
                debug_helper.HandleException(e, methodInfo, "hourly event error");
            }
        }
    }
}
