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
using System.Diagnostics;
using ALDropspotter.Services;

namespace ALDropspotter.Views
{
    /// <summary>
    /// Interaction logic for ImagePage.xaml
    /// </summary>
    public partial class ImagePage : Page
    {
        public ImagePage()
        {
            InitializeComponent();
        }

        public string MapImagePath { get; set; } = "/img/mp_rr_tropic_island_mu1.png";
        // Dictionary with map names as keys and image paths as values
        public Dictionary<String, String> MapImagePaths = new()
        {
            { "Storm Point", "/img/mp_rr_tropic_island_mu1.png"},
            { "Worlds Edge", "/img/mp_rr_desertlands_mu4.png"}
        };
        public string LobbyImagePath { get; set; }
        private ImageProcessingService imageProcessor = new();
        private DropspotMatchingService dropspotMatcher = new();

        public void loadImage(String imagePath)
        {
            Debug.WriteLine("Received lobby image: " + imagePath);

            // Get the text from the image
            LobbyImagePath = imagePath;
            Dictionary<String, String> lobbyText = imageProcessor.ExtractTextFromImage(LobbyImagePath);

            // Get the map name from the text
            String mapName = lobbyText["map_name"];
            Debug.WriteLine("Map name: " + mapName);

            // Get dropspot matches (everything else)
            Dictionary<String, String> dropspots = lobbyText.Where(x => x.Key.StartsWith("team_")).ToDictionary(x => x.Key, x => x.Value);

            // Loop over all dropspots and print them
            Debug.WriteLine("Dropspots:");
            foreach (KeyValuePair<String, String> dropspot in dropspots)
            {
                Debug.WriteLine(dropspot.Key + ": " + dropspot.Value);
            }


            // Match the dropspots
            Dictionary<String, String> matchedValues = dropspotMatcher.GetDropspotMatches(mapName, dropspots);
            // Loop over all dropspots and print them
            Debug.WriteLine("Matched values:");
            foreach (KeyValuePair<String, String> dropspot in matchedValues)
            {
                Debug.WriteLine(dropspot.Key + ": " + dropspot.Value);
            }

            // Get the free dropspots
            Dictionary<String, String> freeDropspots = dropspotMatcher.GetFreeDropspots(matchedValues);
            // Loop over all dropspots and print them
            Debug.WriteLine("Free dropspots:");
            foreach (KeyValuePair<String, String> dropspot in freeDropspots)
            {
                Debug.WriteLine(dropspot.Value);
            }

            // Display the map
            //BitmapImage bitmapImage = new BitmapImage();
            //bitmapImage.BeginInit();
            //bitmapImage.UriSource = new Uri(MapImagePath);
            //bitmapImage.EndInit();
            //ImageElement.Source = bitmapImage;
        }
    }

}
