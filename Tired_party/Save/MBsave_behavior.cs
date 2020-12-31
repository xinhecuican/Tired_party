﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.CampaignSystem;

namespace Tired_party.Save
{
    class MBsave_behavior : CampaignBehaviorBase
    {
        public override void RegisterEvents()
        {
            throw new NotImplementedException();
        }
        public override void SyncData(IDataStore dataStore)
        {
            try
            {
                dataStore.SyncData("_party_tired", ref party_tired);
            }
            catch (Exception)
            {
            }
            finally
            {
                if(party_tired == null)
                {
                    party_tired = new Party_tired();
                }
            }
        }
        Party_tired party_tired = Party_tired.Current;
    }
}
