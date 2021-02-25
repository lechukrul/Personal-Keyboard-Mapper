using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Resources;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using WindowsInput.Native;
using log4net;
using Personal_Keyboard_Mapper.Lib;
using Personal_Keyboard_Mapper.Lib.Comparers;
using Personal_Keyboard_Mapper.Lib.Converters;
using Personal_Keyboard_Mapper.Lib.Enums;
using Personal_Keyboard_Mapper.Lib.Interfaces;
using Personal_Keyboard_Mapper.Lib.Model;
using Action = Personal_Keyboard_Mapper.Lib.Model.Action;

namespace Personal_Keyboard_Mapper
{
    public partial class ConfigEditor : Form
    {
        private ILog logger;
        private MainWindow mainWindow;
        private string newConfigName;
        private KeyCombinationsConfiguration combinationsConfiguration;
        private List<IKeyCombination> newCombinations;
        private IKeyCombination currentCombination;
        private Array actionItems;
        private int currentRowIndex;
        private int currentColumnIndex;
        ComponentResourceManager resources = new ComponentResourceManager(typeof(ConfigEditor));
        public ConfigEditor(ILog log, MainWindow form)
        {
            logger = log;
            mainWindow = form;
            newCombinations = new List<IKeyCombination>();
            var keys = ActionComboBoxKeys();
            actionItems = keys;
            currentCombination = new TwoKeysCombination();
            combinationsConfiguration = new KeyCombinationsConfiguration();
            combinationsConfiguration.CombinationSize = 2;
            InitializeComponent();
        }
        public ConfigEditor(ILog log, MainWindow form, KeyCombinationsConfiguration configuration)
        {
            logger = log;
            mainWindow = form;
            combinationsConfiguration = configuration;
            var keys = ActionComboBoxKeys();
            actionItems = keys;
            currentCombination = new TwoKeysCombination();
            newCombinations = configuration.Combinations.ToList();
            InitializeComponent();
        }

        private static Keys[] ActionComboBoxKeys()
        {
            var alphaNumReg = new Regex("^VK_[a-zA-Z0-9]$");
            var functionKeysReg = new Regex("^F\\d\\d?");
            var keys = Enum.GetValues(typeof(VirtualKeyCode)).Cast<VirtualKeyCode>()
                .Where(x => alphaNumReg.IsMatch(x.ToString())
                            || functionKeysReg.IsMatch(x.ToString())
                            || Globals.ModKeysVirtualKeyCodes.Contains(x)
                            || Globals.NumericKeypadWithShiftVirtualKeyCodes.Contains(x)
                            || x == VirtualKeyCode.TAB
                            || x == VirtualKeyCode.DELETE
                            || x == VirtualKeyCode.ESCAPE)
                .Select(x => (Keys) x)
                .ToArray();
            return keys;
        }

        private void ConfigEditor_Load(object sender, EventArgs e)
        {
            ActionComboBox.DataSource = actionItems;
            ActionComboBox.DisplayMember = "A";
            Helper.AddNumericRowsToGrid(ConfigGrid);
            ShowConfigPanel(false);
        }

        private void FillRow()
        {
            var numItems = Globals.NumericKeypadVirtualKeyCodes
                .Select(x => x.ToString())
                .Select(x => x.Last())
                .ToArray();
            if (ConfigGrid.RowCount > 0 && ConfigGrid.SelectedRows[0] != null)
            {
                var firstKeyCell = (DataGridViewComboBoxCell) ConfigGrid.SelectedRows[0].Cells[0];
                var secondKeyCell = (DataGridViewComboBoxCell) ConfigGrid.SelectedRows[0].Cells[1];
            }
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            AltCheckBox.Checked = false;
            CrtlCheckBox.Checked = false;
            ShiftCheckBox.Checked = false;
            ActionComboBox.ResetText();
            currentRowIndex = e.RowIndex;
            currentColumnIndex = e.ColumnIndex;
            if (currentRowIndex >= 0 && currentColumnIndex >= 0)
            {
                ShowConfigPanel(true);
                var firstKey = new KeyboardKey(Globals.NumericKeypadVirtualKeyCodes.ElementAt(currentRowIndex), KeyCombinationPosition.First);
                var SecondKey = new KeyboardKey(Globals.NumericKeypadVirtualKeyCodes.ElementAt(currentColumnIndex), KeyCombinationPosition.First);
                currentCombination = new TwoKeysCombination();
                currentCombination.Keys = new[] { firstKey, SecondKey };
                currentCombination.logger = logger; 
            }

        }

