﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace Tired_party.Information_Screen
{
    public class InformationDataVM : ViewModel
    {
        string _text;
        string _time;
 
        public InformationDataVM(string text, string time)
        {
            _text = text;
            _time = time;
        }

        public override void RefreshValues()
        {
            base.RefreshValues();
        }

        [DataSourceProperty]
        public string Text
        {
            get
            {
                return _text;
            }
            set
            {
                if(value != _text)
                {
                    _text = value;
                    OnPropertyChanged("Text");
                }
            }
        }

        [DataSourceProperty]
        public string Time
        {
            get
            {
                return _time;
            }
            set
            {
                if(value != _time)
                {
                    _time = value;
                    OnPropertyChanged("Time");
                }
            }
        }
    }
}
