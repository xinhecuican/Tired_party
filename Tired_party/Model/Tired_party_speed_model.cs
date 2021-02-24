using MCM.Abstractions.Settings.Base.Global;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.SandBox.GameComponents.Map;
using Tired_party;

namespace Tired_party.Model
{
    class Tired_party_speed_model : DefaultPartySpeedCalculatingModel
    {
        public override ExplainedNumber CalculateFinalSpeed(MobileParty mobileParty, ExplainedNumber finalSpeed)
        {
            ExplainedNumber base_speed = base.CalculateFinalSpeed(mobileParty, finalSpeed);
            if (Party_tired.Current == null)
            {
                return base_speed;
            }
            //Army速度太慢，就不减速了
            if (mobileParty.Army != null && (mobileParty.Army.LeaderParty.AttachedParties.Contains(mobileParty) || mobileParty.Army.LeaderParty == mobileParty))
            {
                return base_speed;
            }
            tired_party_data tired = null;
            Party_tired.Current.Party_tired_rate.TryGetValue(mobileParty, out tired);
            if (tired != null && !GlobalSettings<mod_setting>.Instance.is_ban)
            {
                base_speed.AddFactor(Get_ratio(tired)-1, new TaleWorlds.Localization.TextObject("tired party"));
                return base_speed;
            }
            return base_speed;
        }

        private float Get_ratio(tired_party_data tired)
        {
            float now = tired.Now;
            if (now >= 0.8)
            {
                return 1.1f;
            }
            else if (now >= Party_tired.begin_to_decrease)
            {
                return 1f;
            }
            else if (now >= GlobalSettings<mod_setting>.Instance.limit_speed - 0.7f)
            {
                return (float)(1f - (Party_tired.begin_to_decrease - now));
            }
            else
            {
                return GlobalSettings<mod_setting>.Instance.limit_speed;
            }
        }
    }


}