        private void ConfigNameTxtBox_Leave(object sender, EventArgs e)
        {
            if (ConfigNameTxtBox.Text != String.Empty)
            {
                newConfigName = ConfigNameTxtBox.Text + ".keysconfig";
            }
        }

        /// <summary>
        /// Shows the configuration panel.
        /// </summary>
        /// <param name="show">if set to <c>true</c> [show].</param>
        private void ShowConfigPanel(bool show)
        {
            foreach (Control panelControl in ConfigPanel.Controls)
            {
                panelControl.Enabled = show;
            }
        }

        private void ConfigGrid_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
        }

        private void saveActionBtn_Click(object sender, EventArgs e)
        {
            if (currentCombination?.Action != null)
            {
                var modKeys = currentCombination.Action.GetActionModKeys();
                var noModKeys = currentCombination.Action.GetActionNoModKeys();
                var cellContent = $"{string.Join(" ", modKeys).Replace("LMENU", "ALT")}-" +
                                  $"{string.Join("+", noModKeys.Select(x => (Keys)x))}".TrimStart('-');
                var addedCombination = new TwoKeysCombination()
                {
                    Action = currentCombination.Action,
                    FirstKeyVirtualCode = ConfigGrid.CurrentCell.RowIndex.ToString(),
                    SecondKeyVirtualCode = (ConfigGrid.CurrentCell.ColumnIndex - 1).ToString(),
                    Keys = currentCombination.Keys,
                    logger = logger
                };
                if (!newCombinations.Contains(addedCombination, new KeyCombinationsComparer()))
                {
                    newCombinations.Add(addedCombination);
                    var openCloseBrackets = Globals.OpenCloseBrackets
                        .Where(x => x.keys.SequenceEqual(addedCombination.Action.VirtualKeys))
                        .Select(x => x.sign)
                        .FirstOrDefault();
                    if (!string.IsNullOrEmpty(openCloseBrackets))
                    {
                        ConfigGrid.CurrentCell.Value = openCloseBrackets;
                    }
                    else
                    {
                        ConfigGrid.CurrentCell.Value = cellContent;
                    }
                }
                else
                {
                    MessageBox.Show(Resources.Resources.CombinationExistsMsg);
                    logger.Error("Duplicate comb error");
                }
                currentCombination.Clear();
            }
        }

        private void KeyActionRadioBtn_CheckedChanged(object sender, EventArgs e)
        {
            TextActionRadioBtn.Checked = !KeyActionRadioBtn.Checked;
        }

        private void TextActionRadioBtn_CheckedChanged(object sender, EventArgs e)
        {
            KeyActionRadioBtn.Checked = !TextActionRadioBtn.Checked;
        }

        private void ActionComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (currentCombination == null)
            {
                throw new NullReferenceException(nameof(currentCombination));
            }
            var actionKey = (VirtualKeyCode)ActionComboBox.SelectedItem;
            var action = new Action();
            if (actionKey == VirtualKeyCode.MBUTTON || actionKey == VirtualKeyCode.LBUTTON ||
                actionKey == VirtualKeyCode.RBUTTON)
            {
                action.Type = ActionType.Mouse;
            }
            else
            {
                action.Type = ActionType.Keyboard;
            }

