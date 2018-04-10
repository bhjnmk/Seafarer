using System.IO;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;
using System;
using System.Windows.Controls;

namespace SeafarerApp
{
    /// <summary>
    /// Interaction logic for Files.xaml
    /// </summary>
    public partial class Files : UserControl
    {
        #region Variables

        string directoryPath = @"D:\Seafarer";
        string fileName = "";
        string serverFileName = "";
        string driveFileName = "";

        #endregion //Variables

        public Files()
        {
            InitializeComponent();
            TxtbPath.Text = directoryPath;
            Directory.CreateDirectory(directoryPath);
            Directory.CreateDirectory(directoryPath + @"\Archiwum");
            Folder.FilesNamesLoad(ListViewDrive, directoryPath);
        }

        public void Connect()
        {
            try
            {
                FTPconnection.DisplayFilesNamesFromServer(ListViewServer);
            }
            catch
            {
                MessageBox.Show("Brak połączenia z Internetem!");
            }
        }

        #region GeneralMethods

        private void SetRtbText(string text, bool clear, bool readOnly)
        {
            RtbEditor.IsReadOnly = readOnly;
            if (clear)
            {
                RtbEditor.Document.Blocks.Clear();
            }
            RtbEditor.AppendText(text);
            //RtbEditor.Document.Blocks.Add(new Paragraph(new Run(text)));                      
        }

        private void EnabledServerBtns(bool enabled)
        {
            OpenFileServerBtn.IsEnabled = enabled;
            DeleteServerFileBtn.IsEnabled = enabled;
            DownloadBtn.IsEnabled = enabled;
        }

        private void EnabledDriveBtns(bool enabled)
        {
            OpenFileDriveBtn.IsEnabled = enabled;
            DeleteDriveFileBtn.IsEnabled = enabled;
            EditFileBtn.IsEnabled = enabled;
            ArchiveFileBtn.IsEnabled = enabled;
            UploadBtn.IsEnabled = enabled;
        }

        private void EnabledEditorBtns(bool enabled)
        {
            SaveBtn.IsEnabled = enabled;
            SaveAsBtn.IsEnabled = enabled;
            ClearBtn.IsEnabled = enabled;
            
        }

        private void ListEnabled(ListView lv, bool server)
        {
            if (lv.SelectedItems.Count < 1)
            {
                if (server)
                {
                    EnabledServerBtns(false);
                }
                else
                {
                    EnabledDriveBtns(false);
                }
            }
            else if (lv.SelectedItems.Count > 1)
            {
                lv.SelectedItems.Clear();
            }
            else
            {
                if (server)
                {
                    EnabledServerBtns(true);
                }
                else
                {
                    EnabledDriveBtns(true);
                }
            }
        }

        #endregion //GeneralMethods

        #region ServerView

        private void RefreshServerBtn_Click(object sender, RoutedEventArgs e)
        {
            Connect();
        }

        private void ListViewServer_Click(object sender, MouseButtonEventArgs e)
        {
            var item = (sender as ListView).SelectedItem;
            if (item != null)
            {
                serverFileName = item.ToString();
            }
            ListViewDrive.SelectedItems.Clear();
        }

        private void OpenFileServerBtn_Click(object sender, RoutedEventArgs e)
        {
            if (serverFileName.Length > 5)
            {
                string text = FTPconnection.DisplayFileFromServer(serverFileName);
                SetRtbText(text, true, true);
                SaveAsBtn.IsEnabled = true;
            }
            else
            {
                MessageBox.Show("Zaznacz plik, który chcesz otworzyć");
            }
        }

        private void DeleteServerFileBtn_Click(object sender, RoutedEventArgs e)
        {
            if (serverFileName.Length > 5)
            {
                FTPconnection.DeleteFileOnServer(serverFileName);
                FTPconnection.DisplayFilesNamesFromServer(ListViewServer);
                serverFileName = "";
            }
            else
            {
                MessageBox.Show("Zaznacz plik, który chcesz usunąć");
            }
        }

        private void DownloadBtn_Click(object sender, RoutedEventArgs e)
        {
            if (ListViewServer.SelectedItems.Count > 0)
            { 
                FTPconnection.DownloadFileFromServer(serverFileName, directoryPath, ListViewDrive);
            }
        }

        #endregion //ServerView

        #region FolderView

        private void ChooseFolderBtn_Click(object sender, RoutedEventArgs e)
        {
            string newPath = Folder.LoadFolder(directoryPath);
            string folderName = Path.GetFileName(newPath);
            if (folderName == "Archiwum")
            {
                MessageBox.Show("Jesteś w archiwum, aktualna ścieżka zapisu plików: " + directoryPath);
            }
            else
            {
                directoryPath = newPath;
            }
            TxtbPath.Text = directoryPath;
            Folder.FilesNamesLoad(ListViewDrive, directoryPath);

        }

        private void ListViewDrive_Click(object sender, MouseButtonEventArgs e)
        {
            var item = (sender as ListView).SelectedItem;
            if (item != null)
            {
                driveFileName = item.ToString();
            }
            ListViewServer.SelectedItems.Clear();
        }

