using HarmonyLib;
using MCM.Abstractions.Settings.Base.Global;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Instrumentation;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.SandBox.GameComponents.Map;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Engine.Options;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.ViewModelCollection.HUD.KillFeed.Personal;
using TaleWorlds.MountAndBlade.ViewModelCollection.Multiplayer.KillFeed.Personal;
using Tired_party.Helper;
using Tired_party.Patch;
using Tired_party.sneak_attack;

namespace Tired_party.Mission_time
{
    class agent_spawn_controller : MissionLogic
    {
        private MapEvent mapEvent;
        private MissionAgentSpawnLogic missionAgentSpawnLogic;
        public MissionTimer check_time = null;
        public MissionTimer atmosphere_change_check;
        public int defend_initial_num;
        public int attacker_initial_num;
        public List<arrive_time_data> ready_to_place;
        private TextObject text1 = new TextObject("{=m3sLthymyB}is going to arrive in half an hour", null);
        private TextObject text2 = new TextObject("{=1yoGH2Ud8A}from )", null);
        public int tick_times;

        private float season_factor;
        public override void OnBehaviourInitialize()
        {
            missionAgentSpawnLogic = base.Mission.GetMissionBehaviour<MissionAgentSpawnLogic>();
            mapEvent = MapEvent.PlayerMapEvent;
            ready_to_place = new List<arrive_time_data>();
        }
        public override void AfterStart()
        {
            base.AfterStart();
            missionAgentSpawnLogic.SetSpawnHorses(TaleWorlds.Core.BattleSideEnum.Attacker, mapEvent.IsSiegeAssault);
            missionAgentSpawnLogic.SetSpawnHorses(TaleWorlds.Core.BattleSideEnum.Defender, mapEvent.IsSiegeAssault);
            missionAgentSpawnLogic.InitWithSinglePhase(missiontime_data.current.initial_num[0], missiontime_data.current.initial_num[1], 
                missiontime_data.current.initial_num[0], missiontime_data.current.initial_num[1], true, true, 1);
            int num = MBMath.Floor((float)this.mapEvent.GetNumberOfInvolvedMen(BattleSideEnum.Defender));
            int num2 = MBMath.Floor((float)this.mapEvent.GetNumberOfInvolvedMen(BattleSideEnum.Attacker));
            int battle_size = (int)AccessTools.Field(typeof(MissionAgentSpawnLogic), "_battleSize").GetValue(missionAgentSpawnLogic);
            defend_initial_num = MBMath.Ceiling(battle_size * num / (num + num2));
            attacker_initial_num = battle_size - defend_initial_num;
            check_time = new MissionTimer(GlobalSettings<mod_setting>.Instance.time_lapse_ratio / 2);
            //atmosphere_change_check = new MissionTimer(GlobalSettings<mod_setting>.Instance.time_lapse_ratio / 6);
            //tick_times = 0;
            //season_factor = Campaign.Current.Models.MapWeatherModel.GetSeasonTimeFactor();
        }

