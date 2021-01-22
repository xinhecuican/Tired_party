using MCM.Abstractions.Settings.Base.Global;
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
using TaleWorlds.MountAndBlade;
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
            if(Party_tired.Current == null || GlobalSettings<mod_setting>.Instance.is_ban)
            {
                return;
            }
            try
            {
                bool is_daytime = CampaignTime.Now.IsDayTime;
                foreach (var party in Party_tired.Current.Party_tired_rate)
                {
                    float value_event = (party.Key.MapEvent != null || party.Key.BesiegedSettlement != null) ? 0.6f : 1f;
                    float value_settlement = party.Key.CurrentSettlement != null ? 1.2f : 1f;
                    if (party.Key.IsMainParty)
                    {
                        if(party.Key.Army != null ? is_not_move(party.Key.Army.LeaderParty) : is_not_move(party.Key))
                        {
                            party.Value.Now += value_event * value_settlement * (is_daytime ? GlobalSettings<mod_setting>.Instance.recovery_in_day_time_main : GlobalSettings<mod_setting>.Instance.recovery_in_night_time_main);
                        }
                        else
                        {
                            party.Value.Now -= is_daytime ? party.Value.Reduce_rate : party.Value.Reduce_rate * 1.1f;
                        }
                    }
                    else
                    {
                        if (party.Key.Army != null ? is_not_move(party.Key.Army.LeaderParty) : is_not_move(party.Key))
                        {
                            
                            party.Value.Now += value_settlement * value_event * (is_daytime ? GlobalSettings<mod_setting>.Instance.recovery_in_day_time : GlobalSettings<mod_setting>.Instance.recovery_in_night_time);
                        }
                        else
                        {
                            party.Value.Now -= is_daytime ? party.Value.Reduce_rate : party.Value.Reduce_rate * 1.1f;
                        }
                    }

                    if(party.Value.Now > 0.8)
                    {
                        party.Value.Morale = -5f;
                    }
                    else if (party.Value.Now <= 0.3)
                    {
                        party.Value.Morale += 3 * (0.3f - party.Value.Now);
                    }
                    else if (party.Value.Morale > 1e-8)
                    {
                        if (party.Value.Morale > 0)
                        {
                            party.Value.Morale = party.Value.Morale - 0.3f < 0 ? 0 : party.Value.Morale - 1f;
                        }
                        else
                        {
                            party.Value.Morale = party.Value.Morale + 0.3f > 0 ? 0 : party.Value.Morale + 1f;
                        }
                    }

                    if(party.Key == Campaign.Current.MainParty)
                    {
                        if (party.Value.Now < 0.5f  && party.Value.Now >= 0.3f&& CampaignTime.Now.GetHourOfDay % 6 == 0)
                        {
                            if(BannerlordConfig.Language.Equals("简体中文"))
                                message_helper.SimpleMessage("部队还剩" + Calculate_party_tired.calculate_remaining_hours(party.Value).ToString() + "小时达到极限");
                            else
                                message_helper.SimpleMessage("party remain " + Calculate_party_tired.calculate_remaining_hours(party.Value).ToString() + " hours");
                        }
                        else if(party.Value.Now < 0.3f  && party.Value.Now > 0f && CampaignTime.Now.GetHourOfDay % 3 == 0)
                        {
                            if(BannerlordConfig.Language.Equals("简体中文"))
                                message_helper.TechnicalMessage("部队还剩"+Calculate_party_tired.calculate_remaining_hours(party.Value).ToString()+"小时达到极限");
                            else
                                message_helper.TechnicalMessage("party remain " + Calculate_party_tired.calculate_remaining_hours(party.Value).ToString() + " hours");
                        }
                        else if(party.Value.Now == 0 && CampaignTime.Now.GetHourOfDay % 2 == 0)
                        {
                            if(BannerlordConfig.Language.Equals("简体中文"))
                                message_helper.ErrorMessage("部队需要休息");
                            else
                                message_helper.ErrorMessage("Troop needs rest ");
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

        public static bool is_not_move(MobileParty party)
        {
            bool flag_busy = Party_tired.Current.Party_tired_rate.ContainsKey(party) ? Party_tired.Current.Party_tired_rate[party].is_busy : false;
            bool flag_besige_fight = party.BesiegerCamp != null ? party.BesiegerCamp.IsReadyToBesiege : false;
            return (party.DefaultBehavior == AiBehavior.Hold
                          || party.Position2D == party.TargetPosition || !party.IsMoving) && (!flag_busy && !flag_besige_fight);
        }

        public override void SyncData(IDataStore dataStore)
        {

        }
    }
}
