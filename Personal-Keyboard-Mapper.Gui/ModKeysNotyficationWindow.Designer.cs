
namespace Personal_Keyboard_Mapper.Gui
{
    partial class ModKeysNotificationWindow
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
            this.ModKeysGrid = new System.Windows.Forms.DataGridView();
            this.ModKeysColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.ModKeysGrid)).BeginInit();
            this.SuspendLayout();
            // 
            // ModKeysGrid
            // 
            this.ModKeysGrid.AllowUserToAddRows = false;
            this.ModKeysGrid.AllowUserToDeleteRows = false;
            this.ModKeysGrid.AllowUserToOrderColumns = true;
            this.ModKeysGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.ModKeysGrid.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.ModKeysColumn});
            this.ModKeysGrid.Location = new System.Drawing.Point(1, 12);
            this.ModKeysGrid.Name = "ModKeysGrid";
            this.ModKeysGrid.ReadOnly = true;
            this.ModKeysGrid.RowHeadersWidth = 51;
            this.ModKeysGrid.RowTemplate.Height = 24;
            this.ModKeysGrid.Size = new System.Drawing.Size(140, 121);
            this.ModKeysGrid.TabIndex = 0;
            // 
            // ModKeysColumn
            // 
            this.ModKeysColumn.HeaderText = "";
            this.ModKeysColumn.MinimumWidth = 6;
            this.ModKeysColumn.Name = "ModKeysColumn";
            this.ModKeysColumn.ReadOnly = true;
            this.ModKeysColumn.Width = 125;
            // 
            // ModKeysNotificationWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(148, 139);
            this.Controls.Add(this.ModKeysGrid);
            this.Name = "ModKeysNotificationWindow";
            this.Text = "ModKeysNotyficationWindow";
            ((System.ComponentModel.ISupportInitialize)(this.ModKeysGrid)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView ModKeysGrid;
        private System.Windows.Forms.DataGridViewTextBoxColumn ModKeysColumn;
    }
}