using System.Collections.Generic;
using log4net.Repository.Hierarchy;
using Personal_Keyboard_Mapper.Lib.Model;
using System.Configuration;
using System.Linq;
using System.Windows.Forms;
using log4net;
using Personal_Keyboard_Mapper.Lib.Extensions;
using Personal_Keyboard_Mapper.Lib.Interfaces;

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

        /// <summary>
        /// Fills the combinations table.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="combinationsTable">The combinations table.</param>
        /// <param name="loadedConfiguration">The loaded configuration.</param>
        public static void FillCombinationsTable(ILog logger, DataGridView combinationsTable, KeyCombinationsConfiguration loadedConfiguration)
        {
            IEnumerable<IKeyCombination> combinations;
            if (loadedConfiguration == null)
            {
                logger.Warn(ConfigurationManager.AppSettings["NoConfigurationLoadedError"]);  
                combinations = new List<IKeyCombination>();
            }
            else
            { 
                combinations = loadedConfiguration.Combinations;
            }
            for (int i = 0; i < 10; i++)
            {
                for (int j = 0; j < 10; j++)
                {
                    combinationsTable.Rows[i].Cells[j + 1].Value = "";
                    var searchedCombination = combinations
                        .FirstOrDefault(x => x.HasKeys(i.ToString(), j.ToString()));
                    if (searchedCombination != null)
                    {
                        combinationsTable.Rows[i].Cells[j + 1].Value = string.Join(" ",
                            searchedCombination.Action.ToString());
                    }
                }
            }
        }
    }
}