﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.SaveSystem;

namespace Tired_party.Information_Screen
{
    public class information_node
    {
        [SaveableField(1)]
        public string info;
        [SaveableField(2)]
        public float time;
        public information_node(string info, float time)
        {
            this.info = info;
            this.time = time;
        }
    }
}
