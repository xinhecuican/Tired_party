﻿using Messages.FromLobbyServer.ToClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.Localization;

namespace Tired_party.Helper
{
    class debug_helper
    {
		public static void HandleException(Exception ex, MethodInfo methodInfo, string sectionName)
		{
			message_helper.ErrorMessage(string.Format("tired party - error occured in [{1}]{0} - {2} See details in the mod log.", (methodInfo != null) ? (" in " + methodInfo.Name) : "", sectionName, ex.Message));
			log_helper.Log(string.Format("Error occured{0} - {1}", (methodInfo != null) ? string.Format(" in {0}", methodInfo) : "", ex.ToString()), sectionName);
		}

		public static void HandleException(Exception ex, string sectionName, string logMessage, string chatMessage)
		{
			if (chatMessage.Length > 0)
			{
				TextObject textObject = new TextObject(chatMessage, null);
				textObject.SetTextVariable("SECTION", sectionName);
				textObject.SetTextVariable("EXCEPTION_MESSAGE", ex.Message);
				message_helper.ErrorMessage(textObject);
			}
			log_helper.Log(string.Format(logMessage, ex.ToString()), sectionName);
		}
	}
}
