﻿namespace Dopamine.Data
{
    public class AlbumData
    {
        public string AlbumTitle { get; set; }

        public string AlbumArtists { get; set; }

        public string TrackTitle { get; set; }

        public string Artists { get; set; }

        public string AlbumKey { get; set; }

        public string Path { get; set; }

        public long? Year { get; set; }

        public long? DateFileCreated { get; set; }

        public long? DateFileModified { get; set; }        

        public long? DateAdded { get; set; }

        public static AlbumData CreateDefault()
        {
            return new AlbumData()
            {
                AlbumTitle = string.Empty,
                AlbumArtists = string.Empty,
                TrackTitle = string.Empty,
                Artists = string.Empty,
                AlbumKey = string.Empty,
                Path = string.Empty,
                Year = 0,
                DateFileCreated = 0,
                DateFileModified = 0,
                DateAdded = 0
            };
        }
    }
}
