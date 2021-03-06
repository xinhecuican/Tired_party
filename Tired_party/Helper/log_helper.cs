﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.Library;

namespace Tired_party.Helper
{
    class log_helper
    {
		public static void Log(string message)
		{
			string aologFile = log_helper.AOLogFile;
			lock (aologFile)
			{
				using (StreamWriter streamWriter = File.AppendText(log_helper.AOLogFile))
				{
					streamWriter.WriteLine(message);
				}
			}
		}

		public static void Log(string message, string sectionName)
		{
			string aologFile = log_helper.AOLogFile;
			lock (aologFile)
			{
				using (StreamWriter streamWriter = File.AppendText(log_helper.AOLogFile))
				{
					streamWriter.WriteLine(string.Format("[{0:dd.MM.yyyy HH:mm:ss}] - {1}.\n{2}", DateTime.Now, sectionName, message));
				}
			}
		}

		public static readonly string AOLogFile = Path.Combine(BasePath.Name, "Modules", "Tired_party", "tired_party.log");
	}
}
