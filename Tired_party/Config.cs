﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using TaleWorlds.Library;
using Tired_party.Helper;

namespace Tired_party
{
    [Serializable]
    public class Config
    {
        public Config(string file_path)
        {
            
            try
            {
                using (StreamReader streamReader = new StreamReader(file_path))
                {
                    SubModule.config = (Config)new XmlSerializer(typeof(Config)).Deserialize(streamReader);
                }
            }
            catch (Exception e)
            {
                MethodInfo methodInfo = MethodBase.GetCurrentMethod() as MethodInfo;
                debug_helper.HandleException(e, methodInfo, "submodule load error");
                SubModule.config = new Config();
            }
        }

        public Config()
        {
            battle_radius = 6f;
        }

        public static void write_to_config()
        {
            if(SubModule.config == null)
            {
                return;
            }
            string file_path = Path.Combine(new string[]
            {
                BasePath.Name,
                "Modules",
                "Tired_party",
                "ModuleData",
                "Config.xml"
            });
            string content = string.Empty;
            using (System.IO.StringWriter writer = new System.IO.StringWriter())
            {
                XmlSerializer xz = new XmlSerializer(typeof(Config));
                xz.Serialize(writer, SubModule.config);
                content = writer.ToString();
            }
            using(StreamWriter stream_writer = new StreamWriter(file_path))
            {
                stream_writer.Write(content);
            }
        }
        public float battle_radius { get; set; } = 6f;
    }
}
