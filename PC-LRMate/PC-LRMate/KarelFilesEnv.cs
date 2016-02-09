using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace PC_LRMate
{
    /// <summary>
    /// Informations globales sur le fichier karel traité
    /// </summary>
    public class KarelFilesEnv
    {
        string rootDirectory;

        public string RootDirectory
        {
            get { return rootDirectory; }
            set { rootDirectory = value; }
        }
        string pcFullFileName;

        public string PcFullFileName
        {
            get { return pcFullFileName; }
            set { pcFullFileName = value; }
        }
        string klFullFileName;

        public string KlFullFileName
        {
            get { return klFullFileName; }
            set { klFullFileName = value; }
        }
        
        
        string lsFullFileName;

        public string LsFullFileName
        {
            get
            {
                return lsFullFileName;
            }
        }

        public string getFileName(string fullFileName)
        {
            return (new FileInfo(fullFileName).Name);
        }

      

        public string KlFileName
        {
            get {
                return getNameFromFileName(klFullFileName);
            }
          
        }



        private string getNameFromFileName(string klFullFileName)
        {
            FileInfo fi = new FileInfo(klFullFileName);
            return fi.Name;
        }


        private string lsFileName;

        public string LsFileName
        {
            get { return getNameFromFileName(lsFullFileName); }
          
        }
        private string pcFileName;

        public string PcFileName
        {
            get { return getNameFromFileName(pcFullFileName); }
          
        }

        

        /// <summary>
        /// Constructor
        /// </summary>
        public KarelFilesEnv()
        {
            pcFullFileName = null;
            klFullFileName = null;
            lsFullFileName = null; 
            rootDirectory = @"c:\temp";

        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="klFullFileName">full path to kl file</param>
        public KarelFilesEnv(string klFullFileName)
        {
            this.klFullFileName = klFullFileName;
            try{
            FileInfo fi = new FileInfo(klFullFileName);
            if (fi.Extension.ToUpper() == ".KL")
            {

                string klFileName = fi.Name;
                string lsFileName = klFileName.Replace(".kl", ".ls");
                string pcFileName = klFileName.Replace(".kl", ".pc");
                rootDirectory = fi.Directory.FullName;
                pcFullFileName = klFullFileName.Replace(".kl", ".pc");
                lsFullFileName = klFullFileName.Replace(".kl", ".ls");       
            } 
            else
                throw new InvalidOperationException();
            }
            catch (Exception ex){
                throw ex;
            }
        }

        /// <summary>
        /// Update karel informations. Used for example after a save as.
        /// </summary>
        /// <param name="klFullFileName"></param>
        public void Update(string klFullFileName)
        {
            this.klFullFileName = klFullFileName;
            try
            {
                FileInfo fi = new FileInfo(klFullFileName);
                if (fi.Extension.ToUpper() == ".KL")
                {

                    string klFileName = fi.Name;
                    string lsFileName = klFileName.Replace(".kl", ".ls");
                    string pcFileName = klFileName.Replace(".kl", ".pc");
                    rootDirectory = fi.Directory.FullName;
                    pcFullFileName = klFullFileName.Replace(".kl", ".pc");
                    lsFullFileName = klFullFileName.Replace(".kl", ".ls");
                }
                else
                    throw new InvalidOperationException();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


    }
}
