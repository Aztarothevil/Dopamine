using Digimezzo.Foundation.Core.Utils;
using Dopamine.Data;
using Prism.Mvvm;
using System.Collections.Generic;
using System.Linq;

namespace Dopamine.Services.Entities
{
    public class SongViewModel : BindableBase
    {
        private string songTitle;
        private string albumTitle;
        private string albumArtist;
        private IList<string> albumArtists;
        private string year;
        private string artworkPath;
        private string mainHeader;
        private string subHeader;
        private long? dateAdded;
        private long? dateFileCreated;
        private long sortYear;

        public SongViewModel(AlbumData albumData)
        {
            this.albumArtist = this.GetAlbumArtist(albumData);
            this.albumTitle = !string.IsNullOrEmpty(albumData.AlbumTitle) ? albumData.AlbumTitle : ResourceUtils.GetString("Language_Unknown_Album");
            this.albumArtists = this.GetAlbumArtists(albumData);
            this.year = albumData.Year.HasValue && albumData.Year.Value > 0 ? albumData.Year.Value.ToString() : string.Empty;
            this.SortYear = albumData.Year.HasValue ? albumData.Year.Value : 0;
            this.AlbumKey = albumData.AlbumKey;
            this.DateAdded = albumData.DateAdded;
            this.DateFileCreated = albumData.DateFileCreated;
            this.songTitle = albumData.TrackTitle;
            this.path = albumData.Path;
        }

        private string GetAlbumArtist(AlbumData albumData)
        {
            if (!string.IsNullOrEmpty(albumData.AlbumTitle))
            {
                if (!string.IsNullOrEmpty(albumData.AlbumArtists))
                {
                    return DataUtils.GetCommaSeparatedColumnMultiValue(albumData.AlbumArtists);
                }
                else if (!string.IsNullOrEmpty(albumData.Artists))
                {
                    return DataUtils.GetCommaSeparatedColumnMultiValue(albumData.Artists);
                }
            }

            return ResourceUtils.GetString("Language_Unknown_Artist");
        }

        public List<string> GetAlbumArtists(AlbumData albumData)
        {
            if (!string.IsNullOrEmpty(albumData.AlbumArtists))
            {
                return DataUtils.SplitAndTrimColumnMultiValue(albumData.AlbumArtists).ToList();
            }
            else if (!string.IsNullOrEmpty(albumData.Artists))
            {
                return DataUtils.SplitAndTrimColumnMultiValue(albumData.Artists).ToList();
            }

            return new List<string>();
        }

        public string SongTitle
        {
            get { return this.songTitle; }
            set
            {
                SetProperty<string>(ref this.songTitle, value);
                RaisePropertyChanged(nameof(this.HasTitle));
            }
        }

        public string AlbumKey { get; set; }

        public long? DateAdded
        {
            get { return this.dateAdded; }
            set
            {
                SetProperty<long?>(ref this.dateAdded, value);
            }
        }

        public long? DateFileCreated
        {
            get { return this.dateFileCreated; }
            set
            {
                SetProperty<long?>(ref this.dateFileCreated, value);
            }
        }

        public double Opacity { get; set; }

        public bool HasCover
        {
            get { return !string.IsNullOrEmpty(this.artworkPath); }
        }

        public bool HasTitle
        {
            get { return !string.IsNullOrEmpty(this.AlbumTitle); }
        }

        public string ToolTipYear
        {
            get { return !string.IsNullOrEmpty(this.year) ? "(" + this.year + ")" : string.Empty; }
        }

        public string Year
        {
            get { return this.year; }
            set
            {
                SetProperty<string>(ref this.year, value);
                RaisePropertyChanged(nameof(this.ToolTipYear));
            }
        }

        public long SortYear
        {
            get { return this.sortYear; }
            set
            {
                SetProperty<long>(ref this.sortYear, value);
            }
        }

        public string AlbumTitle
        {
            get { return this.albumTitle; }
            set
            {
                SetProperty<string>(ref this.albumTitle, value);
                RaisePropertyChanged(nameof(this.HasTitle));
            }
        }

        public string AlbumArtist
        {
            get { return this.albumArtist; }
            set { SetProperty<string>(ref this.albumArtist, value); }
        }

        public IList<string> AlbumArtists
        {
            get { return this.albumArtists; }
            set { SetProperty<IList<string>>(ref this.albumArtists, value); }
        }

        public string ArtworkPath
        {
            get { return this.artworkPath; }
            set
            {
                SetProperty<string>(ref this.artworkPath, value);
                RaisePropertyChanged(nameof(this.HasCover));
            }
        }

        public string MainHeader
        {
            get { return this.mainHeader; }
            set { SetProperty<string>(ref this.mainHeader, value); }
        }

        public string SubHeader
        {
            get { return this.subHeader; }
            set { SetProperty<string>(ref this.subHeader, value); }
        }

        public override string ToString()
        {
            return this.albumTitle;
        }

        public override bool Equals(object obj)
        {
            if (obj == null || !GetType().Equals(obj.GetType()) || this.AlbumKey == null)
            {
                return false;
            }
            return this.AlbumKey.Equals(((SongViewModel)obj).AlbumKey);
        }

        public override int GetHashCode()
        {
            return this.songTitle.GetHashCode();
        }

        /*Song*/

        private bool isPlaying;

        public bool IsPlaying
        {
            get { return this.isPlaying; }
            set { SetProperty<bool>(ref this.isPlaying, value); }
        }

        private string fileName;

        public string FileName
        {
            get { return this.fileName; }
            set { SetProperty<string>(ref this.fileName, value); }
        }

        private string path;

        public string Path
        {
            get { return this.path; }
            set { SetProperty<string>(ref this.path, value); }
        }
    }
}