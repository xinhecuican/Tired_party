using HarmonyLib;
using MCM.Abstractions.Settings.Base.Global;
using System;
using System.Collections.Generic;
using System.Reflection;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Engine.Screens;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade;
using Tired_party.Behaviors;
using Tired_party.Helper;
using Tired_party.Information_Screen;
using Tired_party.Model;
using Tired_party.Save;
using Tired_party.sneak_attack;

namespace Tired_party
{
    public class SubModule : MBSubModuleBase
    {
        public static TextObject menu_close = new TextObject("{=BnePFn4jE6}Exit", null);
        public static TextObject menu_cancel = new TextObject("{=ybgznSENV2}Done", null);
        public static TextObject title = new TextObject("{=2l0MyiIYvS}Information Screen", null);

        /// <summary>
        /// 游戏处于加载界面时最先被调用的函数，你应该在这个函数中完成初始化的主要部分
        /// </summary>
        protected override void OnSubModuleLoad()
        {
            try
            {
                base.OnSubModuleLoad();
                MethodInfo method = typeof(MissionAgentSpawnLogic).Assembly.
                    GetType("TaleWorlds.MountAndBlade.MissionAgentSpawnLogic+MissionSide").
                    GetMethod("SpawnTroops", BindingFlags.Instance | BindingFlags.Public);
                MethodInfo prefix = typeof(spawn_logic_patch).GetMethod("Prefix", BindingFlags.Static | BindingFlags.Public);
                
                
                Harmony harmony = new Harmony("mod.Tired_party");
                harmony.Patch(method, new HarmonyMethod(prefix), null, null, null);
                harmony.PatchAll();
            }
            catch(Exception e)
            {
                MethodInfo methodInfo = MethodBase.GetCurrentMethod() as MethodInfo;
                debug_helper.HandleException(e, methodInfo, "submodule load error");
            }
        }

        protected override void OnGameStart(Game game, IGameStarter gameStarter)
        {
            base.OnGameStart(game, gameStarter);
            try
            {
                bool flag = !(game.GameType is Campaign);
                if (!flag)
                {
                    bool flag2 = !(gameStarter is CampaignGameStarter);
                    if (!flag2)
                    {
                        InitializeGame(game, (IGameStarter)gameStarter);
                    }
                }
            }
            catch(Exception e)
            {
                MethodInfo methodInfo = MethodBase.GetCurrentMethod() as MethodInfo;
                debug_helper.HandleException(e, methodInfo, "submodule load error");
            }
        }

        public override void OnCampaignStart(Game game, object starterObject)
        {
            
        }

        public override void OnGameEnd(Game game)
        {
            base.OnGameEnd(game);
            InformationManager.OnAddTooltipInformation -= add_information;
            InformationManager.DisplayMessageInternal -= store_info;
            InformationManager.FiringQuickInformation -= store_quick;
        }

        private void InitializeGame(Game game, IGameStarter gameStarter)
        {
            Initialize();
            if (!GlobalSettings<mod_setting>.Instance.is_ban)
            {
                replace_models(gameStarter as CampaignGameStarter);
                AddBehaviours(gameStarter as CampaignGameStarter);
            }
        }

        private void Initialize()
        {
            Party_tired.Current = new Party_tired();
            InformationManager.OnAddTooltipInformation += add_information;
            InformationManager.DisplayMessageInternal += store_info;
            InformationManager.FiringQuickInformation += store_quick;
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
            starter.AddBehavior(new gamemenu_beahvior());
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
                /*for(int i=0; i<list.Count; i++)
                {
                    if(list[i] is DefaultPartySpeedCalculatingModel)
                    {
                        list[i] = new Tired_party_speed_model();
                    }
                    if(list[i] is DefaultPartyMoraleModel)
                    {
                        list[i] = new Morale_model();
                    }
                    if(list[i] is DefaultCombatSimulationModel && !GlobalSettings<mod_setting>.Instance.is_ban_simulation_effect)
                    {
                        list[i] = new tired_party_combat_simulate_model();
                    }
                    if(list[i] is SandboxAgentStatCalculateModel && !GlobalSettings<mod_setting>.Instance.is_ban_combat_effect)
                    {
                        list[i] = new combat_state_model();
                    }
                }*/
                starter.AddModel(new Tired_party_speed_model());
                starter.AddModel(new Morale_model());
                starter.AddModel(new tired_party_combat_simulate_model());
                starter.AddModel(new combat_state_model());
            }
        }

        protected override void OnBeforeInitialModuleScreenSetAsRoot()
        {
        }

