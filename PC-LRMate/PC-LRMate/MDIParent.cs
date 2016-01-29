using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Net;

namespace PC_LRMate
{
    public partial class MDIParent : Form
    {
        private int childFormNumber = 0;
        //complete fileName
        private string fullFileName;
        private string klFileName;
        private string pcFileName;
        private string lsFileName;
        private string fullPcFileName;
        private string fullLsFileName;

        //public string FullFileName
        //{
        //    get { return fullFileName; }
        //    set {
        //        fullFileName = value;
        //        initFileNames();
        //         }
        //}

        public MDIParent()
        {
            InitializeComponent();
        }

        private void initFileNames()
        {
            FileInfo fi = new FileInfo(fullFileName);
            if (fi.Extension.ToUpper() == ".KL")
            {
                klFileName = fi.Name;
                lsFileName = klFileName.Replace(".kl", ".ls");
                pcFileName = klFileName.Replace(".kl", ".pc");
                fullPcFileName = @".\" + pcFileName;
                fullLsFileName = @".\" + lsFileName;
            } 
        }

        private void ShowNewForm(object sender, EventArgs e)
        {
            //Form childForm = new MainForm();
            Form childForm = new MainForm();
            childForm.MdiParent = this;
            childForm.Text = "Programme Karel " + childFormNumber++;
            childForm.Show();
        }

        private void ShowForm(object sender, EventArgs e)
        {
            //MainForm childForm = new MainForm();
            Form childForm = new MainForm(fullFileName);
            childForm.MdiParent = this;
            childForm.Text = "Karel : " + fullFileName;
            childForm.Dock = DockStyle.Fill;
            childForm.Show();
        }

        private void OpenFile(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.RestoreDirectory = true;
      
            openFileDialog.Filter = "Fichiers karel (*.kl)|*.kl|Tous les fichiers (*.*)|*.*";
            if (openFileDialog.ShowDialog(this) == DialogResult.OK)
            {
                fullFileName = openFileDialog.FileName;
                initFileNames();
                ShowForm(this, e);
            }
        }

        private void SaveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            saveFileDialog.Filter = "Fichiers karel (*.kl)|*.kl|Tous les fichiers (*.*)|*.*";
            if (saveFileDialog.ShowDialog(this) == DialogResult.OK)
            {
                string FileName = saveFileDialog.FileName;
            }
        }

        private void saveToolStripButton_Click(object sender, EventArgs e)
        {
            Form activeChild = this.ActiveMdiChild;
            if (activeChild == null)
                return;
            if (activeChild is MainForm)
            {
               // string fileName = ((MainForm)activeChild).FileName;
                fullFileName = ((MainForm)activeChild).SaveFile();
            }
        }

        private void ExitToolsStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void CutToolStripMenuItem_Click(object sender, EventArgs e)
        {
        }

        private void CopyToolStripMenuItem_Click(object sender, EventArgs e)
        {
        }

        private void PasteToolStripMenuItem_Click(object sender, EventArgs e)
        {
        }

        private void ToolBarToolStripMenuItem_Click(object sender, EventArgs e)
        {
            toolStrip.Visible = toolBarToolStripMenuItem.Checked;
        }

        private void StatusBarToolStripMenuItem_Click(object sender, EventArgs e)
        {
            statusStrip.Visible = statusBarToolStripMenuItem.Checked;
        }

