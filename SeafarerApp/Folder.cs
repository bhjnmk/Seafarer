using Microsoft.WindowsAPICodePack.Dialogs;
using System;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

namespace SeafarerApp
{
    public static class Folder
    {
        public static string LoadFolder(string actualPath)
        {
            string path = actualPath;
            CommonOpenFileDialog dlg = new CommonOpenFileDialog();
            dlg.IsFolderPicker = true;
            if (dlg.ShowDialog() == CommonFileDialogResult.Ok)
            {
                path = dlg.FileName;
            }

            string folderName = Path.GetFileName(path);

            if (folderName == "Archiwum")
            {
                return path;
            }
            else
            {
                //utworzenie folderu Archiwum w nowej ścieżce, jeśli nie istnieje
                string archPath = path + @"\Archiwum";
                Directory.CreateDirectory(archPath);
                return path;
            }
        }

        public static void FilesNamesLoad(ListView lvcontrol, string path)
        {
            bool isEmpty = true;
            lvcontrol.Items.Clear();

            if (path.Length > 1)
            {
                string[] fileEntries = Directory.GetFiles(path);
                foreach (string fileName in fileEntries)
                {
                    if (Path.GetExtension(fileName) == ".txt")
                    {
                        lvcontrol.Items.Add(Path.GetFileName(fileName));
                        isEmpty = false;
                    }
                }
            }

            if (isEmpty)
            {
                lvcontrol.Items.Add("Brak plików .txt na dysku");
                lvcontrol.IsEnabled = false;
            }
            else
            {
                lvcontrol.IsEnabled = true;
            }
        }

        public static string OpenFile(string path)
        {
            string text = "";

            string[] lines = File.ReadAllLines(path);

            foreach (string line in lines)
            {
                text += line + "\n";
            }
            return text;
        }

        public static void NewFile(RichTextBox rtb, string path)
        {
            using (FileStream fs = File.Create(path))
            {
                Byte[] emptyText = new UTF8Encoding(true).GetBytes("");
                fs.Write(emptyText, 0, emptyText.Length);
                MessageBox.Show("Utworzono plik: " + path);
            }
            rtb.IsReadOnly = false;
        }

        public static void DeleteFile(string path, string fileName)
        {
            DirectoryInfo di = new DirectoryInfo(path);

            foreach (FileInfo file in di.GetFiles())
            {
                if(file.Name == fileName)
                {
                    file.Delete();
                    MessageBox.Show("Usunięto plik: " + path + @"\" + fileName);
                }
            }
        }

        public static void SaveFile(RichTextBox rtb, string path)
        {
            using (StreamWriter outputFile = new StreamWriter(path))
            {
                TextRange range = new TextRange(rtb.Document.ContentStart, rtb.Document.ContentEnd);
                outputFile.WriteLine(range.Text);
                rtb.IsReadOnly = false;
                MessageBox.Show("Zapisano");
            }
        }

    }
}