        public override void OnPreMissionTick(float dt)
        {
            try
            {
                base.OnPreMissionTick(dt);
                Type t = typeof(MissionAgentSpawnLogic).Assembly.
                        GetType("TaleWorlds.MountAndBlade.MissionAgentSpawnLogic+SpawnPhase");
                /*if(atmosphere_change_check != null && atmosphere_change_check.Check(true))
                {
                    tick_times++;
                    float hour_normalized = (float)((float)(CampaignTime.Now.ToHours + tick_times/6f) % 24.0) / 24f;
                    bool sun_is_moon = hour_normalized >= 0.0833333358f && hour_normalized < 0.9166667f ? false : true;
                    float altitude ;
                    float angle;
                    GetSunPosition(hour_normalized, season_factor, out altitude, out angle);
                    float environment_factor = GetEnvironmentMultiplier(sun_is_moon, altitude, angle, season_factor);
                    float modified_environment = GetModifiedEnvironmentMultiplier(sun_is_moon, environment_factor);
                    Vec3 sun_color = GetSunColor(sun_is_moon, environment_factor);//绿，红，蓝
                    float sun_size = 0.1f + (1f - environment_factor) / 8f;
                    float brightness = GetSunBrightness(sun_is_moon, environment_factor);
                    float sky_brightness = GetSkyBrightness(sun_is_moon, hour_normalized, environment_factor);
                    //Mission.Current.Scene.SetSunAngleAltitude(angle, altitude);
                    //Mission.Current.Scene.SetSunSize(sun_size);
                    InformationManager.DisplayMessage(new InformationMessage(hour_normalized.ToString()));
                    //Mission.Current.Scene.SetSun(ref sun_color, altitude, angle, brightness * 10f);
                    
                    //Mission.Current.Scene.SetSkyBrightness(sky_brightness );
                    //Mission.Current.Scene.SetEnvironmentMultiplier(true, modified_environment);
                }*/
                if (check_time != null && check_time.Check(true))
                {
                    missiontime_data.pass_time += 0.5f;
                    for (int i = 0; i < 2; i++)
                    {
                        for (int k = 0; k < missiontime_data.current.time_and_direction[i].Count; k++)
                        {
                            missiontime_data.current.time_and_direction[i][k].arrive_time -= 0.5f;
                            if(missiontime_data.current.time_and_direction[i][k].arrive_time <= 0.5f && missiontime_data.current.time_and_direction[i][k].arrive_time > 0)
                            {
                                if (i != (int)MapEvent.PlayerMapEvent.PlayerSide && Nearlycome_notification.notification_list != null)
                                {
                                    SPPersonalKillNotificationItemVM message = new SPPersonalKillNotificationItemVM("",RemoveItem);
                                    message.Message = missiontime_data.current.time_and_direction[i][k].party.Party.MobileParty.Name.ToString() +
                                         text1.ToString() + "(" + text2 + get_angle_text(missiontime_data.current.time_and_direction[i][k]) + ")";
                                    message.ItemType = 1;
                                    Nearlycome_notification.notification_list.Add(message);
                                    //message_helper.TechnicalMessage(missiontime_data.current.time_and_direction[i][k].party.Party.MobileParty.Name.ToString() +
                                         //text1.ToString() + "(" + text2 + get_angle_text(missiontime_data.current.time_and_direction[i][k]) + ")");
                                }
                                else if(Nearlycome_notification.notification_list != null)
                                {
                                    SPPersonalKillNotificationItemVM message = new SPPersonalKillNotificationItemVM("", RemoveItem);
                                    message.Message = missiontime_data.current.time_and_direction[i][k].party.Party.MobileParty.Name.ToString() +
                                         text1.ToString() + "(" + text2 + get_angle_text(missiontime_data.current.time_and_direction[i][k]) + ")";
                                    message.ItemType = 4;
                                    Nearlycome_notification.notification_list.Add(message);
                                    //message_helper.FriendlyMessage(missiontime_data.current.time_and_direction[i][k].party.Party.MobileParty.Name.ToString() +
                                         //text1.ToString() + "(" + text2 + get_angle_text(missiontime_data.current.time_and_direction[i][k]) + ")");
                                }
                            }
                            if (missiontime_data.current.time_and_direction[i][k].arrive_time <= 0)
                            {
                                if (!Mission.Current.IsMissionEnding)
                                {
                                    missiontime_data.current.time_and_direction[i][k].arrive_time = 10000f;
                                    if (i == 0)
                                    {
                                        object o = AccessTools.Property(typeof(MissionAgentSpawnLogic), "DefenderActivePhase").GetValue(missionAgentSpawnLogic);
                                        int add_num = missiontime_data.current.time_and_direction[i][k].party.Party.NumberOfHealthyMembers;
                                        int total_spawn = (int)AccessTools.Field(t, "TotalSpawnNumber").GetValue(o);
                                        total_spawn += add_num;
                                        AccessTools.Field(t, "TotalSpawnNumber").SetValue(o, total_spawn);
                                        int remain_number = (int)AccessTools.Field(t, "RemainingSpawnNumber").GetValue(o);
                                        remain_number += add_num;
                                        AccessTools.Field(t, "RemainingSpawnNumber").SetValue(o, remain_number);
                                        int initial_num = (int)AccessTools.Field(t, "InitialSpawnedNumber").GetValue(o);
                                        if (initial_num < defend_initial_num)
                                        {
                                            if (initial_num + add_num > defend_initial_num)
                                            {
                                                initial_num = defend_initial_num;
                                            }
                                            else
                                            {
                                                initial_num += add_num;
                                            }
                                            AccessTools.Field(t, "InitialSpawnedNumber").SetValue(o, initial_num);
                                        }
                                    }
                                    else
                                    {
                                        object o = AccessTools.Property(typeof(MissionAgentSpawnLogic), "AttackerActivePhase").GetValue(missionAgentSpawnLogic);
                                        int add_num = missiontime_data.current.time_and_direction[i][k].party.Party.NumberOfHealthyMembers;
                                        int total_spawn = (int)AccessTools.Field(t, "TotalSpawnNumber").GetValue(o);
                                        total_spawn += add_num;
                                        AccessTools.Field(t, "TotalSpawnNumber").SetValue(o, total_spawn);
                                        int remain_number = (int)AccessTools.Field(t, "RemainingSpawnNumber").GetValue(o);
                                        remain_number += add_num;
                                        AccessTools.Field(t, "RemainingSpawnNumber").SetValue(o, remain_number);
                                        int initial_num = (int)AccessTools.Field(t, "InitialSpawnedNumber").GetValue(o);
                                        if (initial_num < attacker_initial_num)
                                        {
                                            if (initial_num + add_num > attacker_initial_num)
                                            {
                                                initial_num = attacker_initial_num;
                                            }
                                            else
                                            {
                                                initial_num += add_num;
                                            }
                                            AccessTools.Field(t, "InitialSpawnedNumber").SetValue(o, initial_num);
                                        }
                                    }
                                    InformationManager.AddQuickInformation(i == (int)mapEvent.PlayerSide ? GameTexts.FindText("str_new_reinforcements_have_arrived_for_ally_side", null) : GameTexts.FindText("str_new_reinforcements_have_arrived_for_enemy_side", null), 0, null, "");
                                    MatrixFrame cameraFrame = Mission.Current.GetCameraFrame();
                                    Vec3 position = cameraFrame.origin + cameraFrame.rotation.u;
                                    MBSoundEvent.PlaySound(i == (int)mapEvent.PlayerSide ? SoundEvent.GetEventIdFromString("event:/alerts/report/reinforcements_ally") : SoundEvent.GetEventIdFromString("event:/alerts/report/reinforcements_enemy"), position);
                                }
                                ready_to_place.Add(missiontime_data.current.time_and_direction[i][k]);
                            }
                        }
                    }
                }
            }
            catch(Exception e)
            {
                MethodInfo methodInfo = MethodBase.GetCurrentMethod() as MethodInfo;
                debug_helper.HandleException(e, methodInfo, "submodule load error");
            }
        }

