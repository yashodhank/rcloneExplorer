﻿using System;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace rcloneExplorer
{
  public class rcloneExplorerTickHandler
  {
    IniFile iniSettings;
    rcloneExplorerMiscContainer miscContainer;
    rcloneExplorerExploreHandler exploreHandler;
    rcloneExplorerDownloadHandler downloadsHandler;
    rcloneExplorerUploadHandler uploadsHandler;
    rcloneExplorerSyncHandler syncingHandler;

    public void init()
    {
      iniSettings = rcloneExplorer.iniSettings;
      miscContainer = rcloneExplorer.miscContainer;
      exploreHandler = rcloneExplorer.exploreHandler;
      downloadsHandler = rcloneExplorer.downloadsHandler;
      uploadsHandler = rcloneExplorer.uploadsHandler;
      syncingHandler = rcloneExplorer.syncingHandler;
    }

    public void transferTimer_Tick(object sender, EventArgs e)
    {
        ListView lstDownloads = rcloneExplorer.myform.Controls.Find("lstDownloads", true)[0] as ListView;
        ListView lstUploads = rcloneExplorer.myform.Controls.Find("lstUploads", true)[0] as ListView;
        TabPage tabDownloads = rcloneExplorer.myform.Controls.Find("tabDownloads", true)[0] as TabPage;
        TabPage tabUploads = rcloneExplorer.myform.Controls.Find("tabUploads", true)[0] as TabPage;
        TextBox txtSyncLog = rcloneExplorer.myform.Controls.Find("txtSyncLog", true)[0] as TextBox;
        TextBox txtRawOut = rcloneExplorer.myform.Controls.Find("txtRawOut", true)[0] as TextBox;
        NotifyIcon notifyIcon = rcloneExplorer.notifyIconPub;

        if (!notifyIcon.Visible)
        {
            if (downloadsHandler.downloading.Count > 0)
            {
                for (var i = 0; i < downloadsHandler.downloading.Count; i++)
                {
                    {
                        //check downloadPId proces.exists to see if uploadis complete yet
                        int PID = Convert.ToInt32(downloadsHandler.downloading[i][0]);
                        if (miscContainer.ProcessExists(PID))
                        {
                            //download still in progress
                            lstDownloads.Items[i].SubItems[0].Text = downloadsHandler.downloading[i][0].ToString();
                            lstDownloads.Items[i].SubItems[2].Text = downloadsHandler.downloading[i][2];
                            lstDownloads.Items[i].SubItems[3].Text = downloadsHandler.downloading[i][3];
                        }
                        else
                        {
                            if (lstDownloads.Items[i].SubItems[2].Text != "Done!")
                            {
                                //download complete (guessing! probs best to validate this)
                                lstDownloads.Items[i].SubItems[2].Text = "100!";
                            }
                        }
                    }
                }
                if (tabDownloads.Text != "Downloads (" + lstDownloads.Items.Count + ")")
                {
                    tabDownloads.Text = "Downloads (" + lstDownloads.Items.Count + ")";
                }
            }
            if (uploadsHandler.uploading.Count > 0)
            {
                for (var i = 0; i < uploadsHandler.uploading.Count; i++)
                {
                    {

                        //store current iteration from list
                        string[] entry = uploadsHandler.uploading[i];
                        //entry filename
                        string uploadedFilename = entry[1];

                        //check downloadPId proces.exists to see if uploadis complete yet
                        int PID = Convert.ToInt32(uploadsHandler.uploading[i][0]);
                        if (miscContainer.ProcessExists(PID))
                        { 
                            //upload still in progress
                            //upload list should be {PID, Name, Percent, Speed}
                            lstUploads.Items[i].SubItems[0].Text = uploadsHandler.uploading[i][0].ToString();
                            lstUploads.Items[i].SubItems[2].Text = uploadsHandler.uploading[i][2];
                            lstUploads.Items[i].SubItems[3].Text = uploadsHandler.uploading[i][3];
                        }
                        else
                        {
                            if (lstUploads.Items[i].SubItems[2].Text != "Done!")
                            {
                                //upload complete (guessing! probs best to validate this)
                                lstUploads.Items[i].SubItems[2].Text = "Done!";
                                if (iniSettings.Read("refreshAutomatically") == "true")
                                {
                                    exploreHandler.refreshlstExplorer();
                                }
                            }
                        }
                    }
                }
                if (tabUploads.Text != "Uploads (" + lstUploads.Items.Count + ")")
                {
                    tabUploads.Text = "Uploads (" + lstUploads.Items.Count + ")";
                }
            }
            if (syncingHandler.syncingPID > 0)
            {
                if (File.Exists("sync.log"))
                {
                    using (var fs = new FileStream("sync.log", FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                    using (var sr = new StreamReader(fs, Encoding.Default))
                    {
                        string tmp = sr.ReadToEnd();
                        txtSyncLog.AppendText(tmp.Replace("\n", Environment.NewLine));
                    }
                }
            }
            if (rcloneExplorer.consoleEnabled)
            {
                txtRawOut.AppendText(rcloneExplorer.rawOutputBuffer);
                rcloneExplorer.rawOutputBuffer = "";
            }            
        }
        
    }
  }
}
