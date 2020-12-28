﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tired_party
{
    public class tired_party_data
    {
        private float _reduce_rate;
        private float _now_tired_sum;

        public float Reduce_rate { get { return _reduce_rate; } set { this._now_tired_sum = value; } }
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

        public tired_party_data(float now_tired_sum, float _reduce_rate)
        {
            this._reduce_rate = _reduce_rate;
            this._now_tired_sum = now_tired_sum;
        }
    }
}
