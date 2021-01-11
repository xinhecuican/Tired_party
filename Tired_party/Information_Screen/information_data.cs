﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Engine;
using TaleWorlds.Localization;
using TaleWorlds.SaveSystem;
using TaleWorlds.TwoDimension;

namespace Tired_party.Information_Screen
{
    class information_data
    {
        [SaveableField(1)]
        Dictionary<float, string> data;
        [SaveableField(2)]
        public string name;

        

        public information_data(string name)
        {
            data = new Dictionary<float, string>();
            this.name = name;
        }

        public information_data()
        {
            data = new Dictionary<float, string>();
        }

        public void add_information(string text, float time)
        {
            data[time] = text;
        }

        public void delete_outdated_information()
        {
            //排序nlogn,直接走一遍只需要n
            float test_hour = (float)(CampaignTime.Now.ToHours - 24 * 14);
            foreach(var item in  data.ToList())
            {
                if(item.Key < test_hour)
                {
                    data.Remove(item.Key);
                }
            }
        }
    }
}
