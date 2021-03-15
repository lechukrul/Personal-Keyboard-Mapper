using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Windows.Forms;
using Personal_Keyboard_Mapper.Lib.Extensions;
using Personal_Keyboard_Mapper.Lib.Model;
using Personal_Keyboard_Mapper.Properties;

namespace Personal_Keyboard_Mapper
{
    partial class MainWindow
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainWindow));
            this.OptionsGroup = new System.Windows.Forms.GroupBox();
            this.HelpWndChckBox = new System.Windows.Forms.CheckBox();
            this.SoundChckBox = new System.Windows.Forms.CheckBox();
            this.CombinationsPanel = new System.Windows.Forms.GroupBox();
            this.EditConfigBtn = new System.Windows.Forms.Button();
            this.NewConfigBtn = new System.Windows.Forms.Button();
            this.combinationsTable = new System.Windows.Forms.DataGridView();
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
            this.stopAppBtn = new System.Windows.Forms.Button();
            this.startAppBtn = new System.Windows.Forms.Button();
            this.ExistingConfigsComboBox = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.OptionsGroup.SuspendLayout();
            this.CombinationsPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.combinationsTable)).BeginInit();
            this.SuspendLayout();
            // 
            // OptionsGroup
            // 
            this.OptionsGroup.Controls.Add(this.HelpWndChckBox);
            this.OptionsGroup.Controls.Add(this.SoundChckBox);
            resources.ApplyResources(this.OptionsGroup, "OptionsGroup");
            this.OptionsGroup.Name = "OptionsGroup";
            this.OptionsGroup.TabStop = false;
            // 
            // HelpWndChckBox
            // 
            resources.ApplyResources(this.HelpWndChckBox, "HelpWndChckBox");
            this.HelpWndChckBox.Name = "HelpWndChckBox";
            this.HelpWndChckBox.UseVisualStyleBackColor = true;
            // 
            // SoundChckBox
            // 
            resources.ApplyResources(this.SoundChckBox, "SoundChckBox");
            this.SoundChckBox.Checked = true;
            this.SoundChckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.SoundChckBox.Name = "SoundChckBox";
            this.SoundChckBox.UseVisualStyleBackColor = true;
            this.SoundChckBox.CheckedChanged += new System.EventHandler(this.checkBox1_CheckedChanged);
            // 
            // CombinationsPanel
            // 
            resources.ApplyResources(this.CombinationsPanel, "CombinationsPanel");
            this.CombinationsPanel.Controls.Add(this.EditConfigBtn);
            this.CombinationsPanel.Controls.Add(this.NewConfigBtn);
            this.CombinationsPanel.Controls.Add(this.combinationsTable);
            this.CombinationsPanel.Name = "CombinationsPanel";
            this.CombinationsPanel.TabStop = false;
            // 
            // EditConfigBtn
            // 
            resources.ApplyResources(this.EditConfigBtn, "EditConfigBtn");
            this.EditConfigBtn.Name = "EditConfigBtn";
            this.EditConfigBtn.UseVisualStyleBackColor = true;
            this.EditConfigBtn.Click += new System.EventHandler(this.EditConfigBtn_Click);
            // 
            // NewConfigBtn
            // 
            resources.ApplyResources(this.NewConfigBtn, "NewConfigBtn");
            this.NewConfigBtn.Name = "NewConfigBtn";
            this.NewConfigBtn.UseVisualStyleBackColor = true;
            this.NewConfigBtn.Click += new System.EventHandler(this.NewConfigBtn_Click);
            // 
            // combinationsTable
            // 
            this.combinationsTable.AllowUserToAddRows = false;
            this.combinationsTable.AllowUserToDeleteRows = false;
            resources.ApplyResources(this.combinationsTable, "combinationsTable");
            this.combinationsTable.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
            this.combinationsTable.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCells;
            this.combinationsTable.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.combinationsTable.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
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
            this.combinationsTable.Name = "combinationsTable";
            this.combinationsTable.ReadOnly = true;
            this.combinationsTable.RowTemplate.Height = 24;
            // 
            // Rows
            // 
            resources.ApplyResources(this.Rows, "Rows");
            this.Rows.Name = "Rows";
            this.Rows.ReadOnly = true;
            // 
            // Column0
            // 
            resources.ApplyResources(this.Column0, "Column0");
            this.Column0.Name = "Column0";
            this.Column0.ReadOnly = true;
            // 
            // Column1
            // 
            resources.ApplyResources(this.Column1, "Column1");
            this.Column1.Name = "Column1";
            this.Column1.ReadOnly = true;
            // 
            // Column2
            // 
            resources.ApplyResources(this.Column2, "Column2");
            this.Column2.Name = "Column2";
            this.Column2.ReadOnly = true;
            // 
            // Column3
            // 
            resources.ApplyResources(this.Column3, "Column3");
            this.Column3.Name = "Column3";
            this.Column3.ReadOnly = true;
            // 
            // Column4
            // 
            resources.ApplyResources(this.Column4, "Column4");
            this.Column4.Name = "Column4";
            this.Column4.ReadOnly = true;
            // 
            // Column5
            // 
            resources.ApplyResources(this.Column5, "Column5");
            this.Column5.Name = "Column5";
            this.Column5.ReadOnly = true;
            // 
            // Column6
            // 
            resources.ApplyResources(this.Column6, "Column6");
            this.Column6.Name = "Column6";
            this.Column6.ReadOnly = true;
            // 
            // Column7
            // 
            resources.ApplyResources(this.Column7, "Column7");
            this.Column7.Name = "Column7";
            this.Column7.ReadOnly = true;
            // 
            // Column8
            // 
            resources.ApplyResources(this.Column8, "Column8");
            this.Column8.Name = "Column8";
            this.Column8.ReadOnly = true;
            // 
            // Column9
            // 
            resources.ApplyResources(this.Column9, "Column9");
            this.Column9.Name = "Column9";
            this.Column9.ReadOnly = true;
            // 
            // stopAppBtn
            // 
            resources.ApplyResources(this.stopAppBtn, "stopAppBtn");
            this.stopAppBtn.Name = "stopAppBtn";
            this.stopAppBtn.UseVisualStyleBackColor = true;
            this.stopAppBtn.Click += new System.EventHandler(this.stopAppBtn_Click);
            // 
            // startAppBtn
            // 
            resources.ApplyResources(this.startAppBtn, "startAppBtn");
            this.startAppBtn.Name = "startAppBtn";
            this.startAppBtn.UseVisualStyleBackColor = true;
            this.startAppBtn.Click += new System.EventHandler(this.startAppBtn_Click);
            // 
            // ExistingConfigsComboBox
            // 
            this.ExistingConfigsComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.ExistingConfigsComboBox.FormattingEnabled = true;
            resources.ApplyResources(this.ExistingConfigsComboBox, "ExistingConfigsComboBox");
            this.ExistingConfigsComboBox.Name = "ExistingConfigsComboBox";
            this.ExistingConfigsComboBox.SelectionChangeCommitted += new System.EventHandler(this.ExistingConfigsComboBox_SelectionChangeCommitted);
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // MainWindow
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.label1);
            this.Controls.Add(this.ExistingConfigsComboBox);
            this.Controls.Add(this.startAppBtn);
            this.Controls.Add(this.stopAppBtn);
            this.Controls.Add(this.CombinationsPanel);
            this.Controls.Add(this.OptionsGroup);
            this.Name = "MainWindow";
            this.Load += new System.EventHandler(this.On_Load);
            this.OptionsGroup.ResumeLayout(false);
            this.OptionsGroup.PerformLayout();
            this.CombinationsPanel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.combinationsTable)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        #region declarations
        private System.Windows.Forms.GroupBox OptionsGroup;
        private System.Windows.Forms.CheckBox SoundChckBox;
        private System.Windows.Forms.CheckBox HelpWndChckBox;
        private System.Windows.Forms.GroupBox CombinationsPanel;
        private System.Windows.Forms.Button stopAppBtn;
        private System.Windows.Forms.Button startAppBtn;
        private System.Windows.Forms.DataGridView combinationsTable;
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
        private Button EditConfigBtn;
        private Button NewConfigBtn;
        #endregion

        private ComboBox ExistingConfigsComboBox;
        private Label label1;
    }
}

