using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ALDropspotter.Views
{
    /// <summary>
    /// Interaction logic for ImageDropPage.xaml
    /// </summary>
    public partial class ImageDropPage : Page
    {
        // Create MyImageElement
        Image MyImageElement = new();
        ImagePage imagePage = new();

        // Create a constructor
        public ImageDropPage()
        {
            InitializeComponent();
        }

        private void Image_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effects = DragDropEffects.Copy;
            }
            else
            {
                e.Effects = DragDropEffects.None;
            }
        }

        private void Image_Drop(object sender, DragEventArgs e)
        {
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
            if (files.Length > 0)
            {
                // Navigate to the image page
                var mainWindow = (MainWindow)Application.Current.MainWindow;
                imagePage.loadImage(files[0]);
                mainWindow.PageFrame.Navigate(imagePage);
            }
        }
    }
}
