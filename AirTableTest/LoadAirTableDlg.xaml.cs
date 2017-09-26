using AirTableDB;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace AirTableTest
{
  /// <summary>
  /// Interaction logic for LoadAirTableDlg.xaml
  /// </summary>
  public partial class LoadAirTableDlg : Window
  {
    public LoadAirTableDlg()
    {
      InitializeComponent();
    }

    private void Window_Loaded(object sender, RoutedEventArgs e)
    {
      BackgroundWorker worker = new BackgroundWorker();
      worker.WorkerReportsProgress = true;
      worker.DoWork += LoadfromAirTable;
      worker.ProgressChanged += worker_ProgressChanged;

      worker.RunWorkerAsync();
    }

    void worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
    {
      pbStatus.Value = e.ProgressPercentage;
    }

    void LoadfromAirTable(object sender, DoWorkEventArgs e)
    {
      try
      {
        BackgroundWorker bw = sender as BackgroundWorker;

        string[] s_MatCategories = new string[] { "CP", "FB", "GS", "LM", "LT", "MT", "PT", "PV", "SS", "ST", "TL", "WC", "WD", "WP", "WDP" };
        string APPLICATION_ID = "appLyhgFjSLx0kVBv";

        List<ATTable> tables = new List<ATTable>();

        int index = 0;
        foreach (string matCat in s_MatCategories)
        {
          ATTable at = DownloadData.GetData(APPLICATION_ID, matCat);
          if (null != at && !at.IsEmpty())
          {
            tables.Add(at);
          }
          index++;
          int percengage = (int)(index * 20 / s_MatCategories.Length);
          bw.ReportProgress(percengage);
        }

        string jsFile = @"C:\Users\lyan\Source\Repos\airtable.net-master\AirTableTest\bin\Debug\file.json";
        string knFile = @"C:\Users\lyan\Source\Repos\airtable.net-master\AirTableTest\bin\Debug\KeyNote.txt";

        DownloadData.DownloadImages(tables, jsFile, @"C:\Users\lyan\Source\Repos\airtable.net-master\AirTableTest\bin\Debug\images\", bw);
        DownloadData.SaveKeynoteFile(tables, knFile);
        bw.ReportProgress(98);
        DownloadData.SaveJson(tables, jsFile);
        bw.ReportProgress(100);
      }
      catch (Exception ex)
      {
        Console.WriteLine(ex.ToString());
      }

      Console.WriteLine("GAME OVER");

      Console.Read();
    }
  }
}
