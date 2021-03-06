﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.CampaignSystem;
using TaleWorlds.SaveSystem;
using Tired_party.Information_Screen;

namespace Tired_party.Save
{
    class tired_save_definer : SaveableTypeDefiner
    {
        public tired_save_definer() : base(432943821) { }

        protected override void DefineClassTypes()
        {
            // The Id's here are local and will be related to the Id passed to the constructor
            AddClassDefinition(typeof(Party_tired), 1);
            AddClassDefinition(typeof(tired_party_data), 2);
            AddClassDefinition(typeof(information_data), 3);
            AddClassDefinition(typeof(MCMsetting_data), 4);
            AddClassDefinition(typeof(information_node), 5);
        }

        /*protected override void DefineStructTypes()
        {
            AddStructDefinition(typeof(information_data.node), 5);
        }*/

        protected override void DefineContainerDefinitions()
        {
            ConstructContainerDefinition(typeof(List<tired_party_data>));
            ConstructContainerDefinition(typeof(Dictionary<MobileParty, tired_party_data>));
            ConstructContainerDefinition(typeof(List<information_data>));
            ConstructContainerDefinition(typeof(Dictionary<float, string>));
            ConstructContainerDefinition(typeof(Queue<information_node>));
        }

        
    }
}
