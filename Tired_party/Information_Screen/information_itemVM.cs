﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.Engine.Options;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace Tired_party.Information_Screen
{
    public abstract class information_itemVM : ViewModel
    {
		public information_itemVM(InformationVM optionsVM, TextObject name, TextObject description, int typeID)
		{
			this._nameObj = name;
			this._optionsVM = optionsVM;
			this.OptionTypeID = (int)typeID;
			this._descriptionObj = description;
			this.RefreshValues();
		}

		public override void RefreshValues()
		{
			base.RefreshValues();
		}

		public void ExecuteAction()
		{
		}

		[DataSourceProperty]
		public string Name
		{
			get
			{
				return this._nameObj.ToString();
			}
			set
			{
				this._nameObj = new TextObject(value, null);
				base.OnPropertyChanged("Name");
			}
		}

		[DataSourceProperty]
		public string Description
		{
			get
			{
				return this._descriptionObj.ToString();
			}
			set
			{
				this._descriptionObj = new TextObject(value, null);
				base.OnPropertyChanged("Description");
			}
		}

		[DataSourceProperty]
		public string[] ImageIDs
		{
			get
			{
				return this._imageIDs;
			}
			set
			{
				bool flag = value != this._imageIDs;
				if (flag)
				{
					this._imageIDs = value;
					base.OnPropertyChanged("ImageIDs");
				}
			}
		}

		[DataSourceProperty]
		public int OptionTypeID
		{
			get
			{
				return this._optionTypeId;
			}
			set
			{
				bool flag = value != this._optionTypeId;
				if (flag)
				{
					this._optionTypeId = value;
					base.OnPropertyChanged("OptionTypeID");
				}
			}
		}

		private TextObject _nameObj;
		protected InformationVM _optionsVM;
		private TextObject _descriptionObj;
		private int _optionTypeId = -1;

		private string[] _imageIDs;
	}
}
