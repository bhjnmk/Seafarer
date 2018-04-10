using System;
using System.IO;
using System.Net;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Text.RegularExpressions;

namespace SeafarerApp
{
    public static class FTPconnection
    {
        const string username = "mkuzmicz";
        const string password = "Top3n@nt@48";
        const string serverFTP = "ftp://mkwk018.cba.pl/mkuzmicz.cba.pl/Seafarer/";

        public static void DisplayFilesNamesFromServer(ListView listView)
        {
            listView.Items.Clear();
            FtpWebRequest request = (FtpWebRequest)WebRequest.Create(serverFTP);
            request.Method = WebRequestMethods.Ftp.ListDirectory;
            request.Credentials = new NetworkCredential(username,password);

            FtpWebResponse response = (FtpWebResponse)request.GetResponse();
            StreamReader reader = new StreamReader(response.GetResponseStream());

            string line = reader.ReadLine();

            Regex regTxt = new Regex(@".*\.txt");

            bool isEmpty = true;

            while (!string.IsNullOrEmpty(line))
            {
                if (regTxt.IsMatch(line))
                {
                    listView.Items.Add(line);
                    isEmpty = false;
                }
                line = reader.ReadLine();
            }

            if(isEmpty)
            {
                listView.Items.Add("Brak plików .txt na serwerze");
                listView.IsEnabled = false;
            }
            else
            {
                listView.IsEnabled = true;
            }

            reader.Close();
            response.Close();
        }

        public static string DisplayFileFromServer(string fileName)
        {
            string server = serverFTP + fileName;
            WebClient request = new WebClient();
            string fileString = "";

            request.Credentials = new NetworkCredential(username, password);
            try
            {
                byte[] newFileData = request.DownloadData(server);
                fileString = Encoding.UTF8.GetString(newFileData);
            }
            catch (WebException e)
            {
                MessageBox.Show(e.ToString());
            }

            return fileString;
        }

        public static void DeleteFileOnServer(string fileName)
        {
            string serverFTP = "ftp://mkwk018.cba.pl/mkuzmicz.cba.pl/Seafarer/" + fileName;

            FtpWebRequest request = (FtpWebRequest)WebRequest.Create(serverFTP);
            request.Method = WebRequestMethods.Ftp.DeleteFile;
            request.Credentials = new NetworkCredential("mkuzmicz", "Top3n@nt@48");

            FtpWebResponse response = (FtpWebResponse)request.GetResponse();
            MessageBox.Show("Status usuwania: " + response.StatusDescription);
            response.Close();
        }

        public static void DownloadFileFromServer(string fileName, string path, ListView lv)
        {
            WebClient request = new WebClient();
            string fileString = "";

            request.Credentials = new NetworkCredential(username, password);
            try
            {
                byte[] newFileData = request.DownloadData(serverFTP + fileName);
                fileString = Encoding.UTF8.GetString(newFileData);
            }
            catch (WebException e)
            {
                MessageBox.Show(e.ToString());
            }

            string fPath = path + @"\" + fileName;

            using (FileStream fs = File.Create(fPath))
            {
                Byte[] emptyText = new UTF8Encoding(true).GetBytes(fileString);
                fs.Write(emptyText, 0, emptyText.Length);
                MessageBox.Show("Pobrano plik: " + fPath);
                Folder.FilesNamesLoad(lv, path);
            }
        }

        public static void UploadFileOnServer(string path, string fileName)
        {
            FtpWebRequest request = (FtpWebRequest)WebRequest.Create(serverFTP + fileName);
            request.Method = WebRequestMethods.Ftp.UploadFile;
            request.Credentials = new NetworkCredential(username,password);

            string text = File.ReadAllText(path);
            byte[] fileContent = Encoding.UTF8.GetBytes(text);
            request.ContentLength = fileContent.Length;

            Stream requestStream = request.GetRequestStream();
            requestStream.Write(fileContent, 0, fileContent.Length);
            requestStream.Close();

            FtpWebResponse response = (FtpWebResponse)request.GetResponse();

            MessageBox.Show("Upload ukończony, status: " + response.StatusDescription);

            response.Close();
        }

    }
}
