using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using log4net;
using Personal_Keyboard_Mapper.Gui;
using Personal_Keyboard_Mapper.Lib;
using Personal_Keyboard_Mapper.Lib.Enums;
using Personal_Keyboard_Mapper.Lib.Extensions;
using Personal_Keyboard_Mapper.Lib.Interfaces;
using Personal_Keyboard_Mapper.Lib.Model;
using Personal_Keyboard_Mapper.Lib.Prediction;
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
        static private HelpWindow helperWindow;
        private PredictionOverlayWindow predictionOverlayWindow;
        private WordPredictionService predictionService;

        public MainWindow(ILog log)
        {
            InitializeComponent();
            logger = log;
            helperWindow = new HelpWindow(logger);
            predictionOverlayWindow = new PredictionOverlayWindow(logger);
            predictionService = new WordPredictionService(
                System.IO.Path.Combine(Application.StartupPath, "word_frequencies.json"));
            predictionService.PredictionChanged += word => predictionOverlayWindow.SafeUpdatePrediction(word);
            configFileName = ConfigurationManager.AppSettings["DefaultConfigFileName"];
            Globals.AliasResources = new ResXResourceSet(ConfigurationManager.AppSettings["KeyAliasesResxFileName"]);
            Globals.GlobalResources = new ResXResourceSet(ConfigurationManager.AppSettings["GlobalResxFileName"]);
            keysSounds = new KeysSoundEffects(logger, Resources.Resources.key1, Resources.Resources.key2,
                Resources.Resources.ctrl, Resources.Resources.shift, Resources.Resources.win,
                Resources.Resources.alt);
            Globals.IsSoundOn = true;  
            existingConfigs = new List<string>();
            try
            {
                CollectExistingConfigs();
                if (!File.Exists(configFileName))
                { 
                    configFileName = existingConfigs[0] ?? "";
                }

                if (existingConfigs.Any())
                {
                    config = new JsonConfigSource(logger, configFileName);
                    hookService = new GlobalHookService(logger, config, keysSounds, true);
                    hookService.PredictionService = predictionService;
                }
                Helper.AddNumericRowsToGrid(combinationsTable);
            }
            catch (ArgumentOutOfRangeException outRangeException)
            {
                Helper.AddNumericRowsToGrid(combinationsTable);
                this.startAppBtn.Enabled = false;
                this.stopAppBtn.Enabled = false;
                this.EditConfigBtn.Enabled = false;
            }
            catch (Exception e)
            {
                logger.Error(e.StackTrace);
            }
        }
         

        private void On_Load(object sender, EventArgs args)
        {
            this.CombinationsPanel.Width = this.Width;
            this.combinationsTable.Width = this.Width;
            if (!existingConfigs.Any())
            {
                CollectExistingConfigs();
            }
            ExistingConfigsComboBox.DataSource = existingConfigs
                .ToArray();
            ExistingConfigsComboBox.SelectedIndex = existingConfigs.FindIndex(x => x == configFileName
                .Split('.')[0]);
            try
            {
                if (config != null)
                {
                    hookService.StartHookService(config, helperWindow);
                    startAppBtn.Enabled = false;
                }
                Helper.FillCombinationsTable(logger, this.combinationsTable, hookService?.combinationsConfig);
                RefreshPredictionComboBox();
            }
            catch (Exception e)
            {
                logger.Error(e.StackTrace);
            }
        }

        private void RefreshPredictionComboBox()
        {
            if (predictionCombinationComboBox == null) return;
            predictionCombinationComboBox.Items.Clear();

            if (hookService?.combinationsConfig?.Combinations == null) return;

            foreach (var combo in hookService.combinationsConfig.Combinations
                .Where(c => !c.IsNotEmptyActionCombination(logger))
                .OfType<TwoKeysCombination>())
            {
                predictionCombinationComboBox.Items.Add(
                    new EmptyCombinationItem(combo.FirstKeyVirtualCode, combo.SecondKeyVirtualCode));
            }

            if (predictionCombinationComboBox.Items.Count > 0)
            {
                predictionCombinationComboBox.SelectedIndex = 0;
                UpdateAcceptanceCombination();
            }
        }

        private void UpdateAcceptanceCombination()
        {
            if (predictionCombinationComboBox.SelectedItem is EmptyCombinationItem item)
                predictionService?.SetAcceptanceCombination(item.FirstKey, item.SecondKey);
        }

        private void PredictionChkBox_CheckedChanged(object sender, EventArgs e)
        {
            Globals.IsPredictionOn = predictionChkBox.Checked;
            predictionCombinationComboBox.Enabled = predictionChkBox.Checked;
            if (!predictionChkBox.Checked)
            {
                predictionService?.ClearBuffer();
                predictionOverlayWindow?.SafeUpdatePrediction(null);
            }
        }

        private void PredictionCombinationComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateAcceptanceCombination();
        }

        private class EmptyCombinationItem
        {
            public string FirstKey { get; }
            public string SecondKey { get; }

            public EmptyCombinationItem(string firstKey, string secondKey)
            {
                FirstKey = firstKey;
                SecondKey = secondKey;
            }

            public override string ToString() => $"{FirstKey} + {SecondKey}";
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
                    existingConfigs.Add(file.Split('\\').LastOrDefault()?.Split('.')[0]);
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
                configFileName = config.ConfigFilePath;
                if (hookService != null)
                {
                    hookService.StopHookService();
                    hookService.LoadCombinationsConfiguration(config);
                }
                else
                {
                    hookService = new GlobalHookService(logger, config, keysSounds, true);
                    hookService.PredictionService = predictionService;
                    hookService.LoadCombinationsConfiguration();
                }
                if (hookService.combinationsConfig != null && hookService.combinationsConfig.Combinations.Any())
                {
                    hookService.StartHookService(config, helperWindow);
                    Helper.FillCombinationsTable(logger, this.combinationsTable, hookService.combinationsConfig);
                    RefreshPredictionComboBox();
                    AddUpdateAppSettings("DefaultConfigFileName", config.ConfigFilePath);
                    if (existingConfigs.All(x => x != configFileName))
                    {
                        CollectExistingConfigs();
                        if (!existingConfigs.Any())
                        {
                            this.EditConfigBtn.Enabled = false;
                        }
                        else
                        {
                            this.EditConfigBtn.Enabled = true;
                        }
                    }

                    existingConfigs = existingConfigs.Distinct().ToList();
                    ExistingConfigsComboBox.DataSource = existingConfigs
                        .ToArray();
                    ExistingConfigsComboBox.SelectedIndex = existingConfigs.FindIndex(x => x == configFileName
                        .Split('.')[0]); 
                    this.startAppBtn.Enabled = false;
                    this.stopAppBtn.Enabled = true; 
                }
                else
                {
                    this.startAppBtn.Enabled = false;
                    this.stopAppBtn.Enabled = false;
                }
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
                hookService.StartHookService(config, helperWindow);
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
            try
            {
                var configEditor = new ConfigEditor(logger, this, hookService.combinationsConfig, (string)this.ExistingConfigsComboBox.SelectedItem);
                configEditor.Show();
            }
            catch (NullReferenceException)
            {
                MessageBox.Show(Globals.GlobalResources.GetString("EmptyConfiguratinMsg"));
            }
        }

        private void ExistingConfigsComboBox_SelectionChangeCommitted(object sender, EventArgs e)
        {
            var selectedConfig = (string)ExistingConfigsComboBox.SelectedItem;
            ReloadConfig(new JsonConfigSource(logger, selectedConfig));
        }

        private void HelpWndChckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (Globals.IsHelpWindowOn)
            {
                helperWindow.Hide();
            }

            Globals.IsHelpWindowOn = !Globals.IsHelpWindowOn;
        }

        private void OnClosing(object sender, CancelEventArgs e)
        {
            helperWindow.Close();
            predictionOverlayWindow?.Close();
        }

        private void MainWindow_Resize(object sender, EventArgs e)
        {
            this.CombinationsPanel.Width = this.Width;
            this.combinationsTable.Width = this.Width;
        }

        private void OptionsGroup_Enter(object sender, EventArgs e)
        {

        }
    }
}
