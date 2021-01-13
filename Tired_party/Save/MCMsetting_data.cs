﻿using System;
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

    }
}
