using AirtableApiClient;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AirTableDB
{
  public class DownloadData
  {
    const string API_KEY = "keyvJVU4lZksGnWhI";         // airtable api key for tests

    public static ATTable GetData(string APPLICATION_ID, string TABLE_NAME)
    {
      string BASE_URL = $"https://api.airtable.com/v0/{APPLICATION_ID}/{Uri.EscapeUriString(TABLE_NAME)}";
      ATTable at = new ATTable(TABLE_NAME);
      try
      {
        AirtableBase airtableBase = new AirtableBase(API_KEY, APPLICATION_ID);  

        var task = airtableBase.ListRecords(TABLE_NAME);

        TaskHelper.RunTaskSynchronously(task);

        AirtableListRecordsResponse response = task.Result;

        if(null == response || response.Records == null)
        {
          return at;
        }

        foreach (AirtableRecord rd in response.Records)
        {
          int index = int.Parse(rd.GetField("Index").ToString());
          ATRow row = new ATRow(index, TABLE_NAME);

          IEnumerable<AirtableAttachment> attaches = rd.GetAttachmentField("Photo");
          if (null != attaches && attaches.Count() > 0)
          {
            row.PhotoAttach = attaches.First();
          }

          foreach (KeyValuePair<string, object> pair in rd.Fields)
          {
            if(pair.Key != "Index" && pair.Key != "Photo")
            {
              row.Descriptions.Add(pair.Value.ToString());
            }
          }

          if(!row.IsEmpty())
          {
            at.Rows.Add(row);
          }
        }
      }
      catch(Exception ex)
      {
        MessageBox.Show(ex.ToString());
      }
      return at;
    }

    public static void DownloadImages(List<ATTable> ats, string jFile, string folder, BackgroundWorker bw)
    {
      int start = 20;
      int current = start;
      int total = 75;
      try
      {
        Dictionary<string, Dictionary<string, AirtableAttachment>> oldAttaches = new Dictionary<string, Dictionary<string, AirtableAttachment>>();
        if (File.Exists(jFile))
        {
          List<ATTable> oldAts = LoadJson(jFile);
         
          foreach (ATTable at in oldAts)
          {
            Dictionary<string, AirtableAttachment> atDict = new Dictionary<string, AirtableAttachment>();
            oldAttaches.Add(at.TableName, atDict);
            foreach (ATRow row in at.Rows)
            {
              if (row.PhotoAttach != null)
              {
                atDict.Add(row.PhotoAttach.Id, row.PhotoAttach);
              }
            }
          }
        }

        start += (int)(total * 0.1);
        current = start;
        bw.ReportProgress(current);

        Dictionary<string, string> imgFiles = new Dictionary<string, string>();

        for(int i =0; i<ats.Count; i++)
        {
          ATTable at = ats[i];

          Dictionary<string, AirtableAttachment> oldAtDict = new Dictionary<string, AirtableAttachment>();
          oldAttaches.TryGetValue(at.TableName, out oldAtDict);

          foreach (ATRow row in at.Rows)
          {
            if (row.PhotoAttach == null || row.PhotoAttach.Filename == null)
            {
              continue;
            }

            try
            {            
              if (null == oldAtDict || oldAtDict.ContainsKey(row.PhotoAttach.Id) || oldAtDict[row.PhotoAttach.Id] != row.PhotoAttach)
              {

                string ext = Path.GetExtension(row.PhotoAttach.Filename);
                string subFolder = Path.Combine(folder, row.TableName);

                if (!Directory.Exists(subFolder))
                {
                  Directory.CreateDirectory(subFolder);
                }

                string localFilename = Path.Combine(subFolder, row.TableName + "_" + row.Index.ToString() + "." + ext);

                imgFiles.Add(localFilename, row.PhotoAttach.Url);
              }
            }
            catch(Exception e)
            {
              Console.WriteLine(e);
            }
          }
          current = start + (int)(total * 0.1 * i / ats.Count);
          bw.ReportProgress(current);
        }

        start = current;

        for(int i = 0; i < imgFiles.Count; i++)
        {
          KeyValuePair<string, string> img = imgFiles.ElementAt(i);
          try
          {
            if (File.Exists(img.Key))
            {
              File.Delete(img.Key);
            }

            using (WebClient client = new WebClient())
            {
              client.DownloadFile(img.Value, img.Key);
            }

            current = start + (int)(i * total * 0.8 / imgFiles.Count);
            bw.ReportProgress(current);
          }
          catch(Exception e)
          {
            Console.WriteLine(e);
          }
        }
      }
      catch(Exception ex)
      {
        Console.WriteLine(ex);
      }
    }

    public static void SaveJson(List<ATTable> ats, string file)
    {
      StreamWriter sw = null;
      try
      {
        if(File.Exists(file))
        {
          File.Delete(file);
        }

        string json = JsonConvert.SerializeObject(ats);

        sw = new StreamWriter(file);
        sw.WriteLine(json);
      }
      catch
      {
      }
      finally
      {
        if (null != sw)
        {
          sw.Close();
        }
      }
    }

    public static void SaveKeynoteFile(List<ATTable> ats, string file)
    {
      StreamWriter sw = null;
      try
      {
        if (File.Exists(file))
        {
          File.Delete(file);
        }
        sw = new StreamWriter(file);
        
        foreach(ATTable at in ats)
        {
          sw.WriteLine(at.TableName);
          foreach(ATRow row in at.Rows)
          {
            string desp = "TBD";
            if (row.Descriptions.Count > 0 && !string.IsNullOrEmpty(row.Descriptions[0]))
            {
              desp = row.Descriptions[0];
            }
            sw.WriteLine(row.TableName + "_" + row.Index.ToString() + "\t" + desp + "\t" +row.TableName);
          }
        }
      }
      catch
      {
      }
      finally
      {
        if (null != sw)
        {
          sw.Close();
        }
      }
    }

    public static List<ATTable> LoadJson(string file)
    {
      StreamReader sr = null;
      try
      {
        sr = new StreamReader(file);
        string jsTxt = sr.ReadToEnd();
        List<ATTable> ats = JsonConvert.DeserializeObject<List<ATTable>>(jsTxt);

        return ats;
      }
      catch
      {
      }
      finally
      {
        if (null != sr)
        {
          sr.Close();
        }
      }
      return null;
    }
  }

  public class ATTable
  {
    public string TableName { get; set; }
    public List<ATRow> Rows = new List<ATRow>();

    public string Description { get; set;}

    public ATTable(string nam)
    {
      TableName = nam;
    }

    public bool IsEmpty()
    {
      if(Rows.Count == 0)
      {
        return true;
      }

      foreach(ATRow row in Rows)
      {
        if(!row.IsEmpty())
        {
          return false;
        }
      }

      return true;
    }
  }

  public class ATRow
  {
    public int Index { get; set; }
    public AirtableAttachment PhotoAttach { get; set; }
    public List<string> Descriptions = new List<string>();

    public string TableName { get; set; }

    public ATRow(int i, string nam)
    {
      Index = i;
      TableName = nam;
    }

    public bool IsEmpty()
    {
      if(null == PhotoAttach)
      {
        return false;
      }
      if(Descriptions.Count ==0)
      {
        return true;
      }
      foreach(string desp in Descriptions)
      {
        if(!string.IsNullOrWhiteSpace(desp))
        {
          return false;
        }
      }
      return true;
    }
  }

  public static class TaskHelper
  {
    public static void RunTaskSynchronously(this Task t)
    {
      var task = Task.Run(async () => await t);
      task.Wait();
    }

    public static T RunTaskSynchronously<T>(this Task<T> t)
    {
      T res = default(T);
      var task = Task.Run(async () => res = await t);
      task.Wait();
      return res;
    }
  }
}
