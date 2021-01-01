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
    [SaveableClass(34124065)]
    public class Party_tired
    {
        [SaveableField(1)]
        public Dictionary<MobileParty, tired_party_data> _party_tired_rate;
        [SaveableField(2)]
        private static Party_tired _party_tired;

        public static float recovery_in_day_time = 0.25f;
        public static float recovery_in_night_time = 0.33f;
        public static float begin_to_decrease = 0.3f;
        public static MobileParty test_party = null;
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
                if(Party_tired._party_tired == null)
                {
                    Party_tired._party_tired = new Party_tired();
                }
                return Party_tired._party_tired;
            }
            set
            {
                Party_tired._party_tired = value;
            }
        }

        public static void add_to_dict(MobileParty mobileParty, float now_tired = 1)
        {
            if(mobileParty.IsCaravan || mobileParty.IsVillager)
            {
                return;
            }
            float rate = Calculate_party_tired.calculate_ratio(mobileParty);
            tired_party_data data = new tired_party_data(1.0f, rate, mobileParty.MemberRoster.TotalManCount);
            data.AiBehavior = mobileParty.DefaultBehavior;
            data.ai_behavior_object = mobileParty.TargetSettlement;
            data.ai_behavior_target = mobileParty.TargetPosition;
            data.target_party = mobileParty.TargetParty;
            Current._party_tired_rate.Add(mobileParty, data);
        }

        public Party_tired()
        {
            this._party_tired_rate = new Dictionary<MobileParty, tired_party_data>();
        }
    }
}
