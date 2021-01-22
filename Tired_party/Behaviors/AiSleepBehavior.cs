using MCM.Abstractions.Settings.Base.Global;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.Library;
using Tired_party.Helper;
using SandBox.View.Map;
using TaleWorlds.Engine;

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

        public void PartyHourlyTick(MobileParty mobileParty)
        {
            /*
             * 目标：让ai会睡觉（晚上更为倾向）
             * 追逐敌军时尽量不让他睡觉
             * 没事时保持充足体力
             * 智力足够时不会一直逃跑
             */

            try
            {
                if (Party_tired.Current == null || !Party_tired.Current.Party_tired_rate.ContainsKey(mobileParty) || GlobalSettings<mod_setting>.Instance.is_ban)
                {
                    return;
                }
                if (mobileParty.Army != null && mobileParty.Army.LeaderParty != mobileParty)
                {
                    foreach (MobileParty party in mobileParty.Army.LeaderParty.AttachedParties)
                    {
                        if (party == mobileParty)
                        {
                            return;
                        }
                    }
                }

                bool flag_is_day_time = CampaignTime.Now.IsDayTime;

                if (mobileParty.Army != null && mobileParty.Army.LeaderParty == mobileParty && !mobileParty.IsMainParty && mobileParty.LeaderHero != null
                    && !GlobalSettings<mod_setting>.Instance.is_ban_army)//军队sleep ai
                {
                    if (Party_tired.Current.Party_tired_rate[mobileParty].need_reset_army)
                    {
                       /* mobileParty.Army.AIBehavior = Party_tired.Current.Party_tired_rate[mobileParty].army_ai_behavior_flags;
                        mobileParty.Army.AiBehaviorObject = Party_tired.Current.Party_tired_rate[mobileParty].army_ai_behavior_object;*/
                        Party_tired.Current.Party_tired_rate[mobileParty].need_reset_army = false;
                        recover_party_state(mobileParty);
                        foreach (MobileParty party in mobileParty.Army.LeaderPartyAndAttachedParties)
                        {
                            Party_tired.ToggleTent(party.Party, false);
                        }
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

                    if (army_now_tired <= 0)
                    {
                        Party_tired.Current.Party_tired_rate[mobileParty.Army.LeaderParty].reset_time++;
                        if (Party_tired.Current.Party_tired_rate[mobileParty.Army.LeaderParty].reset_time > 24)
                        {
                            Party_tired.Current.Party_tired_rate[mobileParty.Army.LeaderParty].reset_time = 0;
                            Party_tired.Current.Party_tired_rate[mobileParty].need_reset_army = true;
                            /*Party_tired.Current.Party_tired_rate[mobileParty].army_ai_behavior_flags = mobileParty.Army.AIBehavior;
                            Party_tired.Current.Party_tired_rate[mobileParty].army_ai_behavior_object = mobileParty.Army.AiBehaviorObject;
                            mobileParty.Army.AiBehaviorObject = mobileParty;

                            mobileParty.Army.AIBehavior = Army.AIBehaviorFlags.NumberOfAIBehaviorFlags;*/
                            store_party_state(mobileParty);
                            mobileParty.SetMoveGoToPoint(mobileParty.Position2D);
                            Party_tired.ToggleTent(mobileParty.Party, true);
                            foreach (MobileParty party in mobileParty.AttachedParties)
                            {
                                Party_tired.ToggleTent(party.Party, true);
                            }
                            return;
                        }
                    }
                    else
                    {
                        Party_tired.Current.Party_tired_rate[mobileParty].reset_time = 0;
                    }
                    if (mobileParty.Army.AIBehavior == Army.AIBehaviorFlags.AssaultingTown || mobileParty.Army.AIBehavior == Army.AIBehaviorFlags.Besieging
                         || mobileParty.Army.AIBehavior == Army.AIBehaviorFlags.Gathering
                        || mobileParty.Army.AIBehavior == Army.AIBehaviorFlags.Raiding)
                    {
                        return;
                    }

                    float ans_army_tired = Party_tired.begin_to_decrease - army_now_tired > 0 ? (float)Math.Sqrt(Party_tired.begin_to_decrease - army_now_tired) * 5 / 3f + 0.49f : (float)Math.Pow(1 - army_now_tired, 2);
                    bool flag_army_go_to_someplace_behavior = mobileParty.Army.AIBehavior == Army.AIBehaviorFlags.GoToSettlement || mobileParty.Army.AIBehavior == Army.AIBehaviorFlags.Defending;
                    bool flag_army_not_busy = mobileParty.Army.AIBehavior == Army.AIBehaviorFlags.Patrolling;
                    float cohesion_remain_time = (mobileParty.Army.Cohesion - mobileParty.Army.CohesionThresholdForDispersion) / mobileParty.Army.CohesionChange;
                    float ans_army_go_to_someplace = 1f;
                    if (flag_army_go_to_someplace_behavior || flag_army_not_busy)
                    {
                        if (army_now_tired - 0.5f < 0)
                        {
                            ans_army_go_to_someplace = 1.4f * (0.8f + 0.2f * mobileParty.Position2D.DistanceSquared(mobileParty.TargetPosition) / (Campaign.MapDiagonal * Campaign.MapDiagonal));
                        }
                    }
                    float ans_army_time = flag_is_day_time ? 0.8f : 1.2f;
                    float ans_army_cohesion = cohesion_remain_time - 2.0f < 0 ? 0.8f : 0.8f + 0.2f * (cohesion_remain_time - 2.0f) / cohesion_remain_time;       //剩余时间越多，就越不着急，可以休息
                    float ans_army = ans_army_cohesion * ans_army_go_to_someplace * ans_army_tired * ans_army_time;
                    if (mobileParty.Army.LeaderParty == Party_tired.test_party)
                    {
                        InformationManager.DisplayMessage(new InformationMessage(ans_army.ToString()));
                    }
                    if (ans_army > 0.7f)
                    {

                        /*Party_tired.Current.Party_tired_rate[mobileParty].army_ai_behavior_flags = mobileParty.Army.AIBehavior;
                        Party_tired.Current.Party_tired_rate[mobileParty].army_ai_behavior_object = mobileParty.Army.AiBehaviorObject;
                        
                        mobileParty.Army.AIBehavior = Army.AIBehaviorFlags.NumberOfAIBehaviorFlags;
                        mobileParty.Army.AiBehaviorObject = mobileParty;*/
                        Party_tired.Current.Party_tired_rate[mobileParty].need_reset_army = true;
                        store_party_state(mobileParty);
                        mobileParty.SetMoveGoToPoint(mobileParty.Position2D);
                        Party_tired.ToggleTent(mobileParty.Party, true);

                        foreach (MobileParty party in mobileParty.AttachedParties)
                        {
                            Party_tired.ToggleTent(party.Party, true);
                        }

                    }
                    return;
                }


                /* party ai */
                if (Party_tired.Current.Party_tired_rate[mobileParty].reset_time > 0)
                {
                    if(Party_tired.Current.Party_tired_rate[mobileParty].is_busy && Party_tired.Current.Party_tired_rate[mobileParty].reset_time > 1)
                    {
                        Party_tired.ToggleTent(mobileParty.Party, true);
                        Party_tired.Current.Party_tired_rate[mobileParty].is_busy = false;
                    }
                    Party_tired.Current.Party_tired_rate[mobileParty].reset_time--;
                    if(Party_tired.Current.Party_tired_rate[mobileParty].reset_time == 1)
                    {
                        Party_tired.Current.Party_tired_rate[mobileParty].is_busy = true;
                    }
                    if (Party_tired.Current.Party_tired_rate[mobileParty].reset_time == 0)
                    {
                        if(mobileParty.IsMainParty)
                        {
                            Campaign.Current.SetTimeControlModeLock(false);
                            Campaign.Current.TimeControlMode = CampaignTimeControlMode.Stop;
                        }
                        Party_tired.Current.Party_tired_rate[mobileParty].is_fleeing = false;
                        Party_tired.Current.Party_tired_rate[mobileParty].is_busy = false;
                        Party_tired.ToggleTent(mobileParty.Party, false);
                    }
                    else
                    {
                        return;
                    }
                }
                if (Party_tired.Current.Party_tired_rate[mobileParty].Now <= 1e-8)
                {
                    Party_tired.Current.Party_tired_rate[mobileParty].Limit++;

                    if (Party_tired.Current.Party_tired_rate[mobileParty].Limit >= 24 * (1 + (mobileParty.LeaderHero != null ? Math.Pow(mobileParty.LeaderHero.GetSkillValue(DefaultSkills.Charm) / 300f, 1.2) : 0))
                        && mobileParty != Campaign.Current.MainParty)
                    {
                        mobileParty.Ai.DisableForHours(3);
                        Party_tired.Current.Party_tired_rate[mobileParty].reset_time += 3;
                        Party_tired.ToggleTent(mobileParty.Party, true);
                        Party_tired.Current.Party_tired_rate[mobileParty].Limit = 0;
                        return;
                    }
                    else if (Party_tired.Current.Party_tired_rate[mobileParty].Limit >= 24 * (1 + (mobileParty.LeaderHero != null ? Math.Pow(mobileParty.LeaderHero.GetSkillValue(DefaultSkills.Charm) / 300f, 1.2) : 0)))
                    {
                        mobileParty.SetMoveModeHold();
                        Party_tired.Current.Party_tired_rate[mobileParty].reset_time += 3;
                        Party_tired.ToggleTent(mobileParty.Party, true);
                        Campaign.Current.SetTimeControlModeLock(false);
                        Campaign.Current.TimeControlMode = CampaignTimeControlMode.UnstoppableFastForward;
                        Campaign.Current.SetTimeControlModeLock(true);
                        Party_tired.Current.Party_tired_rate[mobileParty].Limit = 0;
                        return;
                    }
                }
                else
                {
                    Party_tired.Current.Party_tired_rate[mobileParty].Limit = 0;
                }

                if (
                    mobileParty.DefaultBehavior == AiBehavior.DefendSettlement || mobileParty.DefaultBehavior == AiBehavior.AssaultSettlement
                    || mobileParty.DefaultBehavior == AiBehavior.BesiegeSettlement)
                {
                    return;
                }

                if (Party_tired.Current.Party_tired_rate[mobileParty].need_recovery && mobileParty != Campaign.Current.MainParty)
                {
                    recover_party_state(mobileParty);
                    Party_tired.Current.Party_tired_rate[mobileParty].need_recovery = false;
                }
                float now_tired = Party_tired.Current.Party_tired_rate[mobileParty].Now;
                float ans_tired = Party_tired.begin_to_decrease - now_tired > 0 ? (float)Math.Sqrt(Party_tired.begin_to_decrease - now_tired)  * 5 / 3f + 0.49f : (float)Math.Pow(1 - now_tired, 2);
                bool flag_follow_behavior = (mobileParty.DefaultBehavior == AiBehavior.GoAroundParty && mobileParty.TargetParty != null) || mobileParty.DefaultBehavior == AiBehavior.EscortParty;
                bool flag_engage_behavior = mobileParty.ShortTermBehavior == AiBehavior.EngageParty;
                bool flag_go_to_someplace_behavior = mobileParty.DefaultBehavior == AiBehavior.GoToPoint || mobileParty.ShortTermBehavior == AiBehavior.GoToSettlement;
                bool flag_flee_to_point_behavior = mobileParty.ShortTermBehavior == AiBehavior.FleeToPoint;

                float ans_engage_behavior = 1f;
                if (flag_engage_behavior && mobileParty.AiBehaviorObject.IsMobile)
                {
                    float party_speed = mobileParty.ComputeSpeed();
                    float enemy_speed = mobileParty.AiBehaviorObject.MobileParty.ComputeSpeed();
                    float double_speed = (float)Math.Pow(enemy_speed / party_speed, 4);
                    ans_engage_behavior = (float)(party_speed < enemy_speed ? 1.1f : 0.8f * double_speed);

                    if (Party_tired.Current.Party_tired_rate.ContainsKey(mobileParty) && Party_tired.Current.Party_tired_rate.ContainsKey(mobileParty.AiBehaviorObject.MobileParty))
                    {
                        float party_remain_hour = Calculate_party_tired.calculate_remaining_hours(Party_tired.Current.Party_tired_rate[mobileParty]);
                        float enemy_remain_hour = Calculate_party_tired.calculate_remaining_hours(Party_tired.Current.Party_tired_rate[mobileParty.AiBehaviorObject.MobileParty]);
                        ans_engage_behavior *= party_remain_hour > enemy_remain_hour ? enemy_remain_hour / party_remain_hour : 1f;
                    }
                }

                float ans_follow_behavior = 1f;
                if (flag_follow_behavior)
                {
                    ans_follow_behavior = ans_tired > Party_tired.begin_to_decrease ? 0.8f + 0.2f * (float)Math.Sqrt(Math.Abs(mobileParty.Position2D.DistanceSquared(mobileParty.TargetParty.Position2D) / (Campaign.MapDiagonal * Campaign.MapDiagonal))) : 1f;
                }

                float ans_go_to_someplace_behavior = 1f;
                if (flag_go_to_someplace_behavior && mobileParty.TargetPosition != null)
                {
                    if (now_tired < 0.6f)
                    {
                        ans_go_to_someplace_behavior = 1.4f * (0.8f + 0.2f * (float)Math.Sqrt(Math.Abs(mobileParty.Position2D.DistanceSquared(mobileParty.TargetPosition) / (Campaign.MapDiagonal * Campaign.MapDiagonal))));
                    }
                }

                float ans_time = 1.2f;
                if (flag_is_day_time)
                {
                    ans_time = 0.8f;
                }

                float ans_flee_behavior = 1f;
                if (flag_flee_to_point_behavior)
                {
                    float party_speed = mobileParty.ComputeSpeed();
                    float enemy_speed = mobileParty.AiBehaviorObject.MobileParty.ComputeSpeed();
                    ans_flee_behavior = party_speed > enemy_speed || mobileParty.IsDisorganized || mobileParty.IsBandit ? 0f :
                        enemy_speed / party_speed * (0.7f + 0.3f * (mobileParty.LeaderHero != null ? mobileParty.LeaderHero.GetSkillValue(DefaultSkills.Tactics) / 150f : 0f));
                }

                float ans = ans_time * ans_tired * ans_follow_behavior * ans_engage_behavior * ans_go_to_someplace_behavior * ans_flee_behavior;
                if (mobileParty == Party_tired.test_party)
                {
                    InformationManager.DisplayMessage(new InformationMessage(ans.ToString()));
                }

                if (ans > 0.7f && mobileParty != Campaign.Current.MainParty)
                {
                    store_party_state(mobileParty);
                    Party_tired.Current.Party_tired_rate[mobileParty].need_recovery = true;
                    if(flag_flee_to_point_behavior)
                    {
                        Party_tired.Current.Party_tired_rate[mobileParty].is_fleeing = true;
                    }
                    if (flag_go_to_someplace_behavior)
                    {
                        Party_tired.Current.Party_tired_rate[mobileParty].reset_time += sleep_hours(mobileParty);
                    }
                    else
                    {
                        Party_tired.Current.Party_tired_rate[mobileParty].reset_time += 3;
                    }
                    Party_tired.Current.Party_tired_rate[mobileParty].is_busy = true;
                    Party_tired.ToggleTent(mobileParty.Party, true);
                    mobileParty.SetMoveModeHold();
                }

            }
            catch (Exception e)
            {
                MethodInfo methodInfo = MethodBase.GetCurrentMethod() as MethodInfo;
                debug_helper.HandleException(e, methodInfo, "ai hourly tick");
            }
        }

        private void store_party_state(MobileParty mobileParty)
        {
            Party_tired.Current.Party_tired_rate[mobileParty].AiBehavior = mobileParty.DefaultBehavior;
            Party_tired.Current.Party_tired_rate[mobileParty].ai_behavior_target = mobileParty.TargetPosition;
            Party_tired.Current.Party_tired_rate[mobileParty].ai_behavior_object = mobileParty.TargetSettlement;
            Party_tired.Current.Party_tired_rate[mobileParty].target_party = mobileParty.TargetParty;
        }

        private void recover_party_state(MobileParty mobileParty)
        {
            switch (Party_tired.Current.Party_tired_rate[mobileParty].AiBehavior)
            {
                case AiBehavior.EngageParty: mobileParty.SetMoveEngageParty(Party_tired.Current.Party_tired_rate[mobileParty].target_party); break;
                case AiBehavior.EscortParty: mobileParty.SetMoveEscortParty(Party_tired.Current.Party_tired_rate[mobileParty].target_party); break;
                case AiBehavior.GoAroundParty: mobileParty.SetMoveGoAroundParty(Party_tired.Current.Party_tired_rate[mobileParty].target_party); break;
                case AiBehavior.GoToPoint: mobileParty.SetMoveGoToPoint(Party_tired.Current.Party_tired_rate[mobileParty].ai_behavior_target); break;
                case AiBehavior.GoToSettlement: mobileParty.SetMoveGoToSettlement(Party_tired.Current.Party_tired_rate[mobileParty].ai_behavior_object); break;
                case AiBehavior.RaidSettlement: mobileParty.SetMoveRaidSettlement(Party_tired.Current.Party_tired_rate[mobileParty].ai_behavior_object); break;
                case AiBehavior.DefendSettlement: mobileParty.SetMoveDefendSettlement(Party_tired.Current.Party_tired_rate[mobileParty].ai_behavior_object); break;
                case AiBehavior.BesiegeSettlement: mobileParty.SetMoveBesiegeSettlement(Party_tired.Current.Party_tired_rate[mobileParty].ai_behavior_object); break;
            }

        }

        private int sleep_hours(MobileParty mobile, float recover_to = 0.8f)
        {
            float temp = Party_tired.Current.Party_tired_rate[mobile].Now;
            int ans = 0;
            bool flag_is_day_time = CampaignTime.Now.IsDayTime;
            if (!flag_is_day_time)
            {
                while (CampaignTime.HoursFromNow(ans).IsNightTime && temp < recover_to)
                {
                    temp += GlobalSettings<mod_setting>.Instance.recovery_in_night_time;
                    ans++;
                }
            }
            else
            {
                while (temp < recover_to && CampaignTime.HoursFromNow(ans).IsDayTime)
                {
                    temp += GlobalSettings<mod_setting>.Instance.recovery_in_day_time;
                    ans++;
                }
            }
            ans += 2;
            return ans;
        }


    }
}
