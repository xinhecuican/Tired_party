﻿
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.Engine;
using TaleWorlds.Engine.GauntletUI;
using TaleWorlds.Engine.Screens;
using TaleWorlds.GauntletUI.Data;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;
using TaleWorlds.TwoDimension;

namespace Tired_party.Information_Screen
{
    public class Tired_information_screen : ScreenBase
    {
		private GauntletLayer _gauntletLayer;
		private InformationVM _dataSource;
		private GauntletMovie _gauntletMovie;
		private SpriteCategory _spriteCategory;

		protected override void OnInitialize()
		{
			base.OnInitialize();
			SpriteData spriteData = UIResourceManager.SpriteData;
			TwoDimensionEngineResourceContext resourceContext = UIResourceManager.ResourceContext;
			ResourceDepot uiresourceDepot = UIResourceManager.UIResourceDepot;
			this._spriteCategory = spriteData.SpriteCategories["ui_options"];
			this._spriteCategory.Load(resourceContext, uiresourceDepot);
			this._dataSource = new InformationVM();
			this._gauntletLayer = new GauntletLayer(4000, "GauntletLayer");
			this._gauntletMovie = this._gauntletLayer.LoadMovie("tired_party_information_screen", this._dataSource);
			this._gauntletLayer.Input.RegisterHotKeyCategory(HotKeyManager.GetCategory("GenericPanelGameKeyCategory"));
			this._gauntletLayer.InputRestrictions.SetInputRestrictions(true, InputUsageMask.All);
			this._gauntletLayer.IsFocusLayer = true;
			base.AddLayer(this._gauntletLayer);
			ScreenManager.TrySetFocus(this._gauntletLayer);
			Utilities.SetForceVsync(true);
		}

		protected override void OnFinalize()
		{
			base.OnFinalize();
			this._spriteCategory.Unload();
			Utilities.SetForceVsync(false);
		}

		protected override void OnDeactivate()
		{
			LoadingWindow.EnableGlobalLoadingWindow(false);
		}

		protected override void OnFrameTick(float dt)
		{
			base.OnFrameTick(dt);
			bool flag = (SubModule.information_screen_is_open && this._dataSource.IsFinished) || (this._gauntletLayer.Input.IsHotKeyReleased("Exit"));
			if (flag)
			{
				this._dataSource.UnpauseGame();
				ScreenManager.PopScreen();
				SubModule.information_screen_is_open = false;
			}
		}
	}
}
