using HarmonyLib;
using MCM.Abstractions.Settings.Base.Global;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.Remoting.Messaging;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.SandBox.GameComponents.Map;
using TaleWorlds.CampaignSystem.SandBox.GameComponents.Party;
using TaleWorlds.Core;
using TaleWorlds.Core.ViewModelCollection;
using TaleWorlds.Engine;
using TaleWorlds.Engine.Screens;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.GauntletUI;
using Tired_party.Behaviors;
using Tired_party.Helper;
using Tired_party.Model;
using Tired_party.Save;

namespace Tired_party
{
    public class SubModule : MBSubModuleBase
    {
        

        /// <summary>
        /// 游戏处于加载界面时最先被调用的函数，你应该在这个函数中完成初始化的主要部分
        /// </summary>
        protected override void OnSubModuleLoad()
        {
            try
            {
                base.OnSubModuleLoad();
                new Harmony("mod.Tired_party").PatchAll();
            }
            catch(Exception e)
            {
                MethodInfo methodInfo = MethodBase.GetCurrentMethod() as MethodInfo;
                debug_helper.HandleException(e, methodInfo, "submodule load error");
            }
        }

        protected override void OnGameStart(Game game, IGameStarter gameStarter)
        {
            CampaignGameStarter campaign_game_starter = gameStarter as CampaignGameStarter;
            if(campaign_game_starter != null)
            {
                InitializeGame(game, (IGameStarter)campaign_game_starter);
            }
        }

        public override void OnCampaignStart(Game game, object starterObject)
        {
            
        }
        
        private void InitializeGame(Game game, IGameStarter gameStarter)
        {
            Initialize();
            replace_models(gameStarter as CampaignGameStarter);
            AddBehaviours(gameStarter as CampaignGameStarter);
        }

        private void Initialize()
        {
            Party_tired.Current = new Party_tired();
        }

        private void AddBehaviours(CampaignGameStarter starter)
        {
            if(starter == null)
            {
                return;
            }
            starter.AddBehavior(new Hourly_change_behaviour());
            starter.AddBehavior(new Recalculate_ratio_behavior());
            starter.AddBehavior(new AiSleepBehavior());
            starter.AddBehavior(new MBsave_behavior());
        }

        private void replace_models(CampaignGameStarter starter)
        {
            if(starter == null)
            {
                return;
            }
            IList<GameModel> list = starter.Models as IList<GameModel>;
            bool flag = list != null;
            if (flag)
            {
                for(int i=0; i<list.Count; i++)
                {
                    if(list[i] is DefaultPartySpeedCalculatingModel)
                    {
                        list[i] = new Tired_party_speed_model();
                    }
                    if(list[i] is DefaultPartyMoraleModel)
                    {
                        list[i] = new Morale_model();
                    }
                }
            }
        }

        protected override void OnBeforeInitialModuleScreenSetAsRoot()
        {
            if(!GlobalSettings<mod_setting>.Instance.is_ban_information)
                InformationManager.OnAddTooltipInformation += add_information;
            InformationManager.DisplayMessageInternal += store_info;
            InformationManager.FiringQuickInformation += store_quick;
        }

        private void store_quick(string  message, int priorty = 0, BasicCharacterObject announcerCharacter = null, string soundEventPath = "")
        {
            Party_tired.Current.information[0].add_information(message, (float)CampaignTime.Now.ToHours);
        }

        private void store_info(InformationMessage information)
        {
            Party_tired.Current.information[0].add_information(information.ToString(), (float)CampaignTime.Now.ToHours);
        }

