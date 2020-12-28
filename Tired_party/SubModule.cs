using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.GameMenus;
using TaleWorlds.CampaignSystem.SandBox.GameComponents.Map;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Engine.Screens;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.ViewModelCollection;
using TaleWorlds.SaveSystem;
using Tired_party.Behaviors;
using Tired_party.Model;
namespace Tired_party
{
    public class SubModule : MBSubModuleBase
    {
        [SaveableField(1)]
        public Party_tired party_tired;

        private static SubModule _current;

        public SubModule()
        {
            party_tired = new Party_tired();
        }
        public static SubModule Current
        {
            get
            {
                if(_current == null)
                {
                    _current = new SubModule();
                }
                return _current;
            }

            set
            {
                if(value is SubModule)
                {
                    _current = value;
                }
                
            }
        }

        /// <summary>
        /// 游戏处于加载界面时最先被调用的函数，你应该在这个函数中完成初始化的主要部分
        /// </summary>
        protected override void OnSubModuleLoad()
        {
            base.OnSubModuleLoad();
            Module.CurrentModule.AddInitialStateOption(new InitialStateOption("Message",
                new TextObject("try", null), 9990,
                () => { 
                    InformationManager.DisplayMessage(new InformationMessage("Hello World!"));
                },
                false));
        }

        protected override void OnGameStart(Game game, IGameStarter gameStarter)
        {
            CampaignGameStarter campaign_game_starter = gameStarter as CampaignGameStarter;
            if(campaign_game_starter != null)
            {
                this.replace_models(campaign_game_starter);
            }
        }

        public override void OnCampaignStart(Game game, object starterObject)
        {
            base.OnCampaignStart(game, starterObject);
            bool flag = (game.GameType is Campaign);
            if(flag)
            {
                InitializeGame(game, (IGameStarter)starterObject);
            }
        }
        
        private void InitializeGame(Game game, IGameStarter gameStarter)
        {
            Initialize();
            replace_models(gameStarter as CampaignGameStarter);
            AddBehaviours(gameStarter as CampaignGameStarter);
        }

        private void Initialize()
        {
            Current = new SubModule();
            Party_tired.Current.initialize();
        }

        private void AddBehaviours(CampaignGameStarter starter)
        {
            if(starter == null)
            {
                return;
            }
            starter.AddBehavior(new Hourly_change_behaviour());
            starter.AddBehavior(new Recalculate_ratio_behavior());
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
                }
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
                if(party.LeaderHero != null && party.LeaderHero.Name.ToString().Equals(s))
                {
                    InformationManager.DisplayMessage(new InformationMessage(party.DefaultBehavior.ToString()));
                    if(party.DefaultBehavior == AiBehavior.GoToSettlement)
                    {
                        InformationManager.DisplayMessage(new InformationMessage(party.TargetSettlement.Name.ToString()));
                    }
                    
                    break;
                }
                if (party.Name.ToString().Equals(s))
                {
                    if(!party.IsMoving)
                    {
                        InformationManager.DisplayMessage(new InformationMessage("party don't move now!"));
                    }
                    if (party.DefaultBehavior == AiBehavior.GoToSettlement)
                    {
                        InformationManager.DisplayMessage(new InformationMessage(party.TargetSettlement.Name.ToString()));
                    }
                    InformationManager.DisplayMessage(new InformationMessage(party.ShortTermBehavior.ToString()));
                    if(!party.IsActive)
                    {
                        InformationManager.DisplayMessage(new InformationMessage("it's not active"));
                    }

                    if(!party.IsVisible)
                    {
                        InformationManager.DisplayMessage(new InformationMessage("it's not visible"));
                    }

                    if(party != null && Party_tired.Current.Party_tired_rate.ContainsKey(party))
                    {
                        InformationManager.DisplayMessage(new InformationMessage(Party_tired.Current.Party_tired_rate[party].Now.ToString()));
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