        private void RemoveItem(SPPersonalKillNotificationItemVM item)
        {
            if(Nearlycome_notification.notification_list != null)
                Nearlycome_notification.notification_list.Remove(item);
        }

        private string get_angle_text(arrive_time_data data)
        {
            Vec2 main_party_direction = MapEvent.PlayerMapEvent.Position - Campaign.Current.MainParty.Position2D;
            float angle = main_party_direction.AngleBetween(data.enter_direction);
            TextObject text;
            if(angle >= Math.PI/2)
            {
                text = new TextObject("{=Feh3l6YBDL}Northeastern");
            }
            else if(angle >= 0)
            {
                text = new TextObject("{=C2wPNpo9Oz}Southeastern");
            }
            else if(angle >= - Math.PI / 2)
            {
                text = new TextObject("{=UdjhlmfgnH}Southwestern");
            }
            else
            {
                text = new TextObject("{=Umw5lAzt9c}Northwestern");
            }
            return text.ToString();
        }

        private float GetEnvironmentMultiplier(bool _sunIsMoon, float altitude, float angle, float seasonFactor)
        {
            float num;
            if (_sunIsMoon)
            {
                num = altitude / 180f * 2f;
            }
            else
            {
                num = altitude / 180f * 2f;
            }
            num = ((num > 1f) ? (2f - num) : num);
            num = (float)Math.Pow((double)num, 0.5);
            float num2 = 1f - 0.0111111114f * angle;
            return MBMath.ClampFloat(Math.Min((float)Math.Sin(Math.Pow((double)MBMath.ClampFloat(num * num2, 0f, 1f), 2.0)) * 2f, 1f), 0f, 1f) * 0.999f + 0.001f;
        }

