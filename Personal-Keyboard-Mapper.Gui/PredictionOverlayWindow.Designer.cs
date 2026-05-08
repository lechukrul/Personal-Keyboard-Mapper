namespace Personal_Keyboard_Mapper.Gui
{
    partial class PredictionOverlayWindow
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
                components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.predictionLabel = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // predictionLabel
            this.predictionLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.predictionLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 20F, System.Drawing.FontStyle.Bold);
            this.predictionLabel.ForeColor = System.Drawing.Color.White;
            this.predictionLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.predictionLabel.Name = "predictionLabel";
            // PredictionOverlayWindow
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(40, 40, 40);
            this.ClientSize = new System.Drawing.Size(240, 70);
            this.Controls.Add(this.predictionLabel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "PredictionOverlayWindow";
            this.Opacity = 0.85;
            this.ShowInTaskbar = false;
            this.TopMost = true;
            this.ResumeLayout(false);
        }

        private System.Windows.Forms.Label predictionLabel;
    }
}
