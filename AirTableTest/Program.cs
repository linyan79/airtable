using AirtableApiClient;
using AirTableTest;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AirTableDB
{
  class Program
  {
    [STAThread]
    static void Main(string[] args)
    {
      //LoadAirTableDlg dlg = new LoadAirTableDlg();
      //dlg.ShowDialog();
      AirtableBase ab = new AirtableBase("keyvJVU4lZksGnWhI", "appQ1V53DWuMVNEAz");
      Uri uri = ab.BuildUriForListRecords("tblOyBDehWFloiXiJ", null, new string[] { "Base SKU" }, null, null, null, null, null);

      //private Uri BuildUriForListRecords(
      //      string tableName,
      //      string offset,
      //      IEnumerable<string> fields,
      //      string filterByFormula,
      //      int? maxRecords,
      //      int? pageSize,
      //      IEnumerable<Sort> sort,
      //      string view)
      //{
    }
  }
}
