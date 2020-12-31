using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Library;

namespace Tired_party
{
    public class tired_party_data
    {
        private float _reduce_rate;
        private float _now_tired_sum;
        private int _number;
        public AiBehavior AiBehavior;
        public Settlement ai_behavior_object;
        public Vec2 ai_behavior_target;
        public bool need_recovery;
        public MobileParty target_party;
        
        public int Number { get { return _number; } set { if (value < 0) { _number = 0; } else { _number = value; } } }
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

        public tired_party_data(float now_tired_sum, float _reduce_rate, int number)
        {
            this._reduce_rate = _reduce_rate;
            this._now_tired_sum = now_tired_sum;
            _number = number;
            need_recovery = false;
        }
    }
}
