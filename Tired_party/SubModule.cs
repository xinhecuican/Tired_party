using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.GameMenus;
using TaleWorlds.CampaignSystem.SandBox.GameComponents.Map;
using TaleWorlds.CampaignSystem.SandBox.GameComponents.Party;
using TaleWorlds.CampaignSystem.ViewModelCollection;
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
            base.OnSubModuleLoad();
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

        protected override void OnApplicationTick(float dt)
        {
            this.On_key_press();
        }
        private void On_key_press()
        {
            bool flag = Input.IsKeyDown(InputKey.G) && Input.IsKeyDown(InputKey.LeftAlt);
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
                    Party_tired.test_party = party;
                    if(party.MapFaction.IsMinorFaction)
                    {
                        InformationManager.DisplayMessage(new InformationMessage("minor faction", Colors.Blue));
                    }
                    if(!party.IsMoving)
                    {
                        InformationManager.DisplayMessage(new InformationMessage("party don't move"));
                    }
                    if (party.DefaultBehavior == AiBehavior.GoToSettlement)
                    {
                        InformationManager.DisplayMessage(new InformationMessage(party.TargetSettlement.Name.ToString()));
                    }
                    InformationManager.DisplayMessage(new InformationMessage(party.ShortTermBehavior.ToString()));

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