        private void CascadeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LayoutMdi(MdiLayout.Cascade);
        }

        private void TileVerticalToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LayoutMdi(MdiLayout.TileVertical);
        }

        private void TileHorizontalToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LayoutMdi(MdiLayout.TileHorizontal);
        }

        private void ArrangeIconsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LayoutMdi(MdiLayout.ArrangeIcons);
        }

        private void CloseAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (Form childForm in MdiChildren)
            {
                childForm.Close();
            }
        }

        /// <summary>
        /// Compile
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            Form activeChild = this.ActiveMdiChild;
            if (!(activeChild is MainForm))
                return;
            
            if (((MainForm)activeChild).FileName == String.Empty)
                return;

            //Sauvegarde du fichier avant compilation
            ((MainForm)activeChild).SaveFile();
            //Compilation

            try
            {
                System.Diagnostics.Process process = new System.Diagnostics.Process();
                process.StartInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
                process.StartInfo.FileName = Properties.Settings.Default.Compiler;
                process.StartInfo.Arguments = "/l " + fullFileName;
                process.Start();
                process.WaitForExit();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Une erreur est survenue lors du lancement du programme Ktrans.exe : " + ex.Message);
                return;
            }


            try
            {
                if (File.Exists(fullLsFileName))
                {
                    FileInfo fi = new FileInfo(fullFileName);
                    if (activeChild is MainForm)
                    {
                        MainForm child = (MainForm)activeChild;
                        child.FillResultLS(fullLsFileName);
                        if (File.Exists(fullPcFileName))
                        {
                            if (File.Exists(fi.DirectoryName + @"\" + pcFileName))
                                File.Delete(fi.DirectoryName + @"\" + pcFileName);
                            String newLocation = fi.DirectoryName + @"\" + pcFileName;
                            File.Move(fullPcFileName, newLocation);
                            child.PcFileName = newLocation;
                        }
                        if (File.Exists(fullLsFileName))
                        {
                            if (File.Exists(fi.DirectoryName + @"\" + lsFileName))
                                File.Delete(fi.DirectoryName + @"\" + lsFileName);
                            File.Move(fullLsFileName, fi.DirectoryName + @"\" + lsFileName);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Une erreur est survenu lors de la copie des fichiers générés : " + ex.Message);
            }
            
        }

        private void toolStripButtonTransferer_Click(object sender, EventArgs e)
        {
            Form activeChild = this.ActiveMdiChild;
            if (activeChild == null)
                return;
            if (!(activeChild is MainForm))
                return;
            
            MainForm child = (MainForm)ActiveMdiChild;
            if (child.PcFileName == null || child.PcFileName == string.Empty)
            {
                MessageBox.Show("Pas de fichier compilé.");  
                return;
            }
            //activeChild = (MainForm)activeChild;
           
            //FtpWebRequest ftpClient = (FtpWebRequest)FtpWebRequest.Create(Properties.Settings.Default.FTPServer + pcFileName);
            FtpWebRequest ftpClient = (FtpWebRequest)FtpWebRequest.Create(new Uri(Properties.Settings.Default.FTPServer + pcFileName));
            ftpClient.Credentials = new System.Net.NetworkCredential(Properties.Settings.Default.FTPUser, "");
            ftpClient.Method = System.Net.WebRequestMethods.Ftp.UploadFile;
            ftpClient.UseBinary = true;
            ftpClient.KeepAlive = true;
            ftpClient.UsePassive = Properties.Settings.Default.PassiveMode;
            //ftpClient.Timeout = 2000;
            System.IO.FileInfo fi = new System.IO.FileInfo(child.PcFileName);
            ftpClient.ContentLength = fi.Length;
            byte[] buffer = new byte[4097];
            int bytes = 0;
            int total_bytes = (int)fi.Length;
            System.IO.FileStream fs = fi.OpenRead();
            System.IO.Stream rs = ftpClient.GetRequestStream();
            while (total_bytes > 0)
            {
                bytes = fs.Read(buffer, 0, buffer.Length);
                rs.Write(buffer, 0, bytes);
                total_bytes = total_bytes - bytes;
            }
            //fs.Flush();
            fs.Close();
            rs.Close();
            FtpWebResponse uploadResponse = (FtpWebResponse)ftpClient.GetResponse();
            string value = uploadResponse.StatusDescription;

            uploadResponse.Close();
            MessageBox.Show(value);  
        }    
    }
}
