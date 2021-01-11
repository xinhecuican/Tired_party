using MCM.Abstractions.Attributes;
using MCM.Abstractions.Attributes.v2;
using MCM.Abstractions.Dropdown;
using MCM.Abstractions.Settings.Base;
using MCM.Abstractions.Settings.Base.Global;
using TaleWorlds.Core;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade;

namespace Tired_party
{
    class mod_setting : AttributeGlobalSettings<mod_setting>
    {
        public override string Id
        {
            get
            {
                return "tired_party";
            }
        }

        public override string DisplayName
        {
            get
            {
                if(BannerlordConfig.Language.Equals("简体中文"))
                    return string.Format("{0}", new TextObject("军队疲乏度", null));
                else
                    return string.Format("{0}", new TextObject("tired party", null));
            }
        }

        public override string FolderName
        {
            get
            {
                return "Tired_party";
            }
        }
        /*
        [SettingPropertyBool("{=tired_party_setting_do_not}禁用mod", RequireRestart = false, IsToggle = false)]
        [SettingPropertyGroup("{=tired_party_setting}设置/{=tired_party_base_setting}基础设置")]
        public bool is_ban { get; set; } = false;

        [SettingPropertyBool("{=tired_party_setting_do_not_army}禁止对集团军产生影响", RequireRestart = false, IsToggle = false)]
        [SettingPropertyGroup("{=tired_party_setting}设置/{=tired_party_base_setting}基础设置")]
        public bool is_ban_army { get; set; } = false;

        [SettingPropertyFloatingInteger("{=tired_party_night_time}晚上一小时恢复比例", 0f, 1f, "0.00", Order = 2, RequireRestart = false, HintText = "{=tired_hint_text_night_time}晚上每小时恢复的数值，默认是0.33")]
        [SettingPropertyGroup("{=tired_party_setting}设置/{=tired_party_sum_setting}数值设置")]
        public float recovery_in_night_time { get; set; } = Party_tired.recovery_in_night_time;

        [SettingPropertyFloatingInteger("{=tired_party_day_time}白天一小时恢复比例", 0f, 1f, "0.00", Order = 1, RequireRestart = false, HintText = "{=tired_hint_text_day_time}白天每小时恢复的数值，默认是0.25")]
        [SettingPropertyGroup("{=tired_party_setting}设置/{=tired_party_sum_setting}数值设置")]
        public float recovery_in_day_time { get; set; } = Party_tired.recovery_in_day_time;

        [SettingPropertyFloatingInteger("{=tired_party_limit_speed}速度", 0.7f, 1f, "0.00", Order = 3, RequireRestart = false, HintText = "{=tired_hint_text_limit_speed}体力耗尽时速度的比例，默认是0.75")]
        [SettingPropertyGroup("{=tired_party_setting}设置/{=tired_party_sum_setting}数值设置")]
        public float limit_speed { get; set; } = 0.75f;

        [SettingPropertyFloatingInteger("{=tired_party_limit_presist}最低坚持时间", 2f, 5f, "0.00", Order = 3, RequireRestart = false, HintText = "{=tired_hint_text_limit_persist}每次不睡觉可以坚持的最少时间，默认是3天")]
        [SettingPropertyGroup("{=tired_party_setting}设置/{=tired_party_sum_setting}数值设置")]
        public float persist_time { get; set; } = 3f;
        */
        [SettingPropertyBool("{=tired_party_setting_do_not}ban mod", RequireRestart = false, IsToggle = false)]
        [SettingPropertyGroup("{=tired_party_setting}setting/{=tired_party_base_setting}basic setting")]
        public bool is_ban { get; set; } = false;

        [SettingPropertyBool("{=tired_party_setting_do_not_army}ban army effect", RequireRestart = false, IsToggle = false)]
        [SettingPropertyGroup("{=tired_party_setting}setting/{=tired_party_base_setting}basic setting")]
        public bool is_ban_army { get; set; } = false;

        [SettingPropertyBool("{=jDamJPB6mC}don't show information left", RequireRestart = true, IsToggle = false)]
        [SettingPropertyGroup("{=tired_party_setting}setting/{=tired_party_base_setting}basic setting")]
        public bool is_ban_information { get; set; } = true;

