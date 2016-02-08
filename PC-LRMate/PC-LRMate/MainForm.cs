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
    public partial class MainForm : Form
    {
        private KarelFilesEnv karelFileEnv;

        public KarelFilesEnv KarelFileEnv
        {
            get { return karelFileEnv; }
            set { karelFileEnv = value; }
        }

        public MainForm()
        {
            InitializeComponent();
            txtFichier.Dock = DockStyle.Fill;
            txtFichierLS.Dock = DockStyle.Fill;
            karelFileEnv = new KarelFilesEnv();
        }

        public MainForm(string KLFullFileName)
        {
            InitializeComponent();
            txtFichier.Dock = DockStyle.Fill;
            txtFichierLS.Dock = DockStyle.Fill;

            karelFileEnv = new KarelFilesEnv(KLFullFileName);
        }

       
        private void MainForm_Load(object sender, EventArgs e)
        {
            if (karelFileEnv == null)
                return;
            FillRichText(karelFileEnv.KlFullFileName);

            if (karelFileEnv.LsFullFileName == null || karelFileEnv.LsFullFileName == "")
            {
                txtFichierLS.Visible = false;
                return;
            }
          
            FillRichText(karelFileEnv.KlFullFileName);
            txtFichierLS.Visible = false;
             
        }

        public void FillRichText(string fileName)
        {
            try
            {
                if (fileName != "")
                {
                    string content = File.ReadAllText(fileName);
                    txtFichier.Text = content;
                }
            }
            catch (IOException ex)
            {
                return;
            }
        }


        public void FillResultLS(string fileName)
        {
            //karelFileEnv.LsFullFileName = fileName;
            try
            {
                if (fileName != "")
                {
                    string content = File.ReadAllText(fileName);
                    txtFichierLS.Text = content;
                }
            }
            catch (IOException ex)
            {
                return;
            }
        }

        public void FillResultLS()
        {
            try
            {
                if (karelFileEnv.LsFullFileName != "")
                {
                    txtFichierLS.Visible = true;
                    string content = File.ReadAllText(karelFileEnv.LsFullFileName);
                    txtFichierLS.Text = content;
                }
            }
            catch (IOException ex)
            {
                return;
            }
        }

        public string SaveFile()
        {

            if (karelFileEnv.KlFullFileName == null || karelFileEnv.KlFullFileName == "")
            {
                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
                saveFileDialog.Filter = "Fichiers karel (*.kl)|*.kl|Tous les fichiers (*.*)|*.*";
                if (saveFileDialog.ShowDialog(this) == DialogResult.OK)
                {
                    karelFileEnv.KlFullFileName = saveFileDialog.FileName;
                    //((MDIParent)MdiParent).FullFileName = fileName;
                    File.WriteAllText(karelFileEnv.KlFullFileName, txtFichier.Text);
                }
            }
            else
            {
                File.WriteAllText(karelFileEnv.KlFullFileName, txtFichier.Text);
            }
            return karelFileEnv.KlFullFileName;

        }

        public string SaveAsFile()
        {

           
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            saveFileDialog.Filter = "Fichiers karel (*.kl)|*.kl|Tous les fichiers (*.*)|*.*";
            if (saveFileDialog.ShowDialog(this) == DialogResult.OK)
            {
                karelFileEnv.KlFullFileName = saveFileDialog.FileName;
                karelFileEnv.Update(karelFileEnv.KlFullFileName);
                //((MDIParent)MdiParent).FullFileName = fileName;
                File.WriteAllText(karelFileEnv.KlFullFileName, txtFichier.Text);
            }            
            return karelFileEnv.KlFullFileName;

        }


        private void btnCompiler_Click(object sender, EventArgs e)
        {
            
        }
    }
}