        private void add_information(Type type, object[] args)
        {
            if(GlobalSettings<mod_setting>.Instance.is_ban)
            {
                return;
            }
            if (type == typeof(Army) && !GlobalSettings<mod_setting>.Instance.is_ban_army)
            {
                Army army = (Army)args[0];
                float army_now_tired = 0;
                if(last_army == army && Campaign.CurrentTime - last_see_hour_army <= 1)
                {
                    return;
                }
                if(!Party_tired.Current.Party_tired_rate.ContainsKey(army.LeaderParty))
                {
                    if(BannerlordConfig.Language.Equals("简体中文"))
                        message_helper.ErrorMessage(army.LeaderParty.Name.ToString() + "没有加入");
                    else
                        message_helper.ErrorMessage(army.LeaderParty.Name.ToString() + " don't add");
                    return;
                }
                foreach (MobileParty party in army.LeaderPartyAndAttachedParties)
                {
                    if (Party_tired.Current.Party_tired_rate.ContainsKey(party))
                    {
                        army_now_tired += Party_tired.Current.Party_tired_rate[party].Now;
                    }
                }
                army_now_tired /= army.Parties.Count;
                float temp = army_now_tired;
                int remain_hours = 0;
                while (army_now_tired > 0)
                {
                    remain_hours++;
                    army_now_tired -= Party_tired.Current.Party_tired_rate[army.LeaderParty].Reduce_rate;
                }
                last_army = army;
                last_see_hour_army =  Campaign.CurrentTime;
                if(BannerlordConfig.Language.Equals("简体中文"))
                    message_helper.TechnicalMessage(army.Name.ToString() + "还剩" + remain_hours + "小时("+show_information(temp)+")");
                else
                    message_helper.TechnicalMessage(army.Name.ToString() + " remain " + remain_hours + " hours(" + show_information(temp) + ")");
            }
            if (type == typeof(MobileParty))
            {
                MobileParty mobile = (MobileParty)args[0];
                if(last_party == mobile && Campaign.CurrentTime - last_see_hour <= 1)
                {
                    return;
                }
                last_party = mobile;
                last_see_hour = Campaign.CurrentTime;
                
                if (Party_tired.Current.Party_tired_rate.ContainsKey(mobile))
                {
                    if (BannerlordConfig.Language.Equals("简体中文"))
                    {
                        message_helper.TechnicalMessage(mobile.Name + "还剩" + Calculate_party_tired.calculate_remaining_hours(Party_tired.Current.Party_tired_rate[mobile]).ToString() +
                        "小时(" + show_information(Party_tired.Current.Party_tired_rate[mobile].Now) + ")");
                    }
                    else
                    {
                        message_helper.TechnicalMessage(mobile.Name + " remain " + Calculate_party_tired.calculate_remaining_hours(Party_tired.Current.Party_tired_rate[mobile]).ToString() +
                        " hours(" + show_information(Party_tired.Current.Party_tired_rate[mobile].Now) + ")");
                    }
                }
                else if(!mobile.IsCaravan && !mobile.IsVillager)
                {
                    if(BannerlordConfig.Language.Equals("简体中文"))
                        message_helper.ErrorMessage(mobile.Name.ToString() + "没有加入");
                    else
                        message_helper.ErrorMessage(mobile.Name.ToString() + " don't add");
                }
            }
        }
        private Army last_army;
        private float last_see_hour_army = 0;
        private MobileParty last_party;
        private float last_see_hour = 0;

        private string show_information(float rate)
        {
            bool language_is_chinese = BannerlordConfig.Language.Equals("简体中文");
            if(rate > 0.9)
            {
                if (language_is_chinese)
                {
                    return "高昂";
                }
                else
                    return " excited";
            }
            else if (rate > 0.3)
            {
                if(language_is_chinese)
                    return "正常";
                else
                    return "normal";
            }
            else if(rate > 0)
            {
                if(language_is_chinese)
                    return "疲惫";
                else
                    return "tired";
            }
            else
            {
                if(language_is_chinese)
                    return "濒临崩溃";
                else
                    return "Near collapse";
            }
        }


        protected override void OnApplicationTick(float dt)
        {
            this.On_key_press();
        }
        private void On_key_press()
        {
            bool flag = Input.IsKeyDown(InputKey.G);
            bool flag2 = Game.Current != null && Game.Current.GameStateManager != null
                && Game.Current.GameStateManager.ActiveState != null && Game.Current.GameStateManager.ActiveState.GetType() == typeof(MapState)
                && !Game.Current.GameStateManager.ActiveState.IsMission && !Game.Current.GameStateManager.ActiveState.IsMenuState;
            bool flag3 = flag && flag2;
            if (flag3)
            {
                InformationManager.ShowTextInquiry(new TextInquiryData("party message", "enter name", true, true, "yes", "no", yes_action, null));
            }
        }

        void yes_action(string s)
        {
            
            MBReadOnlyList<MobileParty> parties = Campaign.Current.MobileParties;
            foreach(MobileParty party in parties)
            {

                if (party.Name.ToString().Equals(s))
                {
                    Party_tired.test_party = party;
                    if(party.Army != null)
                    {
                        InformationManager.DisplayMessage(new InformationMessage("army" + " " + party.Army.AIBehavior.ToString()));
                    }
                    if(!party.IsMoving)
                    {
                        InformationManager.DisplayMessage(new InformationMessage("party don't move"));
                    }
                    /*if (party.DefaultBehavior == AiBehavior.GoToSettlement)
                    {
                        InformationManager.DisplayMessage(new InformationMessage(party.TargetSettlement.Name.ToString()));
                    }*/
                    InformationManager.DisplayMessage(new InformationMessage(party.DefaultBehavior.ToString()));

                    if(party != null && Party_tired.Current.Party_tired_rate.ContainsKey(party))
                    {
                        InformationManager.DisplayMessage(new InformationMessage(Party_tired.Current.Party_tired_rate[party].Now.ToString(), Colors.Yellow));
                    }
                    else
                    {
                        InformationManager.DisplayMessage(new InformationMessage("party don't add"));
                    }
                    break;
                }
            }
        }
    }
}
