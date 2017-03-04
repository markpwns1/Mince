using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using FastColoredTextBoxNS;
using System.IO;
using System.Diagnostics;
using System.Threading;

namespace MinceIDE
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
            NewTab();
        }

        public TabInfo NewTab(string fileName = null)
        {
            TabPage tab = new TabPage();
            TabInfo info = new TabInfo(tab, fileName);

            tab.Tag = info;
            tab.Text = fileName == null ? "Untitled" : fileName.Substring(fileName.LastIndexOf("\\") + 1);

            var textbox = new FastColoredTextBox();
            textbox.Dock = DockStyle.Fill;
            textbox.Language = Language.JS;

            tab.Controls.Add(textbox);

            textbox.TextChanged += (sender, range) =>
            {
                info.tab.Text = info.saveLocation == null ? "Untitled" : info.saveLocation.Substring(info.saveLocation.LastIndexOf("\\") + 1);
                info.tab.Text += info.isSaved ? "" : "*";
                range.ChangedRange.SetStyle(StyleIndex.Style3, "until", System.Text.RegularExpressions.RegexOptions.None);
            };

            textbox.Text = fileName == null ? "" : File.ReadAllText(fileName);

            tabControl.TabPages.Add(tab);
            tabControl.SelectedTab = tab;

            return info;
        }

        public DialogResult SaveAs(TabInfo tabInfo)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "Mince File (*.mnc)|*.mnc|All Files (*.*)|*.*";
            var result = sfd.ShowDialog();

            if (result == DialogResult.OK)
            {
                File.WriteAllText(sfd.FileName, tabInfo.textBox.Text);

                tabInfo.saveLocation = sfd.FileName;
                tabInfo.originalHashCode = tabInfo.textBox.Text.GetHashCode();

                tabInfo.tab.Text = tabInfo.saveLocation.Substring(tabInfo.saveLocation.LastIndexOf("\\") + 1);
            }

            return result;
        }

        public void Open()
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Mince File (*.mnc)|*.mnc|All Files (*.*)|*.*";
            var result = ofd.ShowDialog();

            if (result == DialogResult.OK)
            {
                if (File.Exists(ofd.FileName))
                {
                    NewTab(ofd.FileName);
                }
                else
                {
                    MessageBox.Show("The file does not exist!");
                }
            }
        }

        public bool Save(TabInfo tabInfo)
        {
            if (!tabInfo.isSaved)
            {
                if (tabInfo.saveLocation == null)
                {
                    return SaveAs(tabInfo) == DialogResult.OK;
                }
                else
                {
                    File.WriteAllText(tabInfo.saveLocation, tabInfo.textBox.Text);

                    tabInfo.originalHashCode = tabInfo.textBox.Text.GetHashCode();

                    tabInfo.tab.Text = tabInfo.saveLocation.Substring(tabInfo.saveLocation.LastIndexOf("\\") + 1);

                    return true;
                }
            }

            return true;
        }

        public void SaveAll()
        {
            foreach (TabPage tab in tabControl.TabPages)
            {
                var info = TabInfo.GetInfo(tab);
                Save(info);
            }
        }

        public bool SafeExit()
        {
            foreach (TabPage tab in tabControl.TabPages)
            {
                var info = TabInfo.GetInfo(tab);
                if (!info.isSaved)
                {
                    var result = MessageBox.Show("Do you want to save " + info.fileName + "?\nAll unsaved work will be lost.", "Warning!", MessageBoxButtons.YesNoCancel);
                    if (result == DialogResult.Yes)
                    {
                        if (!Save(info))
                        {
                            return false;
                        }
                    }
                    else if(result == DialogResult.No)
                    {
                        continue;
                    }
                    else
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        public FastColoredTextBox GetTextBox()
        {
            return TabInfo.GetInfo(tabControl.SelectedTab).textBox;
        }

        public TabInfo CurrentTab()
        {
            return TabInfo.GetInfo(tabControl.SelectedTab);
        }

        private void New_Click(object sender, EventArgs e)
        {
            NewTab();
        }

        private void Close_Click(object sender, EventArgs e)
        {
            var info = CurrentTab();
            if (!info.isSaved)
            {
                var result = MessageBox.Show("Do you want to save " + info.fileName + "?\nAll unsaved work will be lost.", "Warning!", MessageBoxButtons.YesNoCancel);
                if (result == DialogResult.Yes)
                {
                    if (!Save(info))
                    {
                        return;
                    }
                }
                else if (result == DialogResult.No)
                {
                    
                }
                else
                {
                    return;
                }
            }

            tabControl.TabPages.Remove(tabControl.SelectedTab);
        }

        private void SaveAs_Click(object sender, EventArgs e)
        {
            SaveAs(TabInfo.GetInfo(tabControl.SelectedTab));
        }

        private void Open_Click(object sender, EventArgs e)
        {
            Open();
        }

        private void Save_Click(object sender, EventArgs e)
        {
            Save(TabInfo.GetInfo(tabControl.SelectedTab));
        }

        private void SaveAll_Click(object sender, EventArgs e)
        {
            SaveAll();
        }

        private void Exit_Click(object sender, EventArgs e)
        {
            if (SafeExit())
            {
                Application.Exit();
            }
        }

        private void FormExit(object sender, FormClosingEventArgs e)
        {
            e.Cancel = !SafeExit();
        }

        private void Undo_Click(object sender, EventArgs e)
        {
            GetTextBox().Undo();
        }

        private void Redo_Click(object sender, EventArgs e)
        {
            GetTextBox().Redo();
        }

        private void Cut_Click(object sender, EventArgs e)
        {
            GetTextBox().Cut();
        }

        private void Copy_Click(object sender, EventArgs e)
        {
            GetTextBox().Copy();
        }

        private void Paste_Click(object sender, EventArgs e)
        {
            GetTextBox().Paste();
        }

        private void Delete_Click(object sender, EventArgs e)
        {
            GetTextBox().SelectedText = "";
        }

        private void SelectAll_Click(object sender, EventArgs e)
        {
            GetTextBox().SelectAll();
        }

        private void Run_Click(object sender, EventArgs e)
        {
            File.WriteAllText("Compiler/program.mnc", GetTextBox().Text);

            ProcessStartInfo p = new ProcessStartInfo(Application.StartupPath + "/Compiler/MinceCompiler.exe");
            p.WindowStyle = ProcessWindowStyle.Hidden;
            //p.CreateNoWindow = true;
            p.WorkingDirectory = Application.StartupPath + "/Compiler";
            p.Arguments = "\"" + "program.mnc" + "\"";
            Process.Start(p);

            Thread t = new Thread(new ThreadStart(WaitForFile));
            t.Start();

        }

        public void WaitForFile()
        {
            Thread.Sleep(500);

            int i = 0;
            while (i < 50)
            {
                try
                {
                    Process.Start(Application.StartupPath + "/Compiler/Build/program.exe");
                }
                catch (IOException)
                {
                    //Console.WriteLine(e.Message);
                    Thread.Sleep(100);
                    continue;
                }

                return;
            }

            MessageBox.Show("Could not run the file! Go to compiler/Build/program.exe to run it manually.");
        }
    }
}
