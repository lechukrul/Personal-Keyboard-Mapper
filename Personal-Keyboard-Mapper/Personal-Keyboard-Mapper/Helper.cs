using System.Windows.Forms;

namespace Personal_Keyboard_Mapper
{
    public class Helper
    {
        /// <summary>
        /// Adds the numeric rows to grid.
        /// </summary>
        /// <param name="dataGrid">The data grid.</param>
        public static void AddNumericRowsToGrid(DataGridView dataGrid)
        {
            for (int i = 0; i < 10; i++)
            {
                dataGrid.Rows.Add(i.ToString());
            }
        }

    }
}