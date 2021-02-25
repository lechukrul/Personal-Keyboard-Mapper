using System;
using System.Linq;
using Personal_Keyboard_Mapper.Lib;

namespace Personal_Keyboard_Mapper
{
    partial class ConfigEditor
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.ConfigNameLabel = new System.Windows.Forms.Label();
            this.ConfigGroupBox = new System.Windows.Forms.GroupBox();
            this.ConfigGrid = new System.Windows.Forms.DataGridView();
            this.Rows = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column0 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column3 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column4 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column5 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column6 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column7 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column8 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column9 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ConfigPanel = new System.Windows.Forms.Panel();
            this.saveActionBtn = new System.Windows.Forms.Button();
            this.panel2 = new System.Windows.Forms.Panel();
            this.ActionTextBox = new System.Windows.Forms.TextBox();
            this.TextActionRadioBtn = new System.Windows.Forms.RadioButton();
            this.panel1 = new System.Windows.Forms.Panel();
            this.ActionComboBox = new System.Windows.Forms.ComboBox();
            this.CrtlCheckBox = new System.Windows.Forms.CheckBox();
            this.AltCheckBox = new System.Windows.Forms.CheckBox();
            this.ShiftCheckBox = new System.Windows.Forms.CheckBox();
            this.KeyActionRadioBtn = new System.Windows.Forms.RadioButton();
            this.ConfigNameTxtBox = new Personal_Keyboard_Mapper.Custom_Controls.PlaceHolderTextBox();
            this.windowsInputDeviceStateAdaptorBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.inputSimulatorBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.SaveConfigBtn = new System.Windows.Forms.Button();
            this.ConfigGroupBox.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ConfigGrid)).BeginInit();
            this.ConfigPanel.SuspendLayout();
            this.panel2.SuspendLayout();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.windowsInputDeviceStateAdaptorBindingSource)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.inputSimulatorBindingSource)).BeginInit();
            this.SuspendLayout();
            // 
            // ConfigNameLabel
            // 
            this.ConfigNameLabel.AutoSize = true;
            this.ConfigNameLabel.Location = new System.Drawing.Point(50, 38);
            this.ConfigNameLabel.Name = "ConfigNameLabel";
            this.ConfigNameLabel.Size = new System.Drawing.Size(131, 17);
            this.ConfigNameLabel.TabIndex = 1;
            this.ConfigNameLabel.Text = "Configuration name";
            // 
            // ConfigGroupBox
            // 
            this.ConfigGroupBox.Controls.Add(this.ConfigGrid);
            this.ConfigGroupBox.Location = new System.Drawing.Point(1, 255);
            this.ConfigGroupBox.Name = "ConfigGroupBox";
            this.ConfigGroupBox.Size = new System.Drawing.Size(942, 350);
            this.ConfigGroupBox.TabIndex = 2;
            this.ConfigGroupBox.TabStop = false;
            this.ConfigGroupBox.Text = "Configuration";
            // 
            // ConfigGrid
            // 
            this.ConfigGrid.AllowUserToAddRows = false;
            this.ConfigGrid.AllowUserToDeleteRows = false;
            this.ConfigGrid.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
            this.ConfigGrid.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCells;
            this.ConfigGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.ConfigGrid.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Rows,
            this.Column0,
            this.Column1,
            this.Column2,
            this.Column3,
            this.Column4,
            this.Column5,
            this.Column6,
            this.Column7,
            this.Column8,
            this.Column9});
            this.ConfigGrid.Location = new System.Drawing.Point(13, 33);
            this.ConfigGrid.Name = "ConfigGrid";
            this.ConfigGrid.RowHeadersWidth = 51;
            this.ConfigGrid.RowTemplate.Height = 24;
            this.ConfigGrid.Size = new System.Drawing.Size(923, 290);
            this.ConfigGrid.TabIndex = 0;
            this.ConfigGrid.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView1_CellClick);
            // 
            // Rows
            // 
            this.Rows.HeaderText = "";
            this.Rows.MinimumWidth = 6;
            this.Rows.Name = "Rows";
            this.Rows.ReadOnly = true;
            this.Rows.Width = 23;
            // 
            // Column0
            // 
            this.Column0.HeaderText = "0";
            this.Column0.MinimumWidth = 6;
            this.Column0.Name = "Column0";
            this.Column0.ReadOnly = true;
            this.Column0.Width = 45;
            // 
            // Column1
            // 
            this.Column1.HeaderText = "1";
            this.Column1.MinimumWidth = 6;
            this.Column1.Name = "Column1";
            this.Column1.ReadOnly = true;
            this.Column1.Width = 45;
            // 
            // Column2
            // 
            this.Column2.HeaderText = "2";
            this.Column2.MinimumWidth = 6;
            this.Column2.Name = "Column2";
            this.Column2.ReadOnly = true;
            this.Column2.Width = 45;
            // 
            // Column3
            // 
            this.Column3.HeaderText = "3";
            this.Column3.MinimumWidth = 6;
            this.Column3.Name = "Column3";
            this.Column3.ReadOnly = true;
            this.Column3.Width = 45;
            // 
            // Column4
            // 
            this.Column4.HeaderText = "4";
            this.Column4.MinimumWidth = 6;
            this.Column4.Name = "Column4";
            this.Column4.ReadOnly = true;
            this.Column4.Width = 45;
            // 
            // Column5
            // 
            this.Column5.HeaderText = "5";
            this.Column5.MinimumWidth = 6;
            this.Column5.Name = "Column5";
            this.Column5.ReadOnly = true;
            this.Column5.Width = 45;
            // 
            // Column6
            // 
            this.Column6.HeaderText = "6";
            this.Column6.MinimumWidth = 6;
            this.Column6.Name = "Column6";
            this.Column6.ReadOnly = true;
            this.Column6.Width = 45;
            // 
            // Column7
            // 
            this.Column7.HeaderText = "7";
            this.Column7.MinimumWidth = 6;
            this.Column7.Name = "Column7";
            this.Column7.ReadOnly = true;
            this.Column7.Width = 45;
            // 
            // Column8
            // 
            this.Column8.HeaderText = "8";
            this.Column8.MinimumWidth = 6;
            this.Column8.Name = "Column8";
            this.Column8.ReadOnly = true;
            this.Column8.Width = 45;
            // 
            // Column9
            // 
            this.Column9.HeaderText = "9";
            this.Column9.MinimumWidth = 6;
            this.Column9.Name = "Column9";
            this.Column9.ReadOnly = true;
            this.Column9.Width = 45;
            // 
            // ConfigPanel
            // 
            this.ConfigPanel.Controls.Add(this.saveActionBtn);
            this.ConfigPanel.Controls.Add(this.panel2);
            this.ConfigPanel.Controls.Add(this.TextActionRadioBtn);
            this.ConfigPanel.Controls.Add(this.panel1);
            this.ConfigPanel.Controls.Add(this.KeyActionRadioBtn);
            this.ConfigPanel.Location = new System.Drawing.Point(53, 77);
            this.ConfigPanel.Name = "ConfigPanel";
            this.ConfigPanel.Size = new System.Drawing.Size(603, 172);
            this.ConfigPanel.TabIndex = 3;
            // 
            // saveActionBtn
            // 
            this.saveActionBtn.Location = new System.Drawing.Point(276, 146);
            this.saveActionBtn.Name = "saveActionBtn";
            this.saveActionBtn.Size = new System.Drawing.Size(93, 23);
            this.saveActionBtn.TabIndex = 8;
            this.saveActionBtn.Text = "save action";
            this.saveActionBtn.UseVisualStyleBackColor = true;
            this.saveActionBtn.Click += new System.EventHandler(this.saveActionBtn_Click);
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.ActionTextBox);
            this.panel2.Location = new System.Drawing.Point(384, 55);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(153, 79);
            this.panel2.TabIndex = 7;
            // 
            // ActionTextBox
            // 
            this.ActionTextBox.Location = new System.Drawing.Point(0, 25);
            this.ActionTextBox.Name = "ActionTextBox";
            this.ActionTextBox.Size = new System.Drawing.Size(144, 22);
            this.ActionTextBox.TabIndex = 0;
            this.ActionTextBox.Leave += new System.EventHandler(this.ActionTextBox_Leave);
            // 
            // TextActionRadioBtn
            // 
            this.TextActionRadioBtn.AutoSize = true;
            this.TextActionRadioBtn.Location = new System.Drawing.Point(384, 24);
            this.TextActionRadioBtn.Name = "TextActionRadioBtn";
            this.TextActionRadioBtn.Size = new System.Drawing.Size(98, 21);
            this.TextActionRadioBtn.TabIndex = 6;
            this.TextActionRadioBtn.Text = "Text action";
            this.TextActionRadioBtn.UseVisualStyleBackColor = true;
            this.TextActionRadioBtn.CheckedChanged += new System.EventHandler(this.TextActionRadioBtn_CheckedChanged);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.ActionComboBox);
            this.panel1.Controls.Add(this.CrtlCheckBox);
            this.panel1.Controls.Add(this.AltCheckBox);
            this.panel1.Controls.Add(this.ShiftCheckBox);
            this.panel1.Location = new System.Drawing.Point(61, 52);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(201, 83);
            this.panel1.TabIndex = 5;
            // 
            // ActionComboBox
            // 
            this.ActionComboBox.FormattingEnabled = true;
            this.ActionComboBox.Location = new System.Drawing.Point(0, 30);
            this.ActionComboBox.Name = "ActionComboBox";
            this.ActionComboBox.Size = new System.Drawing.Size(121, 24);
            this.ActionComboBox.TabIndex = 1;
            this.ActionComboBox.SelectedIndexChanged += new System.EventHandler(this.ActionComboBox_SelectedIndexChanged);
            // 
            // CrtlCheckBox
            // 
            this.CrtlCheckBox.AutoSize = true;
            this.CrtlCheckBox.Location = new System.Drawing.Point(127, 30);
            this.CrtlCheckBox.Name = "CrtlCheckBox";
            this.CrtlCheckBox.Size = new System.Drawing.Size(66, 21);
            this.CrtlCheckBox.TabIndex = 4;
            this.CrtlCheckBox.Text = "CRTL";
            this.CrtlCheckBox.UseVisualStyleBackColor = true;
            this.CrtlCheckBox.CheckedChanged += new System.EventHandler(this.CrtlCheckBox_CheckedChanged);
            // 
            // AltCheckBox
            // 
            this.AltCheckBox.AutoSize = true;
            this.AltCheckBox.Location = new System.Drawing.Point(127, 3);
            this.AltCheckBox.Name = "AltCheckBox";
            this.AltCheckBox.Size = new System.Drawing.Size(56, 21);
            this.AltCheckBox.TabIndex = 2;
            this.AltCheckBox.Text = "ALT";
            this.AltCheckBox.UseVisualStyleBackColor = true;
            this.AltCheckBox.CheckedChanged += new System.EventHandler(this.AltCheckBox_CheckedChanged);
            // 
            // ShiftCheckBox
            // 
            this.ShiftCheckBox.AutoSize = true;
            this.ShiftCheckBox.Location = new System.Drawing.Point(127, 61);
            this.ShiftCheckBox.Name = "ShiftCheckBox";
            this.ShiftCheckBox.Size = new System.Drawing.Size(69, 21);
            this.ShiftCheckBox.TabIndex = 3;
            this.ShiftCheckBox.Text = "SHIFT";
            this.ShiftCheckBox.UseVisualStyleBackColor = true;
            this.ShiftCheckBox.CheckedChanged += new System.EventHandler(this.ShiftCheckBox_CheckedChanged);
            // 
            // KeyActionRadioBtn
            // 
            this.KeyActionRadioBtn.AutoSize = true;
            this.KeyActionRadioBtn.Checked = true;
            this.KeyActionRadioBtn.Location = new System.Drawing.Point(61, 24);
            this.KeyActionRadioBtn.Name = "KeyActionRadioBtn";
            this.KeyActionRadioBtn.Size = new System.Drawing.Size(124, 21);
            this.KeyActionRadioBtn.TabIndex = 0;
            this.KeyActionRadioBtn.TabStop = true;
            this.KeyActionRadioBtn.Text = "One key action";
            this.KeyActionRadioBtn.UseVisualStyleBackColor = true;
            this.KeyActionRadioBtn.CheckedChanged += new System.EventHandler(this.KeyActionRadioBtn_CheckedChanged);
            // 
            // ConfigNameTxtBox
            // 
            this.ConfigNameTxtBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F, System.Drawing.FontStyle.Italic);
            this.ConfigNameTxtBox.ForeColor = System.Drawing.Color.Gray;
            this.ConfigNameTxtBox.Location = new System.Drawing.Point(199, 35);
            this.ConfigNameTxtBox.Name = "ConfigNameTxtBox";
            this.ConfigNameTxtBox.PlaceHolderText = null;
            this.ConfigNameTxtBox.Size = new System.Drawing.Size(100, 22);
            this.ConfigNameTxtBox.TabIndex = 0;
            this.ConfigNameTxtBox.Leave += new System.EventHandler(this.ConfigNameTxtBox_Leave);
            // 
            // SaveConfigBtn
            // 
            this.SaveConfigBtn.Location = new System.Drawing.Point(307, 611);
            this.SaveConfigBtn.Name = "SaveConfigBtn";
            this.SaveConfigBtn.Size = new System.Drawing.Size(140, 41);
            this.SaveConfigBtn.TabIndex = 4;
            this.SaveConfigBtn.Text = "Save Configuration";
            this.SaveConfigBtn.UseVisualStyleBackColor = true;
            this.SaveConfigBtn.Click += new System.EventHandler(this.SaveConfigBtn_Click);
            // 
            // ConfigEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(955, 683);
            this.Controls.Add(this.SaveConfigBtn);
            this.Controls.Add(this.ConfigPanel);
            this.Controls.Add(this.ConfigGroupBox);
            this.Controls.Add(this.ConfigNameLabel);
            this.Controls.Add(this.ConfigNameTxtBox);
            this.Name = "ConfigEditor";
            this.Text = "ConfigEditor";
            this.Load += new System.EventHandler(this.ConfigEditor_Load);
            this.ConfigGroupBox.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.ConfigGrid)).EndInit();
            this.ConfigPanel.ResumeLayout(false);
            this.ConfigPanel.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.windowsInputDeviceStateAdaptorBindingSource)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.inputSimulatorBindingSource)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Custom_Controls.PlaceHolderTextBox ConfigNameTxtBox;
        private System.Windows.Forms.Label ConfigNameLabel;
        private System.Windows.Forms.GroupBox ConfigGroupBox;
        private System.Windows.Forms.DataGridView ConfigGrid;
        private System.Windows.Forms.DataGridViewTextBoxColumn Rows;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column0;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column1;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column2;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column3;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column4;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column5;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column6;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column7;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column8;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column9;
        private System.Windows.Forms.Panel ConfigPanel;
        private System.Windows.Forms.CheckBox CrtlCheckBox;
        private System.Windows.Forms.CheckBox ShiftCheckBox;
        private System.Windows.Forms.CheckBox AltCheckBox;
        private System.Windows.Forms.ComboBox ActionComboBox;
        private System.Windows.Forms.RadioButton KeyActionRadioBtn;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.TextBox ActionTextBox;
        private System.Windows.Forms.RadioButton TextActionRadioBtn;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button saveActionBtn;
        private System.Windows.Forms.BindingSource windowsInputDeviceStateAdaptorBindingSource;
        private System.Windows.Forms.BindingSource inputSimulatorBindingSource;
        private System.Windows.Forms.Button SaveConfigBtn;
    }
}