            action.VirtualKeys = new List<VirtualKeyCode>() {actionKey};
            action.ActionStringKeys = new List<string>(){actionKey.ToString()};
            currentCombination.Action = action;
        }

        private void ActionTextBox_Leave(object sender, EventArgs e)
        {
            var culture = CultureInfo.CurrentCulture;
            var textAction = ActionTextBox.Text.ToCharArray();
            var action = new Action()
            {
                Type = ActionType.Keyboard,
                VirtualKeys = new List<VirtualKeyCode>(),
                ActionStringKeys = new List<string>()
            };
            foreach (var sign in textAction)
            {
                var signVirtualCodes = OutputKeysConverter.CharToVirtualCode(sign, culture);
                action.VirtualKeys.AddRange(signVirtualCodes);
                action.ActionStringKeys.Add(sign.ToString());
            }
            currentCombination.Action = action;
        }

        private void AltCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (currentCombination == null)
            {
                throw new NullReferenceException(nameof(currentCombination));
            }
            if (AltCheckBox.Checked)
            {
                if (currentCombination.Action == null)
                {
                    currentCombination.Action = new Action
                    {
                        Type = ActionType.Keyboard
                    };
                }
                else
                {
                    currentCombination.Action.VirtualKeys.Add(VirtualKeyCode.LMENU);
                    currentCombination.Action.ActionStringKeys.Add(ConfigurationManager.AppSettings["altAlias"]);
                }
            }
            else
            {
                if (currentCombination.Action != null)
                {
                    if (currentCombination.Action.VirtualKeys.Contains(VirtualKeyCode.LMENU))
                    {
                        currentCombination.Action.VirtualKeys.RemoveAll(x => x == VirtualKeyCode.LMENU);
                        currentCombination.Action.ActionStringKeys.RemoveAll(x => x == ConfigurationManager.AppSettings["altAlias"]);
                    }
                }
            }
        }

        private void CrtlCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (currentCombination == null)
            {
                throw new NullReferenceException(nameof(currentCombination));
            }
            if (CrtlCheckBox.Checked)
            {
                if (currentCombination.Action == null)
                {
                    currentCombination.Action = new Action
                    {
                        Type = ActionType.Keyboard
                    };
                }
                else
                {
                    currentCombination.Action.VirtualKeys.Add(VirtualKeyCode.CONTROL);
                    currentCombination.Action.ActionStringKeys.Add(ConfigurationManager.AppSettings["crtlAlias"]);
                }
            }
            else
            {
                if (currentCombination.Action != null)
                {
                    if (currentCombination.Action.VirtualKeys.Contains(VirtualKeyCode.CONTROL))
                    {
                        currentCombination.Action.VirtualKeys.RemoveAll(x => x == VirtualKeyCode.CONTROL);
                        currentCombination.Action.ActionStringKeys.RemoveAll(x => x == ConfigurationManager.AppSettings["crtlAlias"]);
                    }
                }
            }
        }

        private void ShiftCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (currentCombination == null)
            {
                throw new NullReferenceException(nameof(currentCombination));
            }
            if (ShiftCheckBox.Checked)
            {
                if (currentCombination.Action == null)
                {
                    currentCombination.Action = new Action
                    {
                        Type = ActionType.Keyboard
                    };
                }
                else
                {
                    currentCombination.Action.VirtualKeys.Add(VirtualKeyCode.SHIFT);
                    currentCombination.Action.ActionStringKeys.Add(ConfigurationManager.AppSettings["shiftAlias"]);
                }
            }
            else
            {
                if (currentCombination.Action != null)
                {
                    if (currentCombination.Action.VirtualKeys.Contains(VirtualKeyCode.SHIFT))
                    {
                        currentCombination.Action.VirtualKeys.RemoveAll(x => x == VirtualKeyCode.SHIFT);
                        currentCombination.Action.ActionStringKeys.RemoveAll(x => x == ConfigurationManager.AppSettings["shiftAlias"]);
                    }
                }
            }
        }

        private void SaveConfigBtn_Click(object sender, EventArgs e)
        {
            if (combinationsConfiguration == null)
            {
                throw new NullReferenceException(nameof(combinationsConfiguration));
            }

            if (string.IsNullOrEmpty(newConfigName))
            {
                MessageBox.Show(Resources.Resources.ConfigNameEmpty);
                logger.Error(nameof(newConfigName));
            }
            else
            {
                combinationsConfiguration.Combinations = newCombinations;
                try
                {
                    if (!File.Exists(newConfigName))
                    {
                        File.Create(newConfigName);
                        logger.Info("Create new config file");
                    }
                    var configSource = new JsonConfigSource(logger);
                    configSource.WriteConfigToFile(combinationsConfiguration, newConfigName);
                    mainWindow.ReloadConfig(configSource);
                    this.Close();
                }
                catch (Exception ex)
                {
                    logger.Error(ex.StackTrace);
                }
            }
        }

    }
}
