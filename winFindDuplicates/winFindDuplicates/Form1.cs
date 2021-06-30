using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Threading;

namespace winFindDuplicates
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            listView1.Items.Clear();
            listView1.Groups.Clear();
            filenames = new List<string>();
            myFiles = new List<fileobs>();
            myLengths = new Dictionary<long, int>();
            myhash = new Dictionary<string, int>();
            bgw.RunWorkerAsync(@"F:\My Books2|F:\My Books|F:\Downloads.Ginger");
        }


        List<string> filenames;
        List<fileobs> myFiles;
        Dictionary<long, int> myLengths = new Dictionary<long, int>();
        Dictionary<string, int> myhash = new Dictionary<string, int>();
        private void bgw_DoWork(object sender, DoWorkEventArgs e)
        {
            filenames = new List<string>();
            DoDirectory(e.Argument.ToString());
            myFiles = new List<fileobs>();
            myFiles = (from o in filenames select new fileobs(o)).ToList();
            var m = myFiles.OrderBy(x => x.filesize).ToList();
            foreach (var r in m)
            {
                if (myLengths.ContainsKey(r.filesize)) myLengths[r.filesize]++;
                else myLengths.Add(r.filesize, 1);
            }

            var q = from ml in myLengths
                    where ml.Value > 1
                    select ml;
            var w = (from ml in q
                     join mf in m on ml.Key equals mf.filesize
                     select mf).OrderBy(x => x.filesize).ToList();
            int ilen = w.Count;
            int ilp = 0;
            int paircount = 0;
            foreach (var r in w)
            {
                ilp++;

                bgw.ReportProgress((int)(ilp * 100 / ilen), r.filename);
                if (r.filename.Length < 260 && Path.GetExtension(r.filename).ToLower() == ".pdf")
                {
                    var tt = new MD5(r.filename);
                    Thread th = new Thread(tt.GetHash);
                    th.Start();
                    while (!(tt.Current == -1))
                    {
                        Application.DoEvents();
                        Thread.Sleep(0);
                    }
                    r.hash = tt.Hash;
                    if (myhash.ContainsKey(r.hash)) myhash[r.hash]++;
                    else
                    {
                        myhash.Add(r.hash, 1);
                        paircount++;
                        if (paircount > 100) break;
                    }

                }
            }
            var filegroups = (from hp in myhash
                              join mf in myFiles on hp.Key equals mf.hash
                              where hp.Value > 1
                              select mf).ToList();
            e.Result = filegroups;
        }

        private void DoDirectory(string s)
        {
            foreach (string ss in s.Split('|'))
                try
                {
                    filenames.AddRange(Directory.GetFiles(ss));
                    bgw.ReportProgress(1);
                    foreach (string j in Directory.GetDirectories(ss))
                        DoDirectory(j);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }

        }
        private void bgw_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            if (e.ProgressPercentage == 1) label1.Text = filenames.Count.ToString("#,##0");
            else
            {
                progressBar1.Value = e.ProgressPercentage;
                label2.Text = Path.GetFileName(e.UserState.ToString());
            }
        }

        private void bgw_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            List<fileobs> fb = (List<fileobs>)e.Result;
            var fq = fb.OrderBy(x => x.hash).ToList();
            string curgrp = "";
            ListViewGroup lg = new ListViewGroup();
            foreach (fileobs f in fq)
            {
                if (curgrp != f.hash)
                {
                    curgrp = f.hash;
                    lg = new ListViewGroup(f.hash);
                    listView1.Groups.Add(lg);
                }
                ListViewItem lvi = listView1.Items.Add(f.filename);
                lvi.Group = lg;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            foreach (ListViewItem lvi in listView1.Items)
            {
                if (lvi.Checked)
                {
                    File.Delete(lvi.Text);
                    lvi.Remove();
                }
            }
            foreach (ListViewGroup lvg in listView1.Groups)
            {
                if (lvg.Items.Count == 1) lvg.Items[0].Remove();
            }
        }

        private void listView1_DoubleClick(object sender, EventArgs e)
        {
            if (listView1.SelectedIndices.Count > 0)
            {
                string s = listView1.SelectedItems[0].Text;
                System.Diagnostics.Process.Start(s);
            }
        }
    }

    public class fileobs
    {
        public fileobs(string filename)
        {
            this.filename = filename;
            if (filename.Length < 260)
            {
                FileInfo f = new FileInfo(filename);
                this.filesize = f.Length;
            }
        }
        public string filename { get; set; }
        public long filesize { get; set; }
        public string hash { get; set; }
    }

}