        private void store_quick(string message, int priorty = 0, BasicCharacterObject announcerCharacter = null, string soundEventPath = "")
        {
            Party_tired.Current.information[0].add_information(message, (float)Campaign.CurrentTime);
            Party_tired.Current.information[2].add_information(message, (float)Campaign.CurrentTime);
        }

        private void store_info(InformationMessage information)
        {
            Party_tired.Current.information[0].add_information(information.Information, (float)Campaign.CurrentTime);
        }

        private void add_information(Type type, object[] args)
        {
            if (GlobalSettings<mod_setting>.Instance.is_ban || GlobalSettings<mod_setting>.Instance.is_ban_information)
            {
                return;
            }
            if (type == typeof(Army) && !GlobalSettings<mod_setting>.Instance.is_ban_army)
            {
                Army army = (Army)args[0];
                float army_now_tired = 0;
                if (last_army == army && Campaign.CurrentTime - last_see_hour_army <= 1)
                {
                    return;
                }
                if (!Party_tired.Current.Party_tired_rate.ContainsKey(army.LeaderParty))
                {
                    if (BannerlordConfig.Language.Equals("简体中文"))
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
                last_see_hour_army = Campaign.CurrentTime;
                if (BannerlordConfig.Language.Equals("简体中文"))
                    message_helper.TechnicalMessage(army.Name.ToString() + "还剩" + remain_hours + "小时(" + show_information(temp) + ")");
                else
                    message_helper.TechnicalMessage(army.Name.ToString() + " remain " + remain_hours + " hours(" + show_information(temp) + ")");
            }
            if (type == typeof(MobileParty))
            {
                MobileParty mobile = (MobileParty)args[0];
                if (last_party == mobile && Campaign.CurrentTime - last_see_hour <= 1)
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
                else if (!mobile.IsCaravan && !mobile.IsVillager)
                {
                    if (BannerlordConfig.Language.Equals("简体中文"))
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
            if (rate > 0.9)
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
                if (language_is_chinese)
                    return "正常";
                else
                    return "normal";
            }
            else if (rate > 0)
            {
                if (language_is_chinese)
                    return "疲惫";
                else
                    return "tired";
            }
            else
            {
                if (language_is_chinese)
                    return "濒临崩溃";
                else
                    return "Near collapse";
            }
        }

        protected override void OnApplicationTick(float dt)
        {
            
            if(Input.IsKeyDown(InputKey.M))
            {
                foreach(Formation formation in Mission.Current.PlayerEnemyTeam.Formations)
                {
                    InformationManager.DisplayMessage(new InformationMessage(formation.PrimaryClass.ToString() + formation.CountOfUnits.ToString()));
                }
            }
             this.On_key_press();
        }
        private void On_key_press()
        {
            bool flag = Input.IsKeyDown(InputKey.M) && Input.IsKeyDown(InputKey.LeftAlt);
            bool flag2 = Game.Current != null && Game.Current.GameStateManager != null
                && Game.Current.GameStateManager.ActiveState != null && Game.Current.GameStateManager.ActiveState.GetType() == typeof(MapState)
                && !Game.Current.GameStateManager.ActiveState.IsMission && !Game.Current.GameStateManager.ActiveState.IsMenuState;
            bool flag3 = flag && flag2;
            if (flag3)
            {
                if(!SubModule.information_screen_is_open)
                {
                    _screen = new Tired_information_screen();
                    ScreenManager.PushScreen(_screen);
                    information_screen_is_open = true;
                }             
            }
            bool flag4 = Input.IsKeyDown(InputKey.G);
            if(flag4 && flag2 && GlobalSettings<mod_setting>.Instance.is_ban_debug)
            {
                InformationManager.ShowTextInquiry(new TextInquiryData("party message", "enter name", true, true, "yes", "no", yes_action, hide_inquery), true);
            }
        }
        private static Tired_information_screen _screen;
        public static bool information_screen_is_open = false;

        void yes_action(string s)
        {
            
            MBReadOnlyList<MobileParty> parties = Campaign.Current.MobileParties;
            foreach(MobileParty party in parties)
            {

                if (party.Name.ToString().Equals(s))
                {
                    Party_tired.test_party = party;
                    /*if (party.DefaultBehavior == AiBehavior.GoToSettlement)
                    {
                        InformationManager.DisplayMessage(new InformationMessage(party.TargetSettlement.Name.ToString()));
                    }*/
                    InformationManager.DisplayMessage(new InformationMessage(party.DefaultBehavior.ToString()));
                    InformationManager.DisplayMessage(new InformationMessage(MBRandom.RandomFloat.ToString(), Colors.Yellow));

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
            InformationManager.HideInquiry();
        }

        void hide_inquery()
        {
            InformationManager.HideInquiry();
        }
    }
}
