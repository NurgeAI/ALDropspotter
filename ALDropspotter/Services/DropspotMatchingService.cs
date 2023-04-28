using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ALDropspotter.Services
{
    internal class DropspotMatchingService
    {
        // Dictionary with each map's dropspots and their corresponding name
        public static Dictionary<string, Dictionary<string, string>> Dropspots = new()
        {
            {
                "stormpoint",
                new Dictionary<string, string> {
                    {"bean", "Bean"},
                    {"barometer", "Barometer"},
                    {"cenote", "Cenote Cave"},
                    {"checkpoint", "Checkpoint"},
                    {"command_center", "Command Center"},
                    {"downed_beast", "Downed Beast"},
                    {"fish_farms", "Fish Farms"},
                    {"gale_station", "Gale Station"},
                    {"high_point", "High Point"},
                    {"launch_pad", "Launch Pad"},
                    {"lightning_rod", "Lightning Rod"},
                    {"mill", "Mill"},
                    {"north_pad", "North Pad"},
                    {"shipfall", "Shipfall"},
                    {"storm_catcher", "Storm Catcher"},
                    {"thunder_watch", "Thunder Watch"},
                    {"wall", "Wall"},
                }
            },
            {
                "worldsedge",
                new Dictionary<string, string>
                {
                    {"fissure", "Fissure"},
                    {"overlook", "Overlook"},
                    {"tree", "Tree"},
                }
            }
        };


        // Dictionary matching each term with map name and the dropspot names
        public static Dictionary<string, Dictionary<string, string>> DropspotTerms = new()
        {
            { 
                "stormpoint", 
                new Dictionary<string, string> {
                    {"northpad", "north_pad" },
                    {"npad", "north_pad" },
                    {"checkpoint", "checkpoint" },
                    {"downedbeast", "downed_beast"},
                    {"beast", "downed_beast"},
                    {"mill", "mill"},
                    {"bean", "bean"},
                    {"nonamemill", "bean"},
                    {"millnoname", "bean"},
                    {"cenotecave", "cenote_cave"},
                    {"cenote", "cenote_cave"},
                    {"cave", "cenote_cave"},
                    {"barometer", "barometer" },
                    {"baro", "barometer" },
                    {"wall", "wall" },
                    {"highpoint", "high_point" },
                    {"hpoint", "high_point" },
                    {"lightningrod", "lightning_rod" },
                    {"lrod", "lightning_rod" },
                    {"cascades", "cascades" },
                    {"thunderwatch", "thunder_watch" },
                    {"thunder", "thunder_watch" },
                    {"commandcenter", "command_center" },
                    {"command", "command_center" },
                    {"storm catcher", "storm_catcher" },
                    {"storm", "storm_catcher" },
                    {"jurassic", "jurassic" },
                    {"launchpad", "launch_pad" },
                    {"lpad", "launch_pad" },
                    {"antenna", "antenna" },
                    {"fishfarms", "fish_farms" },
                    {"farms", "fish_farms"},
                    {"ffarms", "fish_farms"},
                    {"fish", "fish_farms"},
                    {"galestation", "gale_station" },
                    {"gale", "gale_station" },
                    {"shipfall", "shipfall" },
                    {"ship", "shipfall" },
                    {"", "none"}
                } 
            },
            { 
                "worldsedge", 
                new Dictionary<string, string>
                {
                    {"fissure", "fissure" },
                    {"tree", "tree" },
                    {"overlook", "overlook" },
                    {"", "none"}
                } 
            }
        };

        // Get dropspots not in dictionary and returns a dictionary of the dropspot id and name
        public Dictionary<string, string> GetFreeDropspots(Dictionary<string, string> lobby)
        {
            // New Dictionary to store the free dropspots
            Dictionary<string, string> freeDropspots = new();

            // Loop over each dropspot
            foreach (KeyValuePair<string, string> dropspot in Dropspots[lobby["map_name"]])
            {
                // If the dropspot is not in the dictionary
                if (!lobby.ContainsValue(dropspot.Key))
                {
                    // Add to the free dropspots
                    freeDropspots[dropspot.Key] = dropspot.Value;
                }
            }

            return freeDropspots;
        }

        // Array of all the map names
        public Dictionary<string, string> GetDropspotMatches(string mapName, Dictionary<string, string> dropspotNames)
        {
            // New Dictionary to store the matches
            Dictionary<string, string> matches = new();

            // Preprocess the map name
            mapName = mapName.ToLower();
            mapName = mapName.Replace(" ", "");

            // Find the most similar map name
            Debug.WriteLine(DropspotTerms.Keys.ToList());
            string matchedMapName = FindMostSimilar(mapName, DropspotTerms.Keys.ToList());

            // Add to the matches
            matches["map_name"] = matchedMapName;

            // Loop over each dropspot name
            foreach (KeyValuePair<string, string> dropspotName in dropspotNames)
            {
                // Preprocessing

                // Make lowercase
                string newDropspotName = dropspotName.Value.ToLower();

                // Remove "team"
                newDropspotName = newDropspotName.Replace("team", "");

                // Remove any non alphabetical characters
                newDropspotName = new string(newDropspotName.Where(c => char.IsLetter(c)).ToArray());

                // Find most similar dropspot name
                newDropspotName = FindMostSimilar(newDropspotName, DropspotTerms[matchedMapName].Keys.ToList());

                // Add to the matches
                matches[dropspotName.Key] = DropspotTerms[matchedMapName][newDropspotName];
            }

            return matches;
        }
        public static string FindMostSimilar(string input, List<string> names)
        {
            string closestMatch = null;
            int smallestDistance = int.MaxValue;

            foreach (string name in names)
            {
                int distance = LevenshteinDistance(input, name);

                if (distance < smallestDistance)
                {
                    closestMatch = name;
                    smallestDistance = distance;
                }
            }

            return closestMatch;
        }

        public static int LevenshteinDistance(string s, string t)
        {
            int n = s.Length;
            int m = t.Length;
            int[,] d = new int[n + 1, m + 1];

            if (n == 0) return m;
            if (m == 0) return n;

            for (int i = 0; i <= n; i++)
            {
                d[i, 0] = i;
            }

            for (int j = 0; j <= m; j++)
            {
                d[0, j] = j;
            }

            for (int j = 1; j <= m; j++)
            {
                for (int i = 1; i <= n; i++)
                {
                    if (s[i - 1] == t[j - 1])
                    {
                        d[i, j] = d[i - 1, j - 1];
                    }
                    else
                    {
                        d[i, j] = Math.Min(Math.Min(d[i - 1, j] + 1, d[i, j - 1] + 1), d[i - 1, j - 1] + 1);
                    }
                }
            }

            return d[n, m];
        }

    }
}
