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

        private string MapImagePath;
        private string MapBoundaryImagePath;
        private string LobbyImagePath;
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

            // Get map png file
            MapImagePath = dropspotMatcher.MapImagePaths[matchedValues["map_name"]];
            Debug.WriteLine(MapImagePath);

            // Print current path
            Debug.WriteLine(System.IO.Directory.GetCurrentDirectory());

            // Display the map
            //Image stormPointImage = (Image)FindName("map_img");
            //stormPointImage.Source = new BitmapImage(new Uri(MapImagePath));

            // Loop over all matched dropspots
            foreach (KeyValuePair<String, String> dropspot in matchedValues)
            {
                // Check if the dropspot is the map name or none
                if (dropspot.Key == "map_name" || dropspot.Value == "none")
                {
                    // Skip the map name
                    continue;
                }

                // Get the name of the dropspot
                String dropspotName = dropspot.Value;

                // Concatenate the name of the dropspot with "border"
                String dropspotBorderName = dropspotName + "_border";

                // Search for the border of the dropspot in the image
                Path dropspotBorder = (Path)FindName(dropspotBorderName);

                // Check if dropspotBorder exists
                if (dropspotBorder == null)
                {
                    Debug.WriteLine("Dropspot border not found: " + dropspotBorderName);
                }


                // Edit the color of the border to ##33FF0000
                dropspotBorder.Fill = new SolidColorBrush(Color.FromArgb(0x33, 0xFF, 0x00, 0x00));
            }

            // Loop over all free dropspots
            foreach (KeyValuePair<String, String> dropspot in freeDropspots)
            {
                // Check if the dropspot is the map name or none
                if (dropspot.Key == "map_name" || dropspot.Value == "none")
                {
                    // Skip the map name
                    continue;
                }

                // Get the name of the dropspot
                String dropspotName = dropspot.Key;

                // Concatenate the name of the dropspot with "border"
                String dropspotBorderName = dropspotName + "_border";

                // Search for the border of the dropspot in the image
                Path dropspotBorder = (Path)FindName(dropspotBorderName);

                // Check if dropspotBorder exists
                if (dropspotBorder == null)
                {
                    Debug.WriteLine("Dropspot border not found: " + dropspotBorderName);
                }

                // Edit the color of the border to #3300FF19
                dropspotBorder.Fill = new SolidColorBrush(Color.FromArgb(0x33, 0x00, 0xFF, 0x19));

                // Add dropspot to sidebar
                FreeDropspotsList.Items.Add(dropspot.Value);
            }
        }
    }
}
