using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using log4net;
using Personal_Keyboard_Mapper.Lib;
using Personal_Keyboard_Mapper.Lib.Enums;
using Personal_Keyboard_Mapper.Lib.Interfaces;
using Personal_Keyboard_Mapper.Lib.Model;
using Personal_Keyboard_Mapper.Lib.Service;

namespace Personal_Keyboard_Mapper
{
    public partial class MainWindow : Form
    {
        private ILog logger;
        static private string configFileName;
        static private IConfigSource config;
        static private KeysSoundEffects keysSounds;
        static private GlobalHookService hookService;
        static private List<string> existingConfigs;
        public MainWindow(ILog log)
        {
            InitializeComponent();
            logger = log;
            configFileName = ConfigurationManager.AppSettings["DefaultConfigFileName"];
            keysSounds = new KeysSoundEffects(logger, Resources.Resources.key1, Resources.Resources.key2,
                Resources.Resources.ctrl, Resources.Resources.shift, Resources.Resources.win,
                Resources.Resources.alt);
            Globals.IsSoundOn = true;
            existingConfigs = new List<string>();
            if (!File.Exists(configFileName))
            {
                logger.Error("Config file is missing");
                MessageBox.Show("Config file is missing");
            }
            else
            {
                try
                {
                    config = new JsonConfigSource(logger, configFileName);
                    hookService = new GlobalHookService(logger, config, keysSounds, true);
                    Helper.AddNumericRowsToGrid(combinationsTable);
                }
                catch (Exception e)
                {
                    logger.Error(e.StackTrace);
                }
            }
        }

        private void On_Load(object sender, EventArgs args)
        {
            CollectExistingConfigs();
            ExistingConfigsComboBox.DataSource = existingConfigs.ToArray(); 
            try
            {
                hookService.StartHookService(config);
                Helper.FillCombinationsTable(logger, this.combinationsTable, hookService.combinationsConfig);
                startAppBtn.Enabled = false; 
            }
            catch (Exception e)
            {
                logger.Error(e.StackTrace);
            }
        }

        /// <summary>
        /// Collects the existing configs.
        /// </summary>
        private void CollectExistingConfigs()
        {
            foreach (var file in Directory.EnumerateFiles(Directory.GetCurrentDirectory()))
            {
                if (file.Split('.')[1] == "keysconfig")
                {
                    existingConfigs.Add(file.Split('\\').LastOrDefault());
                }
            }
        }

        /// <summary>
        /// Reloads the configuration.
        /// </summary>
        /// <param name="newConfig">New configuration</param>
        public void ReloadConfig(IConfigSource newConfig)
        {
            try
            {
                logger.Info("START CONFIG RELOAD");
                config = newConfig;
                if (hookService != null)
                {
                    hookService.StopHookService(); 
                }
                else
                {
                    hookService = new GlobalHookService(logger, config, keysSounds, true);
                }
                hookService.StartHookService(config);
                Helper.FillCombinationsTable(logger, this.combinationsTable, hookService.combinationsConfig);
                AddUpdateAppSettings("DefaultConfigFileName", config.ConfigFilePath);
                logger.Info("CONFIG RELOAD SUCCESSFULLY ENDS");
            }
            catch (Exception e)
            {
                logger.Error("CONFIG RELOAD ERROR");
                logger.Error(e.StackTrace);
                this.Close();
            }
        }

        private void AddUpdateAppSettings(string key, string value)
        {
            try
            {
                var configFile = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                var settings = configFile.AppSettings.Settings;
                if (settings[key] == null)
                {
                    settings.Add(key, value);
                }
                else
                {
                    settings[key].Value = value;
                }
                configFile.Save(ConfigurationSaveMode.Modified);
                ConfigurationManager.RefreshSection(configFile.AppSettings.SectionInformation.Name);
            }
            catch (ConfigurationErrorsException e)
            {
                logger.Error("Error writing app settings");
                logger.Error(e.StackTrace);
            }
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            Globals.IsSoundOn = !Globals.IsSoundOn;
        }

        private void loadConfigBtn_Click(object sender, EventArgs e)
        {

        }

        private void startAppBtn_Click(object sender, EventArgs e)
        {
            try
            {
                hookService.StartHookService();
                stopAppBtn.Enabled = true;
                startAppBtn.Enabled = false;
            }
            catch (Exception exception)
            {
                logger.Error(exception.StackTrace);
            }
        }

        private void stopAppBtn_Click(object sender, EventArgs e)
        {
            hookService.StopHookService();
            stopAppBtn.Enabled = false;
            startAppBtn.Enabled = true;
        }

        private void NewConfigBtn_Click(object sender, EventArgs e)
        {
            var configEditor = new ConfigEditor(logger, this);
            configEditor.Show();
        }

        private void EditConfigBtn_Click(object sender, EventArgs e)
        {
            var configEditor = new ConfigEditor(logger, this, hookService.combinationsConfig, (string)this.ExistingConfigsComboBox.SelectedItem);
            configEditor.Show();
        }

        private void ExistingConfigsComboBox_SelectionChangeCommitted(object sender, EventArgs e)
        {
            var selectedConfig = (string)ExistingConfigsComboBox.SelectedItem;
            ReloadConfig(new JsonConfigSource(logger, selectedConfig));
        }
    }
}