        private void OpenFileDriveBtn_Click(object sender, RoutedEventArgs e)
        {
            if (driveFileName.Length > 5)
            {
                string path = directoryPath + @"/" + driveFileName;
                string text = Folder.OpenFile(path);
                SetRtbText(text, true, true);
                SaveAsBtn.IsEnabled = true;
            }
            else
            {
                MessageBox.Show("Zaznacz plik, który chcesz otworzyć");
            }
        }

        private void DeleteDriveFileBtn_Click(object sender, RoutedEventArgs e)
        {
            if (driveFileName.Length > 5)
            {
                Folder.DeleteFile(directoryPath, driveFileName);
                Folder.FilesNamesLoad(ListViewDrive, directoryPath);
                driveFileName = "";
            }
            else
            {
                MessageBox.Show("Zaznacz plik, który chcesz usunąć");
            }
        }

        private void EditFileBtn_Click(object sender, RoutedEventArgs e)
        {
            if (driveFileName.Length > 5)
            {
                string path = directoryPath + @"/" + driveFileName;
                string text = Folder.OpenFile(path);
                fileName = driveFileName;
                SetRtbText(text, true, false);
                EnabledEditorBtns(true);
            }
            else
            {
                MessageBox.Show("Zaznacz plik, który chcesz edytować");
            }
        }

        private void ArchiveFileBtn_Click(object sender, RoutedEventArgs e)
        {
            string apath = directoryPath + @"\" + driveFileName;
            string npath = directoryPath + @"\Archiwum\" + driveFileName;
            File.Move(apath, npath);
            Folder.FilesNamesLoad(ListViewDrive, directoryPath);
        }

        private void UploadBtn_Click(object sender, RoutedEventArgs e)
        {
            if (ListViewDrive.SelectedItems.Count > 0)
            {
                string path = directoryPath + @"\" + driveFileName;
                FTPconnection.UploadFileOnServer(path, driveFileName);

                ListViewServer.Items.Clear();
                FTPconnection.DisplayFilesNamesFromServer(ListViewServer);
            }
        }

        #endregion //FolderView

        #region Edytor

        private void NewFileBtn_Click(object sender, RoutedEventArgs e)
        {
            DateTime date = DateTime.Now;
            fileName = date.ToString("yyyy-MM-dd_hh-mm-ss") + ".txt";
            string path = directoryPath + @"\" + fileName;

            Folder.NewFile(RtbEditor, path);
            Folder.FilesNamesLoad(ListViewDrive, directoryPath);
            EnabledEditorBtns(true);
            CommandSP.IsEnabled = true;
        }

        private void SaveBtn_Click(object sender, RoutedEventArgs e)
        {
            string path = directoryPath + @"\" + fileName;

            Folder.SaveFile(RtbEditor, path);

            fileName = "";
            SetRtbText("",true,true);
        }

        private void SaveAsBtn_Click(object sender, RoutedEventArgs e)
        {
            DateTime date = DateTime.Now;
            fileName = date.ToString("yyyy-MM-dd_hh-mm-ss") + ".txt";
            string path = directoryPath + @"\" + fileName;

            Folder.NewFile(RtbEditor, path);
            Folder.FilesNamesLoad(ListViewDrive, directoryPath);
            EnabledEditorBtns(true);
            CommandSP.IsEnabled = true;

            Folder.SaveFile(RtbEditor, path);
        }

        private void ClearBtn_Click(object sender, RoutedEventArgs e)
        {
            RtbEditor.Document.Blocks.Clear();
        }

        #endregion //Edytor

        #region GeneralEvents

        private void RtbEditor_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (RtbEditor.IsReadOnly != true)
            {
                using (StreamWriter outputFile = new StreamWriter(directoryPath + @"\" + fileName))
                {
                    TextRange range = new TextRange(RtbEditor.Document.ContentStart, RtbEditor.Document.ContentEnd);
                    outputFile.WriteLine(range.Text);
                }
            }
            else
            {
                EnabledEditorBtns(false);
            }
        }

        private void ListViewServer_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ListEnabled(ListViewServer, true);
        }

        private void ListViewDrive_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ListEnabled(ListViewDrive, false);
        }


        #endregion //GeneralEvents

        private void Command1Btn_Click(object sender, RoutedEventArgs e)
        {

            string com1text = "Tekst komendy 1'\r";
            RtbEditor.CaretPosition = RtbEditor.CaretPosition.DocumentStart;
            SetRtbText(com1text, false, false);
        }

        private void Command2Btn_Click(object sender, RoutedEventArgs e)
        {
            string com1text = "Tekst komendy 2\r";
            SetRtbText(com1text, false,false);
        }

        private void Command3Btn_Click(object sender, RoutedEventArgs e)
        {
            string com1text = "Tekst komendy 3\r";
            SetRtbText(com1text, false,false);
        }

        private void Command4Btn_Click(object sender, RoutedEventArgs e)
        {
            string com1text = "Tekst komendy 4\r";
            SetRtbText(com1text, false, false);
        }

        private void Command5Btn_Click(object sender, RoutedEventArgs e)
        {
            string com1text = "Tekst komendy 5\r";
            SetRtbText(com1text, false, false);
        }

        private void Command6Btn_Click(object sender, RoutedEventArgs e)
        {
            string com1text = "Tekst komendy 6\r";
            SetRtbText(com1text, false,false);
        }

        private void UCFiles_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (UCFiles.Visibility == Visibility.Visible)
            {
                Connect();
            }
        }
    }
}