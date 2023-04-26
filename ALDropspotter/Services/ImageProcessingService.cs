using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using Emgu.CV.Structure;
using Emgu.CV;
using Emgu.CV.Reg;
using System.Drawing.Imaging;
using System.IO;
using System.Diagnostics;
using Tesseract;

namespace ALDropspotter.Services
{
    class ImageProcessingService
    {
        public Dictionary<string, Rectangle> GetRectangles()
        {
            var rectangles = new Dictionary<string, Rectangle>();

            rectangles["map_name"] = new Rectangle(185, 36, 227, 30);

            rectangles["team_1"] = new Rectangle(467, 197, 252, 28);
            rectangles["team_6"] = new Rectangle(467, 407, 252, 28);
            rectangles["team_11"] = new Rectangle(467, 617, 252, 28);
            rectangles["team_16"] = new Rectangle(467, 827, 252, 28);

            rectangles["team_2"] = new Rectangle(747, 197, 252, 28);
            rectangles["team_7"] = new Rectangle(747, 407, 252, 28);
            rectangles["team_12"] = new Rectangle(747, 617, 252, 28);
            rectangles["team_17"] = new Rectangle(747, 827, 252, 28);

            rectangles["team_3"] = new Rectangle(1027, 197, 252, 28);
            rectangles["team_8"] = new Rectangle(1027, 407, 252, 28);
            rectangles["team_13"] = new Rectangle(1027, 617, 252, 28);
            rectangles["team_18"] = new Rectangle(1027, 827, 252, 28);

            rectangles["team_4"] = new Rectangle(1307, 197, 252, 28);
            rectangles["team_9"] = new Rectangle(1307, 407, 252, 28);
            rectangles["team_14"] = new Rectangle(1307, 617, 252, 28);
            rectangles["team_19"] = new Rectangle(1307, 827, 252, 28);

            rectangles["team_5"] = new Rectangle(1587, 197, 252, 28);
            rectangles["team_10"] = new Rectangle(1587, 407, 252, 28);
            rectangles["team_15"] = new Rectangle(1587, 617, 252, 28);
            rectangles["team_20"] = new Rectangle(1587, 827, 252, 28);

            return rectangles;
        }
        // Function to extract text from image, returns a dictionary <string, string> with the text
        public Dictionary<string, string> ExtractTextFromImage(string imagePath)
        {
            var result = new Dictionary<string, string>();
            var image = new Image<Bgr, byte>(imagePath);
            var rectangles = GetRectangles();

            var debugFolderPath = "tmpImages";

            // Create tasks for each rectangle
            var tasks = rectangles.Select(async rectangle =>
            {
                var croppedImage = image.Copy(rectangle.Value);

                var grayscaleImage = croppedImage.Convert<Gray, byte>();

                if (rectangle.Key == "map_name")
                {
                    grayscaleImage = grayscaleImage.ThresholdBinary(new Gray(100), new Gray(255));
                }

                grayscaleImage = grayscaleImage.Erode(1);

                var bitmap = grayscaleImage.ToBitmap();

                string directory = "tmpImages";
                string filename = $"{rectangle.Key}.png";
                string path = Path.Combine(directory, filename);

                Directory.CreateDirectory(directory);
                bitmap.Save(path, System.Drawing.Imaging.ImageFormat.Png);

                string text;
                using (var engine = new TesseractEngine("tessdata", "eng", EngineMode.Default))
                {
                    using (var page = engine.Process(bitmap))
                    {
                        text = page.GetText().Trim();
                    }
                }

                return (rectangle.Key, text);
            });

            // Wait for all tasks to complete
            Task.WaitAll(tasks.ToArray());

            // Add results to dictionary
            foreach (var (key, value) in tasks.Select(t => t.Result))
            {
                result[key] = value;
                Debug.WriteLine($"{key}: {value}");
            }

            return result;
        }
    }
}
