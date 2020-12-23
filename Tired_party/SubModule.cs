using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.GameMenus;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Engine.Screens;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade;
namespace Tired_party
{
    public class SubModule : MBSubModuleBase
    {
        private static Hourly_decrease_behaviour hourly_behaviour;

        internal static Hourly_decrease_behaviour Hourly_behaviour { get => hourly_behaviour; set => hourly_behaviour = value; }

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

        protected override void OnGameStart(Game game, IGameStarter gameStarterObject)
        {
            base.OnGameStart(game, gameStarterObject);
            bool flag = (game.GameType is Campaign);
            if(flag)
            {
                this.InitializeGame(game, gameStarterObject);
            }
                
        }

        private void InitializeGame(Game game, IGameStarter gameStarter)
        {
            SubModule.Hourly_behaviour = new Hourly_decrease_behaviour();
            this.AddBehaviours(gameStarter as CampaignGameStarter);
        }

        private void AddBehaviours(CampaignGameStarter starter)
        {
            starter.AddBehavior(SubModule.hourly_behaviour);
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
                GameMenu.SwitchToMenu("village");
            }
        }
    }
}
