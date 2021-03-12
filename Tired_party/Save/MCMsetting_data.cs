using MCM.Abstractions.Settings.Base.Global;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.SaveSystem;

namespace Tired_party.Save
{
    class MCMsetting_data
    {
        [SaveableField(1)]
        public bool is_ban;
        [SaveableField(2)]
        public bool is_ban_army;
        [SaveableField(3)]
        public float recovery_in_night_time;
        [SaveableField(4)]
        public float recovery_in_day_time;
        [SaveableField(5)]
        public float recovery_in_day_time_main;
        [SaveableField(6)]
        public float recovery_in_night_time_main;
        [SaveableField(7)]
        public float limit_speed;
        [SaveableField(8)]
        public float persist_time;
        [SaveableField(9)]
        public float morale_reduce;
        [SaveableField(10)]
        public bool is_ban_information;
        [SaveableField(11)]
        public bool is_ban_capture_information;
        [SaveableField(12)]
        public bool is_ban_release_information;
        [SaveableField(13)]
        public bool is_ban_married_information;
        [SaveableField(14)]
        public bool is_ban_simulation_effect;
        [SaveableField(15)]
        public bool is_ban_combat_effect;
        [SaveableField(16)]
        public bool is_ban_debug;
        [SaveableField(17)]
        public float combat_effect_rate;
        [SaveableField(18)]
        public bool use_sneak;
        [SaveableField(19)]
        public bool ban_reinforcement;
        [SaveableField(20)]
        public float time_lapse_ratio;
        [SaveableField(21)]
        public float battle_radius;
        [SaveableField(22)]
        public bool is_ban_time_pass;
        [SaveableField(23)]
        public int reinforcement_mode;

        public void save_data()
        {
            is_ban = GlobalSettings<mod_setting>.Instance.is_ban;
            is_ban_army = GlobalSettings<mod_setting>.Instance.is_ban_army;
            is_ban_capture_information = GlobalSettings<mod_setting>.Instance.is_ban_capture_information;
            is_ban_information = GlobalSettings<mod_setting>.Instance.is_ban_information;
            is_ban_married_information = GlobalSettings<mod_setting>.Instance.is_ban_married_information;
            is_ban_release_information = GlobalSettings<mod_setting>.Instance.is_ban_release_information;
            limit_speed = GlobalSettings<mod_setting>.Instance.limit_speed;
            morale_reduce = GlobalSettings<mod_setting>.Instance.morale_reduce;
            persist_time = GlobalSettings<mod_setting>.Instance.persist_time;
            recovery_in_day_time = GlobalSettings<mod_setting>.Instance.recovery_in_day_time;
            recovery_in_day_time_main = GlobalSettings<mod_setting>.Instance.recovery_in_day_time_main;
            recovery_in_night_time = GlobalSettings<mod_setting>.Instance.recovery_in_night_time;
            recovery_in_night_time_main = GlobalSettings<mod_setting>.Instance.recovery_in_night_time_main;
            is_ban_simulation_effect = GlobalSettings<mod_setting>.Instance.is_ban_simulation_effect;
            is_ban_combat_effect = GlobalSettings<mod_setting>.Instance.is_ban_combat_effect;
            is_ban_debug = GlobalSettings<mod_setting>.Instance.is_ban_debug;
            combat_effect_rate = GlobalSettings<mod_setting>.Instance.combat_effect_rate;
            use_sneak = GlobalSettings<mod_setting>.Instance.use_sneak;
            ban_reinforcement = GlobalSettings<mod_setting>.Instance.ban_reinforcement;
            time_lapse_ratio = GlobalSettings<mod_setting>.Instance.time_lapse_ratio;
            battle_radius = GlobalSettings<mod_setting>.Instance.battle_radius;
            is_ban_time_pass = GlobalSettings<mod_setting>.Instance.is_ban_time_pass;
            reinforcement_mode = GlobalSettings<mod_setting>.Instance.reinforcement_mode;
        }

        public void load_data()
        {
            GlobalSettings<mod_setting>.Instance.is_ban = is_ban;
            GlobalSettings<mod_setting>.Instance.is_ban_army = is_ban_army;
            GlobalSettings<mod_setting>.Instance.is_ban_capture_information = is_ban_capture_information;
            GlobalSettings<mod_setting>.Instance.is_ban_information = is_ban_information;
            GlobalSettings<mod_setting>.Instance.is_ban_married_information = is_ban_married_information;
            GlobalSettings<mod_setting>.Instance.is_ban_release_information = is_ban_release_information;
            GlobalSettings<mod_setting>.Instance.limit_speed = limit_speed;
            GlobalSettings<mod_setting>.Instance.morale_reduce = morale_reduce;
            GlobalSettings<mod_setting>.Instance.persist_time = persist_time;
            GlobalSettings<mod_setting>.Instance.recovery_in_day_time = recovery_in_day_time;
            GlobalSettings<mod_setting>.Instance.recovery_in_day_time_main = recovery_in_day_time_main;
            GlobalSettings<mod_setting>.Instance.recovery_in_night_time = recovery_in_night_time;
            GlobalSettings<mod_setting>.Instance.recovery_in_night_time_main = recovery_in_night_time_main;
            GlobalSettings<mod_setting>.Instance.is_ban_simulation_effect = is_ban_simulation_effect;
            GlobalSettings<mod_setting>.Instance.is_ban_combat_effect = is_ban_combat_effect;
            GlobalSettings<mod_setting>.Instance.is_ban_debug = is_ban_debug;
            GlobalSettings<mod_setting>.Instance.combat_effect_rate = combat_effect_rate;
            GlobalSettings<mod_setting>.Instance.use_sneak = use_sneak;
            GlobalSettings<mod_setting>.Instance.ban_reinforcement = ban_reinforcement;
            GlobalSettings<mod_setting>.Instance.time_lapse_ratio = time_lapse_ratio;
            GlobalSettings<mod_setting>.Instance.battle_radius = battle_radius;
            GlobalSettings<mod_setting>.Instance.is_ban_time_pass = is_ban_time_pass;
            GlobalSettings<mod_setting>.Instance.reinforcement_mode = reinforcement_mode;
        }
    }
}
