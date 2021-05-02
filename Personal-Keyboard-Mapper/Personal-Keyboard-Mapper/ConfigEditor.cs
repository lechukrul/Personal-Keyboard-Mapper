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
        private ResXResourceSet aliasResources;
        private string newConfigName;
        private KeyCombinationsConfiguration combinationsConfiguration;
        private List<IKeyCombination> newCombinations;
        private IKeyCombination currentCombination;
        private Array actionItems;
        private Array mouseActionItems;
        private int currentRowIndex;
        private int currentColumnIndex;
        private bool keyActionPanelVisible = true;
        private ComponentResourceManager resources;
        public ConfigEditor(ILog log, MainWindow form)
        {
            logger = log;
            mainWindow = form;
            resources = new ComponentResourceManager(typeof(ConfigEditor));
            aliasResources = new ResXResourceSet(ConfigurationManager.AppSettings["KeyAliasesResxFileName"]);
            newCombinations = new List<IKeyCombination>(); 
            actionItems = ActionComboBoxKeys();
            mouseActionItems = MouseActionsComboBoxItems(); 
            currentCombination = new TwoKeysCombination();
            combinationsConfiguration = new KeyCombinationsConfiguration();
            combinationsConfiguration.CombinationSize = 2;
            InitializeComponent();
        }
        public ConfigEditor(ILog log, MainWindow form, KeyCombinationsConfiguration configuration, string configFileName)
        {
            logger = log;
            mainWindow = form;
            resources = new ComponentResourceManager(typeof(ConfigEditor));
            aliasResources = Globals.AliasResources;
            combinationsConfiguration = configuration;
            actionItems = ActionComboBoxKeys();
            mouseActionItems = MouseActionsComboBoxItems();
            currentCombination = new TwoKeysCombination();
            newCombinations = configuration.Combinations.ToList();
            InitializeComponent();
            this.ConfigNameTxtBox.Text = configFileName.Split('.')[0];
            newConfigName = this.ConfigNameTxtBox.Text; 
        }

        private string[] ActionComboBoxKeys()
        {
            var alphaNumReg = new Regex("^VK_[a-zA-Z0-9]$");
            var functionKeysReg = new Regex("^F\\d\\d?");  
            var keyStrings = Enum.GetValues(typeof(VirtualKeyCode)).Cast<VirtualKeyCode>()
                .Where(x => alphaNumReg.IsMatch(x.ToString())
                            || functionKeysReg.IsMatch(x.ToString())
                            || Globals.NumericKeypadWithShiftVirtualKeyCodes.Contains(x)
                            || x == VirtualKeyCode.TAB
                            || x == VirtualKeyCode.DELETE
                            || x == VirtualKeyCode.ESCAPE)
                .Select(x => ((Keys)x).ToString())
                .ToArray();
            var result = keyStrings.Prepend("");
            return result.ToArray();
        }

        private string[] MouseActionsComboBoxItems()
        {
            return new []
            {
                aliasResources.GetString("leftMouseClick"),
                aliasResources.GetString("leftMouseDoubleClick"),
                aliasResources.GetString("rightMouseClick"),
                aliasResources.GetString("rightMouseDoubleClick"),
                aliasResources.GetString("leftMouseHoldClick"),
                aliasResources.GetString("rightMouseHoldClick")
            };
        }

        private void ConfigEditor_Load(object sender, EventArgs e)
        {
            ActionComboBox.DataSource = actionItems; 
            MouseActionComboBox.DataSource = mouseActionItems;
            ActionComboBox.SelectedIndex = 0;
            MouseActionComboBox.SelectedIndex = 0;
            ConfigNameTxtBox.Focus();
            Helper.AddNumericRowsToGrid(ConfigGrid);
            ConfigGrid.Width = this.Width;
            ConfigGroupBox.Width = this.Width;
            ShowConfigPanel(false);
            if (!string.IsNullOrEmpty(this.ConfigNameTxtBox.Text))
            {
                logger.Info("Load combination for edit");
                Helper.FillCombinationsTable(logger, this.ConfigGrid, combinationsConfiguration);
            }
        }
         
        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            AltCheckBox.Checked = false;
            CrtlCheckBox.Checked = false;
            ShiftCheckBox.Checked = false;
            KeyActionRadioBtn.Checked = true;
            TextActionRadioBtn.Checked = false;
            MouseActionRadioBtn.Checked = false;
            KeyActionPanel.Visible = true;
            TextActionPanel.Visible = false;
            MouseActionPanel.Visible = false;
            ActionComboBox.ResetText();
            MouseActionComboBox.ResetText();
            currentRowIndex = e.RowIndex;
            currentColumnIndex = e.ColumnIndex; 
            try
            {

                if (currentRowIndex >= 0 && currentColumnIndex >= 0)
                {
                    ShowConfigPanel(true);
                    var firstKey = new KeyboardKey(Globals.NumericVirtualKeyCodes.ElementAt(currentRowIndex), KeyCombinationPosition.First);
                    var SecondKey = new KeyboardKey(Globals.NumericVirtualKeyCodes.ElementAt(currentColumnIndex - 1), KeyCombinationPosition.Second);
                    currentCombination = new TwoKeysCombination();
                    currentCombination.Keys = new[] { firstKey, SecondKey };
                    currentCombination.logger = logger;
                }
            }
            catch (ArgumentOutOfRangeException )
            {
                logger.Warn("Row cell clicked");
            }

        }

        private void ConfigNameTxtBox_Leave(object sender, EventArgs e)
        {
            if (ConfigNameTxtBox.Text != String.Empty)
            {
                newConfigName = ConfigNameTxtBox.Text;
            }
        }

        /// <summary>
        /// Shows the configuration panel.
        /// </summary>
        /// <param name="show">if set to <c>true</c> [show].</param>
        private void ShowConfigPanel(bool show)
        {
            ConfigPanel.Visible = show;
        }

        private void SaveAction()
        {
            if (ConfigGrid.CurrentRow != null)
            {
                if (currentCombination?.Action != null)
                {
                    var modKeys = currentCombination.Action.GetActionModKeys();
                    var noModKeys = currentCombination.Action.GetActionNoModKeys();
                    var cellContent = currentCombination.Action.ToString();
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
                        CellUpdate(addedCombination, cellContent);
                    }
                    else
                    {
                        var editedConfig = newCombinations
                            .FirstOrDefault(x => x.Equals(addedCombination));
                        if (editedConfig != null)
                        {
                            editedConfig.Action = addedCombination.Action;  
                            CellUpdate(addedCombination, editedConfig.Action.ToString());
                        }
                    }
                    currentCombination.Clear();
                    MoveToNextCell();
                } 
            }
        }

        private void MoveToNextCell()
        {
            int iColumn = ConfigGrid.CurrentCell.ColumnIndex;
            int iRow = ConfigGrid.CurrentCell.RowIndex;
            if (iColumn == ConfigGrid.ColumnCount - 1)
            {
                if (ConfigGrid.RowCount > (iRow + 1))
                {
                    ConfigGrid.CurrentCell = ConfigGrid[1, iRow + 1];
                }
            }
            else
                ConfigGrid.CurrentCell = ConfigGrid[iColumn + 1, iRow];

            this.KeyActionRadioBtn.Checked = true;
            this.AltCheckBox.Checked = false;
            this.CrtlCheckBox.Checked = false;
            this.ShiftCheckBox.Checked = false;
            this.ActionComboBox.SelectedIndex = 0;
        }

        private void CellUpdate(TwoKeysCombination addedCombination, string cellContent)
        {
            var openCloseBrackets = Globals.OpenCloseBrackets
                .Where(x => x.keys.SequenceEqual(addedCombination.Action.VirtualKeys))
                .Select(x => x.sign)
                .FirstOrDefault();
            if (!string.IsNullOrEmpty(openCloseBrackets))
            {
                ConfigGrid.CurrentCell.Value = cellContent;
            }
            else
            {
                ConfigGrid.CurrentCell.Value = cellContent.TrimStart('-').ToLowerInvariant();
            }
        }

        private void KeyActionRadioBtn_CheckedChanged(object sender, EventArgs e)
        {
            if (KeyActionRadioBtn.Checked)
            {
                TextActionRadioBtn.Checked = false;
                MouseActionRadioBtn.Checked = false;
                KeyActionPanel.Visible = true;
                TextActionPanel.Visible = false;
                MouseActionPanel.Visible = false;
            } 
        }

        private void TextActionRadioBtn_CheckedChanged(object sender, EventArgs e)
        {
            if (TextActionRadioBtn.Checked)
            { 
                MouseActionRadioBtn.Checked = false; 
                KeyActionRadioBtn.Checked = false;
                KeyActionPanel.Visible = false;
                TextActionPanel.Visible = true;
                MouseActionPanel.Visible = false;
                ActionTextBox.Focus();
                ActionTextBox.Text = String.Empty;
            } 
        }
        private void MouseActionRadioBtn_CheckedChanged(object sender, EventArgs e)
        {
            if (MouseActionRadioBtn.Checked)
            {
                KeyActionRadioBtn.Checked = false; 
                TextActionRadioBtn.Checked = false; 
                KeyActionPanel.Visible = false;
                TextActionPanel.Visible = false;
                MouseActionPanel.Visible = true;
            } 
        }
        private void ActionComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (currentCombination == null)
            {
                throw new NullReferenceException(nameof(currentCombination));
            }
            try
            {

                var actionKey = (VirtualKeyCode)Enum.Parse(typeof(Keys), (string)ActionComboBox.SelectedItem);
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

                action.VirtualKeys = new List<VirtualKeyCode>() { actionKey };
                action.ActionStringKeys = new List<string>() { actionKey.ToString() };
                if (currentCombination.Action == null)
                {
                    currentCombination.Action = action;
                }
                else
                {
                    currentCombination.Action.VirtualKeys.AddRange(action.VirtualKeys);
                    currentCombination.Action.ActionStringKeys.AddRange(action.ActionStringKeys);
                }
            }
            catch (ArgumentException ae)
            { 
            }
        }

        private void ActionTextBox_Leave(object sender, EventArgs e)
        {
            SaveTextAction();
        }

        private void SaveTextAction()
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
                var stringKey = sign.ToString();
                if (sign == '\\')
                {
                    stringKey = ConfigurationManager.AppSettings["backslashAlias"];
                }
                action.ActionStringKeys.Add(stringKey);
            }

            if (Globals.Braces.Contains(string.Join("", textAction)))
            {
                action.VirtualKeys.Add(VirtualKeyCode.LEFT);
            }
            currentCombination.Action = action;
            SaveAction();
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
                        Type = ActionType.Keyboard,
                        VirtualKeys = new List<VirtualKeyCode>(),
                        ActionStringKeys = new List<string>()
                    };
                }
                currentCombination.Action.VirtualKeys.Add(VirtualKeyCode.LMENU);
                currentCombination.Action.ActionStringKeys.Add(ConfigurationManager.AppSettings["altAlias"]);
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
                        Type = ActionType.Keyboard,
                        VirtualKeys = new List<VirtualKeyCode>(),
                        ActionStringKeys = new List<string>()
                    };
                }
                currentCombination.Action.VirtualKeys.Add(VirtualKeyCode.CONTROL);
                currentCombination.Action.ActionStringKeys.Add(ConfigurationManager.AppSettings["crtlAlias"]);
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
                        Type = ActionType.Keyboard,
                        VirtualKeys = new List<VirtualKeyCode>(),
                        ActionStringKeys = new List<string>()
                    };
                }
                currentCombination.Action.VirtualKeys.Add(VirtualKeyCode.SHIFT);
                currentCombination.Action.ActionStringKeys.Add(ConfigurationManager.AppSettings["shiftAlias"]);
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
                logger.Error(nameof(newConfigName));
                MessageBox.Show(Globals.GlobalResources.GetString("ConfigNameEmpty"));
            }
            else
            {
                UpdateMouseActionsOutputs(newCombinations.Where(x => x.Action.IsMouseAction()));
                combinationsConfiguration.Combinations = newCombinations;
                try
                {
                    var newConfigFileName = newConfigName; 
                     
                    var configSource = new JsonConfigSource(logger);
                    configSource.WriteConfigToFile(combinationsConfiguration, newConfigFileName);
                    mainWindow.ReloadConfig(configSource);
                    this.Close();
                }
                catch (Exception ex)
                {
                    logger.Error(ex.StackTrace);
                }
            }
        }

        private void UpdateMouseActionsOutputs(IEnumerable<IKeyCombination> mouseCombinations)
        {
            foreach (var action in mouseCombinations.Select(x => x.Action))
            {
                var actionString = action.ToString();
                if (actionString == aliasResources.GetString("leftMouseClick"))
                {
                    action.ActionStringKeys.Clear();
                    action.ActionStringKeys.Add(ConfigurationManager.AppSettings["leftMouseClickAlias"]);
                }
                else if (actionString == aliasResources.GetString("rightMouseClick"))
                {
                    action.ActionStringKeys.Clear();
                    action.ActionStringKeys.Add(ConfigurationManager.AppSettings["rightMouseClickAlias"]);
                }
                else if (actionString == aliasResources.GetString("leftMouseDoubleClick"))
                {
                    action.ActionStringKeys.Clear();
                    action.ActionStringKeys.Add(ConfigurationManager.AppSettings["leftDoubleMouseClickAlias"]);
                }
                else if (actionString == aliasResources.GetString("rightMouseDoubleClick"))
                {
                    action.ActionStringKeys.Clear();
                    action.ActionStringKeys.Add(ConfigurationManager.AppSettings["rightDoubleMouseClickAlias"]);
                }
                else if (actionString == aliasResources.GetString("leftMouseHoldClick"))
                {
                    action.ActionStringKeys.Clear();
                    action.ActionStringKeys.Add(ConfigurationManager.AppSettings["leftHoldMouseClickAlias"]);
                }
                else if (actionString == aliasResources.GetString("rightMouseHoldClick"))
                {
                    action.ActionStringKeys.Clear();
                    action.ActionStringKeys.Add(ConfigurationManager.AppSettings["rightHoldMouseClickAlias"]);
                }
            }
        }

        private void MouseActionComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            var action = new Action()
            {
                Type = ActionType.Mouse,
                VirtualKeys = new List<VirtualKeyCode>(),
                ActionStringKeys = new List<string>()
            };
            if ((string)MouseActionComboBox.SelectedItem == aliasResources.GetString("leftMouseClick"))
            {
                action.VirtualKeys.Add(VirtualKeyCode.LBUTTON);
                action.ActionStringKeys.Add(ConfigurationManager.AppSettings["leftMouseClickAlias"]);
            }
            else if ((string)MouseActionComboBox.SelectedItem == aliasResources.GetString("leftMouseDoubleClick"))
            {
                action.VirtualKeys.Add(VirtualKeyCode.LBUTTON);
                action.VirtualKeys.Add(VirtualKeyCode.LBUTTON);
                action.ActionStringKeys.Add(ConfigurationManager.AppSettings["leftDoubleMouseClickAlias"]);
            }
            else if ((string)MouseActionComboBox.SelectedItem == aliasResources.GetString("leftMouseHoldClick"))
            {
                action.VirtualKeys.Add(VirtualKeyCode.LBUTTON);
                action.VirtualKeys.Add(VirtualKeyCode.LBUTTON);
                action.VirtualKeys.Add(VirtualKeyCode.LBUTTON);
                action.ActionStringKeys.Add(ConfigurationManager.AppSettings["leftHoldMouseClickAlias"]);
            }
            else if ((string)MouseActionComboBox.SelectedItem == aliasResources.GetString("rightMouseClick"))
            {
                action.VirtualKeys.Add(VirtualKeyCode.RBUTTON); 
                action.ActionStringKeys.Add(ConfigurationManager.AppSettings["rightMouseClickAlias"]);
            }
            else if ((string)MouseActionComboBox.SelectedItem == aliasResources.GetString("rightMouseDoubleClick"))
            {
                action.VirtualKeys.Add(VirtualKeyCode.RBUTTON);
                action.VirtualKeys.Add(VirtualKeyCode.RBUTTON);
                action.ActionStringKeys.Add(ConfigurationManager.AppSettings["rightDoubleMouseClickAlias"]);
            }
            else if ((string)MouseActionComboBox.SelectedItem == aliasResources.GetString("rightMouseHoldClick"))
            {
                action.VirtualKeys.Add(VirtualKeyCode.RBUTTON);
                action.VirtualKeys.Add(VirtualKeyCode.RBUTTON);
                action.VirtualKeys.Add(VirtualKeyCode.RBUTTON);
                action.ActionStringKeys.Add(ConfigurationManager.AppSettings["rightHoldMouseClickAlias"]);
            }
            currentCombination.Action = action;
            SaveAction();
        }

        private void saveActionBtn_Click(object sender, EventArgs e)
        {
            SaveAction();
        }

        private void ConfigEditor_Resize(object sender, EventArgs e)
        {
            this.ConfigGroupBox.Width = this.Width;
            this.ConfigGrid.Width = this.Width;
        }

        private void ActionTextBox_LostFocus(object sender, EventArgs e)
        {
            if (!this.SaveConfigBtn.Focused)
            {
                SaveTextAction();
            }
        }

        private void ConfigPanel_MouseClick(object sender, MouseEventArgs e)
        {
            this.Focus();
        }

        private void TextActionPanel_MouseClick(object sender, MouseEventArgs e)
        {
            SaveTextAction();
        }
    }
}
