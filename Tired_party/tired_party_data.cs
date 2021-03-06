﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Library;
using TaleWorlds.SaveSystem;

namespace Tired_party
{
    [SaveableClass(4231681)]
    public class tired_party_data
    {
        [SaveableField(1)]
        private float _reduce_rate;
        [SaveableField(2)]
        private float _now_tired_sum;
        [SaveableField(3)]
        private int _number;
        [SaveableField(4)]
        public AiBehavior AiBehavior;
        [SaveableField(5)]
        public Settlement ai_behavior_object;
        [SaveableField(6)]
        public Vec2 ai_behavior_target;
        [SaveableField(7)]
        public bool need_recovery;
        [SaveableField(8)]
        public MobileParty target_party;
        [SaveableField(9)]
        private int _limit;
        [SaveableField(10)]
        private float morale_change;
        [SaveableField(11)]
        public IMapPoint army_ai_behavior_object;
        [SaveableField(12)]
        public Army.AIBehaviorFlags army_ai_behavior_flags;
        [SaveableField(13)]
        public bool need_reset_army = false;
        [SaveableField(14)]
        public int reset_time;
        [SaveableField(15)]
        public bool is_busy;
        [SaveableField(16)]
        public bool is_fleeing;
        [SaveableField(17)]
        public int discover_state = 0; //0 - 默认， 1 - 发现， 2-未发现

        public int Limit { get { return _limit; } set{ _limit = value; } }
        public int Number { get { return _number; } set { if (value < 0) { _number = 0; } else { _number = value; } } }
        public float Reduce_rate { get { return _reduce_rate; } set { this._reduce_rate = value; } }

        public float Morale
        {
            get
            {
                return morale_change;
            }
            set
            {
                if(value > 40)
                {
                    morale_change = 40;
                }
                else
                {
                    morale_change = value;
                }
            }
        }
        public float Now
        {
            get
            {
                return _now_tired_sum;
            }
            set
            {
                if(value < 0)
                {
                    this._now_tired_sum = 0;
                }
                else if(value > 1)
                {
                    this._now_tired_sum = 1;
                }
                else
                {
                    this._now_tired_sum = value;
                }
            }
        }

        public tired_party_data(float now_tired_sum, float _reduce_rate, int number)
        {
            this._reduce_rate = _reduce_rate;
            this._now_tired_sum = now_tired_sum;
            _number = number;
            need_recovery = false;
            Limit = 0;
            Morale = 0;
            reset_time = 0;
            is_busy = false;
            is_fleeing = false;
        }

        public tired_party_data()
        {
            _reduce_rate = 0;
            _now_tired_sum = 0;
            _number = 0;
            need_recovery = false;
            Limit = 0;
            Morale = 0;
            reset_time = 0;
            is_busy = false;
            is_fleeing = false;
        }
    }
}
