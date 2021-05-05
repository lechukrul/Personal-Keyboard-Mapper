using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using log4net;

namespace Personal_Keyboard_Mapper.Gui
{
    public partial class HelpWindow : Form
    {
        private ILog logger;
        public HelpWindow(ILog log)
        {
            InitializeComponent();
            logger = log;
        }

        public void FillHelperRow(IEnumerable<string> possibleKeyActions)
        {
            if (possibleKeyActions == null)
            {
                logger.Error("Could not fill an help window, no data");   
            }

            if (possibleKeysTable.Rows.Count == 0)
            {
                possibleKeysTable.Rows.Add(new DataGridViewRow());
            }
            var cellIndex = 0;
            foreach (var action in possibleKeyActions)
            {
                possibleKeysTable.Rows[0].Cells[cellIndex].Value = possibleKeyActions.ToArray()[cellIndex++];
            }
        }

        public void ClearHelperRow()
        {
            if (possibleKeysTable.Rows.Count == 1)
            {
                foreach (DataGridViewCell cell in possibleKeysTable.Rows[0].Cells)
                {
                    cell.Value = "";
                }
            }
        }

    }
}