        [SettingPropertyFloatingInteger("{=tired_party_night_time}recovery rate One hour in the evening", 0f, 1f, "0.00", Order = 2, RequireRestart = false, HintText = "{=tired_hint_text_night_time}Value recovered per hour at night, default is 0.33")]
        [SettingPropertyGroup("{=tired_party_setting}setting/{=tired_party_sum_setting}Numerical settings")]
        public float recovery_in_night_time { get; set; } = Party_tired.recovery_in_night_time;

        [SettingPropertyFloatingInteger("{=tired_party_day_time}recovery rate One hour in the morning", 0f, 1f, "0.00", Order = 1, RequireRestart = false, HintText = "{=tired_hint_text_day_time}Value recovered per hour during the day, default is 0.25")]
        [SettingPropertyGroup("{=tired_party_setting}setting/{=tired_party_sum_setting}Numerical settings")]
        public float recovery_in_day_time { get; set; } = Party_tired.recovery_in_day_time;

        [SettingPropertyFloatingInteger("{=tired_party_day_time_main}main party recovery rate One hour in the morning", 0f, 1f, "0.00", Order = 3, RequireRestart = false, HintText = "{=tired_hint_text_day_time}Value recovered per hour during the day, default is 0.25")]
        [SettingPropertyGroup("{=tired_party_setting}setting/{=tired_party_sum_setting}Numerical settings")]
        public float recovery_in_day_time_main { get; set; } = Party_tired.recovery_in_day_time;

        [SettingPropertyFloatingInteger("{=tired_party_night_time_main}main party mrecovery rate One hour in the evening", 0f, 1f, "0.00", Order = 4, RequireRestart = false, HintText = "{=tired_hint_text_night_time}Value recovered per hour at night, default is 0.33")]
        [SettingPropertyGroup("{=tired_party_setting}setting/{=tired_party_sum_setting}Numerical settings")]
        public float recovery_in_night_time_main { get; set; } = Party_tired.recovery_in_night_time;

        [SettingPropertyFloatingInteger("{=tired_party_limit_speed}speed", 0.7f, 1f, "0.00", Order = 5, RequireRestart = false, HintText = "{=tired_hint_text_limit_speed}Rate of velocity when physical strength is exhausted, default is 0.75")]
        [SettingPropertyGroup("{=tired_party_setting}setting/{=tired_party_sum_setting}Numerical settings")]
        public float limit_speed { get; set; } = 0.75f;

        [SettingPropertyFloatingInteger("{=tired_party_limit_presist}Minimum persistence time", 2f, 5f, "0.00", Order = 6, RequireRestart = false, HintText = "{=tired_hint_text_limit_persist}The minimum time you can hold on to every time you don't sleep, the default is 3 days")]
        [SettingPropertyGroup("{=tired_party_setting}setting/{=tired_party_sum_setting}Numerical settings")]
        public float persist_time { get; set; } = 3f;

        [SettingPropertyFloatingInteger("{=WxqpKxSZ63}morale rate", 0.5f, 2f, "0.00", Order = 6, RequireRestart = false, HintText = "{=tX3M9pnZ4L}an argument when calculate morale")]
        [SettingPropertyGroup("{=tired_party_setting}setting/{=tired_party_sum_setting}Numerical settings")]
        public float morale_reduce { get; set; } = 1f;

        [SettingPropertyBool("{=742k0tuqk7}don't show someone is capture by someone", RequireRestart = false, IsToggle = false)]
        [SettingPropertyGroup("{=tired_party_setting}setting/{=JacQTvCmeo}information setting")]
        public bool is_ban_capture_information { get; set; } = false;

        [SettingPropertyBool("{=nhR9oUfEbO}don't show someone is released by someone", RequireRestart = false, IsToggle = false)]
        [SettingPropertyGroup("{=tired_party_setting}setting/{=JacQTvCmeo}information setting")]
        public bool is_ban_release_information { get; set; } = false;

        [SettingPropertyBool("{=zetGJp2Mc3}don't show someone marry someone", RequireRestart = false, IsToggle = false)]
        [SettingPropertyGroup("{=tired_party_setting}setting/{=JacQTvCmeo}information setting")]
        public bool is_ban_married_information { get; set; } = false;
    }
}
