﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using Tired_party.Helper;

namespace Tired_party.Information_Screen
{
    public class InformationListVM : ViewModel
    {

        private string _name;
        private MBBindingList<InformationDataVM> text;
        private string sum;
        public InformationListVM(TextObject name)
        {
            _name = name.ToString();
            for(int i=0; i<Party_tired.Current.information.Count; i++)
            {
                if(Party_tired.Current.information[i].name.Equals(name))
                {
                    text = new MBBindingList<InformationDataVM>();
                    /*List<string> temp_text = Party_tired.Current.information[i].data.Values.ToList();
                    List<float> temp_list = Party_tired.Current.information[i].data.Keys.ToList();
                    for(int k=temp_list.Count-1; k>0; k--)
                    {
                        if(Math.Floor(Campaign.CurrentTime - temp_list[k]) <= 24)
                        {
                            text.Add(new InformationDataVM(temp_text[k], ((int)Math.Floor(Campaign.CurrentTime - temp_list[k])).ToString() + new TextObject("{=ue9utZpXrz}hours ago", null).ToString()));
                        }
                        else
                        {
                            text.Add(new InformationDataVM(temp_text[k], ((int)Math.Floor((Campaign.CurrentTime - temp_list[k]) / 24)).ToString() + new TextObject("{=HTkrMFoJyi}days ago", null).ToString()));
                        }
                    }*/

                    foreach(var item in Party_tired.Current.information[i].data2.Reverse())
                    {
                        if (Math.Floor(Campaign.CurrentTime - item.time) <= 24)
                        {
                            text.Add(new InformationDataVM(item.info, ((int)Math.Floor(Campaign.CurrentTime - item.time)).ToString() + new TextObject("{=ue9utZpXrz}hours ago", null).ToString()));
                        }
                        else
                        {
                            text.Add(new InformationDataVM(item.info, ((int)Math.Floor((Campaign.CurrentTime - item.time) / 24)).ToString() + new TextObject("{=HTkrMFoJyi}days ago", null).ToString()));
                        }
                    }
                    text.Reverse();
                    sum = text.Count.ToString();
                    break;
                }

            }
        }

        [DataSourceProperty]
        public string Name
        {
            get
            {
                return this._name;
            }
            set
            {
                bool flag = value != this._name;
                if (flag)
                {
                    this._name = value;
                    base.OnPropertyChanged("Name");
                }
            }
        }

        [DataSourceProperty]
        public MBBindingList<InformationDataVM> Texts
        {
            get
            {
                return text;
            }
            set
            {
                if(value != text)
                {
                    text = value;
                    OnPropertyChangedWithValue(value, "Texts");
                }
            }
        }

        [DataSourceProperty]
        public string Sum
        {
            get
            {
                return sum;
            }
            set
            {
                if(value != sum)
                {
                    sum = value;
                    OnPropertyChangedWithValue(value, "Sum");
                }
            }
        }
    }
}
