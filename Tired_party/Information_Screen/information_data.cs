using System;
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
    public class information_data
    {
        [SaveableField(5)]
        public Queue<information_node> data2;
        //[SaveableField(1)]
        //public Dictionary<float, string> data;
        [SaveableField(2)]
        public TextObject name;

        

        public information_data(TextObject name)
        {
            data2 = new Queue<information_node>();
            //data = new Dictionary<float, string>();
            this.name = name;
        }

        public information_data()
        {
            data2 = new Queue<information_node>();
            //data = new Dictionary<float, string>();
        }

        public void add_information(string text, float time)
        {
            information_node temp = new information_node(text, time);
            data2.Enqueue(temp);
            //data[time] = text;
        }

        public void delete_outdated_information()
        {
            //排序nlogn,直接走一遍只需要n
            float test_hour = (float)(CampaignTime.Now.ToHours - 24 * 14);
            bool flag = true;
            while(flag)
            {
                information_node temp = data2.Peek();
                if(temp.time < test_hour)
                {
                    data2.Dequeue();
                }
                else
                {
                    flag = false;
                }
            }
            /*foreach(var item in  data.ToList())
            {
                if(item.Key < test_hour)
                {
                    data.Remove(item.Key);
                }
            }*/
        }
    }
}
