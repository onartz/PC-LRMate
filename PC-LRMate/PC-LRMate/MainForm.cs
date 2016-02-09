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
        Dictionary<string, string> instructionsReplace = new Dictionary<string, string>(){
        {"OPEN HAND 1","LACHER"},
        {"CLOSE HAND 1","PRENDRE"},
        {"OPEN HAND 2","LACHER"},
        {"CLOSE HAND 2","PRENDRE"},
        {"OPEN HAND 3","LACHER"},
        {"CLOSE HAND 3","PRENDRE"}
        } ;
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

        /// <summary>
        /// Check dangerous instructions like OPEN HAND , CLOSE HAND
        /// </summary>
        /// <param name="rtb">RichTextBox to search in</param>
        /// <param name="pos"></param>
        /// <returns></returns>
        private bool CheckRichTextBox(RichTextBox rtb, int pos){
            bool res = true;
            int position = pos;
            foreach (KeyValuePair<string, string> s in instructionsReplace)
            {
                position = 0;
                while ((position = rtb.Find(s.Key, position, RichTextBoxFinds.None)) > 0)
                {
                    rtb.Select(position, s.Key.Length);
                    rtb.SelectionColor = Color.Red;
                    string text = string.Format("Instruction {0} trouvée : remplacer par {1}?", s.Key.ToString(), s.Value.ToString());
                    DialogResult d = MessageBox.Show(text,"Remplacer",MessageBoxButtons.YesNo);

                    if (d == DialogResult.Yes)
                    {
                        rtb.SelectionColor = Color.Black;
                        rtb.SelectedText = s.Value;
                    }
                    else
                    {
                        position += s.Key.Length + 1;
                        res = false;
                    }      
                }
            }
            return res;        
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
                File.WriteAllText(karelFileEnv.KlFullFileName, txtFichier.Text);
            }            
            return karelFileEnv.KlFullFileName;

        }

        /// <summary>
        /// Compilation du fichier kl
        /// Vérification que le fichier ne contient pas d'instructions interdites OPEN HAND et CLODE HAND
        /// Sauvegarde du fichier avant de compiler puis lancement du compilateur
        /// </summary>
        public void Compile() {
            SaveFile();
            bool checkResult = CheckRichTextBox(txtFichier, 0);
            if (!(checkResult))
            {
                MessageBox.Show("Echec de la compilation");
                txtFichierLS.Clear();
                return;
            }
            SaveFile();
            try
            {
                System.Diagnostics.Process process = new System.Diagnostics.Process();
                process.StartInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
                process.StartInfo.FileName = Properties.Settings.Default.Compiler;
                process.StartInfo.Arguments = "/l " + KarelFileEnv.KlFullFileName;
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
                //If pc file has been generated in local directory
                if (File.Exists(@".\" + KarelFileEnv.PcFileName))
                {
                    if (File.Exists(KarelFileEnv.PcFullFileName))
                        File.Delete(KarelFileEnv.PcFullFileName);
                    File.Move(@".\" + KarelFileEnv.PcFileName, KarelFileEnv.PcFullFileName);
                }

                //If ls file has been generated in local directory
                if (File.Exists(@".\" + KarelFileEnv.LsFileName))
                {
                    if (File.Exists(KarelFileEnv.LsFullFileName))
                        File.Delete(KarelFileEnv.LsFullFileName);
                    File.Move(@".\" + KarelFileEnv.LsFileName, KarelFileEnv.LsFullFileName);
                    FillResultLS();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Une erreur est survenu lors de la copie des fichiers générés : " + ex.Message);
            }
        }


        private void btnCompiler_Click(object sender, EventArgs e)
        {
            
        }
    }
}
