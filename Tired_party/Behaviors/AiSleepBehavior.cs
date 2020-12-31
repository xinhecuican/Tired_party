using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.Library;
using Tired_party.Helper;

namespace Tired_party.Behaviors
{
    class AiSleepBehavior : CampaignBehaviorBase
    {
        public override void RegisterEvents()
        {
            CampaignEvents.HourlyTickPartyEvent.AddNonSerializedListener(this, new Action<MobileParty>(PartyHourlyTick));
        }

        public override void SyncData(IDataStore dataStore)
        {
        }

        public void AiHourlyTick(MobileParty mobile, PartyThinkParams p)
        {
            if (mobile == Party_tired.test_party)
            {
                InformationManager.DisplayMessage(new InformationMessage("aihourlytick"));
            }
        }

        public void PartyHourlyTick(MobileParty mobileParty)
        {
            /*
             * 目标：让ai会睡觉（晚上更为倾向）
             * 追逐敌军时尽量不让他睡觉
             * 没事时保持充足体力
             * 0.5是临界值
             */
            
            try
            {
                if (Party_tired.Current == null || !Party_tired.Current.Party_tired_rate.ContainsKey(mobileParty))
                {
                    return;
                }
                if (mobileParty.Army != null && mobileParty.Army.LeaderParty != mobileParty)
                {
                    foreach(MobileParty party in mobileParty.Army.LeaderParty.AttachedParties)
                    {
                        if(party == mobileParty)
                        {
                            return;
                        }
                    }
                }

                bool flag_is_day_time = CampaignTime.Now.IsDayTime;

                if (mobileParty.Army != null && mobileParty.Army.LeaderParty == mobileParty && mobileParty.LeaderHero != null)//军队sleep ai
                {
                    if (mobileParty.Army.AIBehavior == Army.AIBehaviorFlags.AssaultingTown || mobileParty.Army.AIBehavior == Army.AIBehaviorFlags.Besieging
                        || mobileParty.Army.AIBehavior == Army.AIBehaviorFlags.Defending || mobileParty.Army.AIBehavior == Army.AIBehaviorFlags.Gathering
                        || mobileParty.Army.AIBehavior == Army.AIBehaviorFlags.Raiding)
                    {
                        return;
                    }
                    float army_now_tired = Party_tired.Current.Party_tired_rate[mobileParty].Now;
                    foreach (MobileParty party in mobileParty.AttachedParties)
                    {
                        if (Party_tired.Current.Party_tired_rate.ContainsKey(party))
                        {
                            army_now_tired += Party_tired.Current.Party_tired_rate[party].Now;
                        }
                    }
                    army_now_tired /= mobileParty.Army.Parties.Count;
                    float ans_army_tired = Party_tired.begin_to_decrease - army_now_tired > 0 ? (Party_tired.begin_to_decrease - army_now_tired) * 8 / 3.0f + 0.2f : 0.2f;
                    bool flag_army_go_to_someplace_behavior = mobileParty.Army.AIBehavior == Army.AIBehaviorFlags.GoToSettlement;
                    bool flag_army_not_busy = mobileParty.Army.AIBehavior == Army.AIBehaviorFlags.Patrolling;
                    float cohesion_remain_time = (mobileParty.Army.CohesionThresholdForDispersion - mobileParty.Army.Cohesion) / mobileParty.Army.CohesionChange;
                    float ans_army_go_to_someplace = 1f;
                    if (flag_army_go_to_someplace_behavior || flag_army_not_busy)
                    {
                        if (army_now_tired - 0.5f < 0 && army_now_tired - 0.3 > 0)
                        {
                            ans_army_go_to_someplace = (1 - army_now_tired) * 5 * (0.8f + 0.2f * mobileParty.Position2D.DistanceSquared(mobileParty.TargetPosition) / (Campaign.MapDiagonal * Campaign.MapDiagonal));
                        }
                    }
                    float ans_army_time = flag_is_day_time ? 0.8f : 1.2f;
                    float ans_army_cohesion = cohesion_remain_time - 2.0f < 0 ? 0.6f : 0.6f + 0.4f * (cohesion_remain_time - 2.0f) / cohesion_remain_time;       //剩余时间越多，就越不着急，可以休息
                    float ans_army = ans_army_cohesion * ans_army_go_to_someplace * ans_army_tired * ans_army_time;
                    if( ans_army > 0.7f)
                    {
                        mobileParty.Army.AIBehavior = Army.AIBehaviorFlags.Waiting;
                    }
                }

                if (
                    mobileParty.DefaultBehavior == AiBehavior.DefendSettlement || mobileParty.DefaultBehavior == AiBehavior.AssaultSettlement
                    || mobileParty.DefaultBehavior == AiBehavior.BesiegeSettlement || mobileParty.ShortTermBehavior == AiBehavior.FleeToPoint)
                {
                    return;
                }

                if(Party_tired.Current.Party_tired_rate[mobileParty].need_recovery)
                {
                    switch(Party_tired.Current.Party_tired_rate[mobileParty].AiBehavior)
                    {
                        case AiBehavior.EngageParty: mobileParty.SetMoveEngageParty(Party_tired.Current.Party_tired_rate[mobileParty].target_party);break;
                        case AiBehavior.EscortParty: mobileParty.SetMoveEscortParty(Party_tired.Current.Party_tired_rate[mobileParty].target_party);break;
                        case AiBehavior.GoAroundParty: mobileParty.SetMoveGoAroundParty(Party_tired.Current.Party_tired_rate[mobileParty].target_party);break;
                        case AiBehavior.GoToPoint: mobileParty.SetMoveGoToPoint(Party_tired.Current.Party_tired_rate[mobileParty].ai_behavior_target);break;
                        case AiBehavior.GoToSettlement: mobileParty.SetMoveGoToSettlement(Party_tired.Current.Party_tired_rate[mobileParty].ai_behavior_object);break;
                    }
                    Party_tired.Current.Party_tired_rate[mobileParty].need_recovery = false;
                }
                float now_tired = Party_tired.Current.Party_tired_rate[mobileParty].Now;
                float ans_tired = Party_tired.begin_to_decrease - now_tired > 0 ? (float)Math.Sqrt((Party_tired.begin_to_decrease - now_tired) * 3.3f) + 0.2f : 0.2f;
                bool flag_follow_behavior = (mobileParty.DefaultBehavior == AiBehavior.GoAroundParty && mobileParty.TargetParty != null) || mobileParty.DefaultBehavior == AiBehavior.EscortParty;
                bool flag_engage_behavior = mobileParty.ShortTermBehavior == AiBehavior.EngageParty;
                bool flag_go_to_someplace_behavior = mobileParty.ShortTermBehavior == AiBehavior.GoToPoint || mobileParty.ShortTermBehavior == AiBehavior.GoToSettlement;
                
                float ans_engage_behavior = 1f;
                if (flag_engage_behavior)
                {
                    float party_speed = mobileParty.ComputeSpeed();
                    float enemy_speed = mobileParty.ComputeSpeed();
                    float double_speed = (float)Math.Pow(party_speed / enemy_speed, 2);
                    ans_engage_behavior = (float)(party_speed < enemy_speed ? 1.1f : 0.8f * double_speed);
                }

                float ans_follow_behavior = 1f;
                if (flag_follow_behavior)
                {
                    ans_follow_behavior = ans_tired > Party_tired.begin_to_decrease ? 0.6f + 0.4f * mobileParty.Position2D.DistanceSquared(mobileParty.TargetParty.Position2D) / (Campaign.MapDiagonal * Campaign.MapDiagonal) : 1f;
                }

                float ans_go_to_someplace_behavior = 1f;
                if (flag_go_to_someplace_behavior && mobileParty.TargetPosition != null)
                {
                    if (now_tired > Party_tired.begin_to_decrease && now_tired < 0.6f)
                    {
                        ans_go_to_someplace_behavior = (1-now_tired) * 5 * (0.8f + 0.2f * mobileParty.Position2D.DistanceSquared(mobileParty.TargetPosition) / (Campaign.MapDiagonal * Campaign.MapDiagonal));
                    }
                    else if(now_tired < Party_tired.begin_to_decrease)
                    {
                        ans_go_to_someplace_behavior = 1.2f;
                    }
                }

                float ans_time = 1.2f;
                if (flag_is_day_time)
                {
                    ans_time = 0.8f;
                }

                float ans = ans_time * ans_tired * ans_follow_behavior * ans_engage_behavior * ans_go_to_someplace_behavior;
                if (mobileParty == Party_tired.test_party)
                {
                    InformationManager.DisplayMessage(new InformationMessage(ans.ToString()));
                }
                
                if(ans > 0.7f)
                {
                    Party_tired.Current.Party_tired_rate[mobileParty].AiBehavior = mobileParty.DefaultBehavior;
                    Party_tired.Current.Party_tired_rate[mobileParty].ai_behavior_target = mobileParty.TargetPosition;
                    Party_tired.Current.Party_tired_rate[mobileParty].ai_behavior_object = mobileParty.TargetSettlement;
                    Party_tired.Current.Party_tired_rate[mobileParty].target_party = mobileParty.TargetParty;
                    Party_tired.Current.Party_tired_rate[mobileParty].need_recovery = true;
                    mobileParty.SetMoveModeHold();
                }
                
            }
            catch(Exception e)
            {
                MethodInfo methodInfo = MethodBase.GetCurrentMethod() as MethodInfo;
                debug_helper.HandleException(e, methodInfo, "ai hourly tick");
            }
        }
        /*
        private string do_things(AIBehaviorTuple tuple)
        {
            switch(tuple.AiBehavior)
            {
                case AiBehavior.AssaultSettlement: return "AssaultSettlement";
                case AiBehavior.BesiegeSettlement: return "besiege settlement";
                case AiBehavior.DefendSettlement: return "defend settlement";
                case AiBehavior.EngageParty: return "engage party";
                case AiBehavior.EscortParty: return "escort party";
                case AiBehavior.FleeToPoint: return "flee to point";
                case AiBehavior.GoAroundParty: return "go around party";
                case AiBehavior.GoToPoint: return "go to point";
                case AiBehavior.GoToSettlement: return "go to settlement";
                case AiBehavior.Hold: return "hold";
                case AiBehavior.JoinParty: return "join party";
                default: return "other";
            }
        }
        */
    }
}
