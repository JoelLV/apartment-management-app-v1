using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Apt_Management_App.Repository
{
    internal class GeneralDatabaseInfo
    {
        public static List<string> GetTableNames()
        /*
         * Returns all the names of the tables
         * in the database to display in the
         * listbox.
         */
        {
            List<string> tableNames = new List<string>() { "Apartments", "Renters", "Payments", "Contracts", "Water Bills", "Electricity Bills" };
            tableNames.Sort();

            return tableNames;
        }
    }
}
