using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.CampaignSystem;
using TaleWorlds.SaveSystem;

namespace Tired_party
{
    public class Party_tired
    {
        [SaveableField(1)]
        private Dictionary<MobileParty, tired_party_data> _party_tired_rate;

        public static float recovery_in_day_time = 0.25f;
        public static float recovery_in_night_time = 0.33f;

        public Dictionary<MobileParty, tired_party_data> Party_tired_rate
        {
            get
            {
                if(_party_tired_rate == null)
                {
                    _party_tired_rate = new Dictionary<MobileParty, tired_party_data>();
                }
                return _party_tired_rate;
            }
            
            set
            {
                this._party_tired_rate = value;
            }
        }

        public static Party_tired Current
        {
            get
            {
                return SubModule.Current.party_tired;
            }
        }

        public static void add_to_dict(MobileParty mobileParty, float now_tired = 1)
        {
            if(mobileParty.IsCaravan)
            {
                return;
            }
            float rate = Calculate_party_tired.calculate_ratio(mobileParty);
            tired_party_data data = new tired_party_data(1.0f, rate);
            Current._party_tired_rate.Add(mobileParty, data);
        }

        public void initialize()
        {
            this._party_tired_rate = new Dictionary<MobileParty, tired_party_data>();
        }
    }
}
