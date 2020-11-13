using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using log4net;
using Personal_Keybard_Mapper.Console.Properties;
using Personal_Keyboard_Mapper.Lib;
using Personal_Keyboard_Mapper.Lib.Interfaces;
using Personal_Keyboard_Mapper.Lib.Model;
using Personal_Keyboard_Mapper.Lib.Service;

namespace Personal_Keybard_Mapper.Console
{
    class Program
    {
        static private string configFilePath;
        static private IConfigSource config;
        static private ILog logger;
        static private KeysSoundEffects keysSounds;
        static private GlobalHookService hookService;

        [STAThread]
        static void Main(string[] args)
        {
            logger = LogManager.GetLogger(typeof(Program));
            keysSounds = new KeysSoundEffects(logger,Resources.key1, Resources.key2, Resources.ctrl,
                                             Resources.shift, Resources.win, Resources.alt);
            try
            {
                configFilePath = "config.txt";
                config = new JsonConfigSource(logger, configFilePath);
                hookService = new GlobalHookService(logger, config, keysSounds, true);
                hookService.StartHookService();
                Application.Run();
                while (true)
                {
                    if (System.Console.Read() == (int)Keys.Enter)
                    {
                        hookService.StopHookService();
                    }
                }

            }
            catch (Exception e)
            {
                logger.Error(e.StackTrace);
                System.Console.ReadKey();
            }
        }
    }
}
