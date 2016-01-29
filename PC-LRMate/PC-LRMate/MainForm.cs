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
        //Karel full file name (.kl)
        private string fileName;

        public string FileName
        {
            get { return fileName; }
            set { fileName = value; }
        }

        //Pc file name (.pc)
        private string pcFileName;

        public string PcFileName
        {
            get { return pcFileName; }
            set { pcFileName = value; }
        }



        private string fileResultLS;

        public MainForm()
        {
            InitializeComponent();
            txtFichier.Dock = DockStyle.Fill;
            txtFichierLS.Dock = DockStyle.Fill;
        }

        public MainForm(string fullFileName)
        {
            InitializeComponent();
            txtFichier.Dock = DockStyle.Fill;
            txtFichierLS.Dock = DockStyle.Fill;

            fileName = fullFileName;
        }

       
        private void MainForm_Load(object sender, EventArgs e)
        {
            //fileName = ((MDIParent)(this.MdiParent)).FullFileName;
            if (fileName == null)
                return;

            FillRichText(fileName);
            txtFichierLS.Visible = fileResultLS != "";
    
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
            fileResultLS = fileName;
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

        public string SaveFile()
        {
           
            if (fileName == null)
            {
                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
                saveFileDialog.Filter = "Fichiers karel (*.kl)|*.kl|Tous les fichiers (*.*)|*.*";
                if (saveFileDialog.ShowDialog(this) == DialogResult.OK)
                {
                    fileName = saveFileDialog.FileName;
                    //((MDIParent)MdiParent).FullFileName = fileName;
                    File.WriteAllText(fileName, txtFichier.Text);
                }
            }
            else
            {
                File.WriteAllText(fileName, txtFichier.Text);
            }
            return fileName;

        }


        private void btnCompiler_Click(object sender, EventArgs e)
        {
            
        }
    }
}
