﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.SaveSystem;

namespace Tired_party
{
    class tired_save_definer : SaveableTypeDefiner
    {
        public tired_save_definer() : base(432943821) { }

        protected override void DefineClassTypes()
        {
            // The Id's here are local and will be related to the Id passed to the constructor
            AddClassDefinition(typeof(Party_tired), 1);
            AddClassDefinition(typeof(SubModule), 2);
        }

        protected override void DefineContainerDefinitions()
        {

        }
    }
}
