using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Library;

namespace Tired_party.Mission_time
{
    class arrive_time_data
    {
        public Vec2 enter_direction;
        public float arrive_time;
        public MapEventParty party;
        public int _number;
        public bool is_initialize_over;
        public int side;
        public int initial_num;
        public arrive_time_data(Vec2 enter_direction, float arrive_time, MapEventParty party, int side)
        {
            this.enter_direction = enter_direction;
            this.arrive_time = arrive_time;
            this.party = party;
            _number = party.Party.NumberOfHealthyMembers;
            is_initialize_over = false;
            this.side = side;
            initial_num = _number;
        }

        public int number
        {
            get
            {
                return _number;
            }
            set
            {
                if(value < 0.5 * initial_num)
                {
                    is_initialize_over = true;
                }
                _number = value;
            }
        }
    }
}
