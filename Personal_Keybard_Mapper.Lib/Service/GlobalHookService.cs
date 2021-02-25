using System;
using System.Windows.Forms;
using log4net;
using Personal_Keyboard_Mapper.Lib.Enums;
using Personal_Keyboard_Mapper.Lib.Hooks;
using Personal_Keyboard_Mapper.Lib.Interfaces;
using Personal_Keyboard_Mapper.Lib.Model;

namespace Personal_Keyboard_Mapper.Lib.Service
{
    /// <summary>
    /// Offers a methods to control global hook.
    /// </summary>
    public class GlobalHookService
    {
        private ILog logger;
        public KeyCombinationsConfiguration combinationsConfig;
        private IHook KeyboardHook;
        private IHook MouseHook;
        private IConfigSource configSource;
        private KeysSoundEffects keysSounds;

        public GlobalHookService(ILog log, IConfigSource config, KeysSoundEffects soundEffects, bool keySoundOn = false)
        {
            Globals.IsSoundOn = keySoundOn;
            logger = log;
            configSource = config;
            keysSounds = soundEffects;
        }

        public void LoadCombinationsConfiguration(IConfigSource config = null)
        {
            if (config != null)
            {
                configSource = config;
            }

            if (configSource == null)
            {
                throw new NullReferenceException(nameof(configSource));
            }

            logger.Info("Loading configuration");
            combinationsConfig = configSource.ReadConfigFromFile();
            logger.Info("configuration loaded");
        }

        public void StartHookService(IConfigSource config = null, ActionType actionHookType = ActionType.Both)
        {
            LoadCombinationsConfiguration(config);

            switch (actionHookType)
            {
                case ActionType.Keyboard:
                    KeyboardHook = new KeyboardHook(logger, combinationsConfig, keysSounds);
                    break;
                case ActionType.Mouse:
                    MouseHook = new MouseHook(logger);
                    break;
                default:
                    KeyboardHook = new KeyboardHook(logger, combinationsConfig, keysSounds);
                    MouseHook = new MouseHook(logger);
                    break;
            }
            KeyboardHook?.StartHook();
            MouseHook?.StartHook();
        }

        public void StopHookService(ActionType hookActionType = ActionType.Both)
        {
            switch (hookActionType)
            {
                case ActionType.Keyboard:
                    logger.Info("Stop keyboard hook");
                    KeyboardHook?.StopHook();
                    break;
                case ActionType.Mouse:
                    logger.Info("Stop mouse hook");
                    MouseHook?.StopHook();
                    break;
                default:
                    logger.Info("Stop keyboard and mouse hook");
                    KeyboardHook?.StopHook();
                    MouseHook?.StopHook();
                    break;
            }
            KeyboardHook = null;
            MouseHook = null;
        }
    }
}