        private Vec3 GetSunColor(bool _sunIsMoon, float environmentMultiplier)
        {
            Vec3 vec;
            if (!_sunIsMoon)
            {
                vec = new Vec3(1f, 1f - (1f - (float)Math.Pow((double)environmentMultiplier, 0.30000001192092896)) / 2f, 0.9f - (1f - (float)Math.Pow((double)environmentMultiplier, 0.30000001192092896)) / 2.5f, -1f);
            }
            else
            {
                vec = new Vec3(0.85f - (float)Math.Pow((double)environmentMultiplier, 0.40000000596046448), 0.8f - (float)Math.Pow((double)environmentMultiplier, 0.5), 0.8f - (float)Math.Pow((double)environmentMultiplier, 0.800000011920929), -1f);
                vec = Vec3.Vec3Max(vec, new Vec3(0.05f, 0.05f, 0.1f, -1f));
            }
            return vec;
        }

        private float GetSunBrightness(bool _sunIsMoon, float environmentMultiplier, bool forceDay = false)
        {
            float num;
            if (!_sunIsMoon || forceDay)
            {
                num = (float)Math.Sin(Math.Pow((double)((environmentMultiplier - 0.001f) / 0.999f), 1.2000000476837158) * 1.5707963267948966) * 85f;
            }
            else
            {
                num = 0.2f;
            }
            return num;
        }

        private float GetSkyBrightness(bool _sunIsMoon, float hourNorm, float envMultiplier)
        {
            float num = (envMultiplier - 0.001f) / 0.999f;
            float num2;
            if (!_sunIsMoon)
            {
                num2 = (float)Math.Sin(Math.Pow((double)num, 1.2999999523162842) * 1.5707963267948966) * 80f;
                num2 -= 1f;
                //num2 = Math.Min(Math.Max(num2, 0.055f), 25f);
            }
            else
            {
                num2 = 0.055f;
            }
            return num2;
        }

        private float GetModifiedEnvironmentMultiplier(bool _sunIsMoon, float envMultiplier)
        {
            float num;
            if (!_sunIsMoon)
            {
                num = (envMultiplier - 0.001f) / 0.999f;
                num = num * 0.999f + 0.001f;
            }
            else
            {
                num = (envMultiplier - 0.001f) / 0.999f;
                num = num * 0f + 0.001f;
            }
            num = Math.Max((float)Math.Pow((double)num, 1.5), 0.001f);
            num = Math.Max(num * 0.5f, 0.001f);
            return num;
        }

        private void GetSunPosition(float hourNorm, float seasonFactor, out float altitude, out float angle)
        {
            if (hourNorm >= 0.0833333358f && hourNorm < 0.9166667f)
            {
                float amount = (hourNorm - 0.0833333358f) / 0.8333334f;
                altitude = MBMath.Lerp(0f, 180f, amount, 1E-05f);
                angle = 50f * seasonFactor;
            }
            else
            {
                if (hourNorm >= 0.9166667f)
                {
                    hourNorm -= 1f;
                }
                float num = (hourNorm - -0.08333331f) / 0.166666657f;
                num = ((num < 0f) ? 0f : ((num > 1f) ? 1f : num));
                altitude = MBMath.Lerp(180f, 0f, num, 1E-05f);
                angle = 50f * seasonFactor;
            }
        }
    }
}
