using ImageViewer.Core;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageViewer.Source.MVVM.Model
{
    class FileManager : ObservableObject
    {
        #region Private Members

        // The internal filepath for the current file.
        private string m_CurrentFile;
        // The internal filepath for the previous file directory.
        private string m_LastDir;
        // The internal filepath for the current folder.
        private string m_CurrentFolder;
        // The internal filepaths for the images in the current folder.
        private string[] m_FilesInCurrentFolder;

        #endregion

        #region Public Members

        // The filepath for the current file.
        public string CurrentFile
        {
            get { return m_CurrentFile; }
            set
            {
                m_CurrentFile = value;
                OnPropertyChanged();
            }
        }

        // The previous file directory
        public string LastDir
        {
            get { return m_LastDir; }
            set
            {
                m_LastDir = value;
                OnPropertyChanged();
            }
        }

        // The filepath for the current folder.
        public string CurrentFolder
        {
            get { return m_CurrentFolder; }
            set
            {
                m_CurrentFolder = value;
                OnPropertyChanged();
            }
        }

        // The filepaths for the files in the current folder.
        public string[] FilesInCurrentFolder
        {
            get { return m_FilesInCurrentFolder; }
            set
            {
                m_FilesInCurrentFolder = value;
                OnPropertyChanged();
            }
        }

        #endregion

        // Opens the file browser
        public void BrowseFiles()
        {
            OpenFileDialog dialog = new OpenFileDialog();

            dialog.Filter = "Image Files (*.jpg;*.png)|*.jpg;*.png";

            if (dialog.ShowDialog() == true)
            {
                CurrentFile = dialog.FileName;

                CurrentFolder = Path.GetDirectoryName(CurrentFile);
                FilesInCurrentFolder = Directory.GetFiles(CurrentFolder, "*.jpg").Union(Directory.GetFiles(CurrentFolder, "*.png")).ToArray();

            }
        }

        /// <summary>
        /// Changes the current image to the next/previous file.
        /// </summary>
        /// <param name="next">True for next, False for previous</param>
        public void ChangeFile(bool next)
        {
            int currentIndex = -1;
            bool success = false;
            // Must be a valid folder
            if (FilesInCurrentFolder.Length == 0)
                return;
            for (int i = 0; i < FilesInCurrentFolder.Length; i++)
            {
                if (FilesInCurrentFolder[i] == CurrentFile)
                {
                    currentIndex = i;
                    currentIndex = next ? currentIndex + 1 : currentIndex - 1;
                    success = true;
                    break;
                }
            }
            if (!success)
                return;

            // Now that you have the new index, make sure it's valid.
            if (!(currentIndex > FilesInCurrentFolder.Length - 1 || currentIndex < 0))
                CurrentFile = FilesInCurrentFolder[currentIndex];
        }
    }
}
