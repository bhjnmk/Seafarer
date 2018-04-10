using System.IO;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;
using Microsoft.Win32;
using Microsoft.WindowsAPICodePack.Dialogs;
using System;
using System.Windows.Controls;

namespace SeafarerApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void FilesBtn_Click(object sender, RoutedEventArgs e)
        {
            filesUC.Visibility = Visibility.Visible;
            startImage.Visibility = Visibility.Hidden;
        }

        private void wizuBtn_Click(object sender, RoutedEventArgs e)
        {
            startImage.Visibility = Visibility.Visible;
            filesUC.Visibility = Visibility.Hidden;
        }
    }
}
