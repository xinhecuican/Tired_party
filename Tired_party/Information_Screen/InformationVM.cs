﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using Tired_party.Helper;

namespace Tired_party.Information_Screen
{
    public class InformationVM : ViewModel
    {
        InformationListVM _all_information;
        InformationListVM _hide_information;
        InformationListVM _important_information;
        public string Title { get; } = SubModule.title.ToString();
        public string SaveText { get; } = SubModule.menu_close.ToString();
        public string CloseText { get; } = SubModule.menu_cancel.ToString();
        public bool IsFinished = false;
        public bool OldGameStateManagerDisabledStatus;

        public InformationVM()
        {
            _all_information = new InformationListVM(new TextObject("{=unxsFo8XOr}all information", null));
            _hide_information = new InformationListVM(new TextObject("{=NLi473iHTH}hidden information", null));
            _important_information = new InformationListVM(new TextObject("{=ylstgZWQuD}important information", null));
            IsFinished = false;
            PauseGame();
        }

        public InformationListVM AllInformation
        {
            get
            {
                return _all_information;
            }
        }

        public InformationListVM HiddenInformation { get { return _hide_information; } }
        public InformationListVM ImportantInformation { get { return _important_information; } }

        private void ExecuteDone()
        {
            IsFinished = true;
        }

        private void ExecuteCancel()
        {
            IsFinished = true;
        }

        internal void PauseGame()
        {
            try
            {
                bool flag = Game.Current != null;
                if (flag)
                {
                    this.OldGameStateManagerDisabledStatus = Game.Current.GameStateManager.ActiveStateDisabledByUser;
                    Game.Current.GameStateManager.ActiveStateDisabledByUser = true;
                }
            }
            catch (Exception e)
            {
                MethodInfo methodInfo = MethodBase.GetCurrentMethod() as MethodInfo;
                debug_helper.HandleException(e, methodInfo, "submodule load error");
            }
        }

        internal void UnpauseGame()
        {
            try
            {
                bool flag = Game.Current != null;
                if (flag)
                {
                    Game.Current.GameStateManager.ActiveStateDisabledByUser = false;
                }
            }
            catch (Exception e)
            {
                MethodInfo methodInfo = MethodBase.GetCurrentMethod() as MethodInfo;
                debug_helper.HandleException(e, methodInfo, "submodule load error");
            }
        }

    }
}
