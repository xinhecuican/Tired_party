using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.Diamond;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using Tired_party.Helper;

namespace Tired_party.Behaviors
{
    class Hourly_change_behaviour : CampaignBehaviorBase
    {
        public override void RegisterEvents()
        {
            CampaignEvents.HourlyTickEvent.AddNonSerializedListener(this, new Action(this.HourlyEvent));
            //CampaignEvents.HourlyTickPartyEvent.AddNonSerializedListener(this, new Action<MobileParty>(Hourly_party_event));
        }
        /*
        private void Hourly_party_event(MobileParty party)
        {
            if (SubModule.Current == null)
            {
                return;
            }

            try
            {
                if(party != null && Party_tired.Current.Party_tired_rate.ContainsKey(party))
                {
                    if(CampaignTime.Now.IsDayTime)
                    {
                        if (party.ShortTermBehavior == AiBehavior.Hold || party.AtCampMode || !party.IsMoving)
                        {
                            Party_tired.Current.Party_tired_rate[party].Now += Party_tired.recovery_in_day_time;
                        }
                        else
                        {
                            Party_tired.Current.Party_tired_rate[party].Now -= Party_tired.Current.Party_tired_rate[party].Reduce_rate;
                        }
                    }
                    else
                    {
                        if (party.ShortTermBehavior == AiBehavior.Hold || party.AtCampMode || !party.IsMoving)
                        {
                            Party_tired.Current.Party_tired_rate[party].Now += Party_tired.recovery_in_night_time;
                        }
                        else
                        {
                            Party_tired.Current.Party_tired_rate[party].Now -= Party_tired.Current.Party_tired_rate[party].Reduce_rate * 1.1f;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                MethodInfo methodInfo = MethodBase.GetCurrentMethod() as MethodInfo;
                debug_helper.HandleException(e, methodInfo, "hourly event error");
            }
        }*/
        
        private void HourlyEvent()
        {
            if(Party_tired.Current == null)
            {
                return;
            }
            float hour = CampaignTime.Now.GetHourOfDay;
            InformationManager.DisplayMessage(new InformationMessage(new TextObject("{=test_of_me_only}Time:" + hour.ToString(), null).ToString()
                        , Color.FromUint(4282569842U)));
            try
            {
                bool is_daytime = CampaignTime.Now.IsDayTime;
                foreach (var party in Party_tired.Current.Party_tired_rate)
                {
                    if (party.Key.ShortTermBehavior == AiBehavior.Hold || party.Key.AtCampMode || party.Key.Position2D == party.Key.TargetPosition)
                    {
                        if (is_daytime)
                        {
                            party.Value.Now += is_daytime ? Party_tired.recovery_in_day_time : Party_tired.recovery_in_night_time;
                        }
                        else
                        {
                            party.Value.Now -= is_daytime ? party.Value.Reduce_rate : party.Value.Reduce_rate * 1.1f;
                        }
                    }
                    if(party.Value.Now <= 0.3)
                    {
                        party.Value.Morale += 0.3f - party.Value.Now;
                    }
                    else if(party.Value.Morale > 0)
                    {
                        party.Value.Morale -= 0.3f;
                    }
                        
                    if(party.Key == Campaign.Current.MainParty)
                    {
                        if (party.Value.Now < 0.5f)
                        {
                            message_helper.SimpleMessage("部队还剩" + Calculate_party_tired.calculate_remaining_hours(party.Value).ToString() + "小时达到极限");
                        }
                        else if(party.Value.Now < 0.3f)
                        {
                            message_helper.TechnicalMessage("部队还剩"+Calculate_party_tired.calculate_remaining_hours(party.Value).ToString()+"小时达到极限");
                        }
                        else if(party.Value.Now == 0)
                        {
                            message_helper.ErrorMessage("部队需要休息");
                        }
                    }
                }
            }
            catch(Exception e)
            {
                MethodInfo methodInfo = MethodBase.GetCurrentMethod() as MethodInfo;
                debug_helper.HandleException(e, methodInfo, "hourly event error");
            }
        
        }

        public override void SyncData(IDataStore dataStore)
        {

        }
    }
}
