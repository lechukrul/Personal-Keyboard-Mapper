using System;
using System.Drawing;
using System.Windows.Forms;
using log4net;

namespace Personal_Keyboard_Mapper.Gui
{
    public partial class PredictionOverlayWindow : Form
    {
        private ILog _logger;

        public PredictionOverlayWindow(ILog log)
        {
            _logger = log;
            InitializeComponent();
            PositionWindow();
        }

        protected override CreateParams CreateParams
        {
            get
            {
                var cp = base.CreateParams;
                cp.ExStyle |= 0x08000000; // WS_EX_NOACTIVATE
                return cp;
            }
        }

        public void SafeUpdatePrediction(string word)
        {
            if (InvokeRequired)
            {
                Invoke(new Action<string>(SafeUpdatePrediction), word);
                return;
            }

            if (string.IsNullOrEmpty(word))
            {
                predictionLabel.Text = string.Empty;
                Hide();
            }
            else
            {
                predictionLabel.Text = word;
                Show();
                TopMost = true;
            }
        }

        private void PositionWindow()
        {
            var screen = Screen.PrimaryScreen.WorkingArea;
            Location = new Point(screen.Right - Width - 10, screen.Height / 2 - Height / 2);
        }
    }
}
