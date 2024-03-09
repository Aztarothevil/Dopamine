using Digimezzo.Foundation.Core.Logging;
using Digimezzo.Foundation.Core.Utils;
using Dopamine.Core.Base;
using Dopamine.Core.Extensions;
using Dopamine.Data;
using Dopamine.Data.Entities;
using Dopamine.Data.Repositories;
using Dopamine.Services.Cache;
using Dopamine.Services.Collection;
using Dopamine.Services.Dialog;
using Dopamine.Services.Entities;
using Dopamine.Services.Indexing;
using Dopamine.Services.Playback;
using Dopamine.Services.Playlist;
using Dopamine.Services.Search;
using Dopamine.Views.Common;
using Prism.Commands;
using Prism.Ioc;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace Dopamine.ViewModels.Common.Base
{
    public abstract class SongsViewModelBase : TracksViewModelBase
    {
        private IContainerProvider container;
        private ICollectionService collectionService;
        private IPlaybackService playbackService;
        private IDialogService dialogService;
        private ISearchService searchService;
        private IPlaylistService playlistService;
        private ICacheService cacheService;
        private IIndexingService indexingService;
        private IAlbumArtworkRepository albumArtworkRepository;
        private ObservableCollection<SongViewModel> songs;
        private CollectionViewSource songsCvs;
        private IList<SongViewModel> selectedAlbums;
        private bool delaySelectedAlbums;
        private long songsCount;
        private MusicOrder musicOrder;
        private string musicOrderText;
        private double coverSize;
        private double albumWidth;
        private double albumHeight;
        private CoverSizeType selectedCoverSize;

        public DelegateCommand ToggleMusicOrderCommand { get; set; }

        public DelegateCommand<string> AddAlbumsToPlaylistCommand { get; set; }

        public DelegateCommand<object> SelectedAlbumsCommand { get; set; }

        public DelegateCommand EditAlbumCommand { get; set; }

        public DelegateCommand AddAlbumsToNowPlayingCommand { get; set; }

        public DelegateCommand<string> SetCoverSizeCommand { get; set; }

        public DelegateCommand DelaySelectedAlbumsCommand { get; set; }

        public DelegateCommand ShuffleSelectedAlbumsCommand { get; set; }

        public double UpscaledCoverSize => this.CoverSize * Constants.CoverUpscaleFactor;

        public bool IsSmallCoverSizeSelected => this.selectedCoverSize == CoverSizeType.Small;

        public bool IsMediumCoverSizeSelected => this.selectedCoverSize == CoverSizeType.Medium;

        public bool IsLargeCoverSizeSelected => this.selectedCoverSize == CoverSizeType.Large;

        public string MusicOrderText => this.musicOrderText;

        public double CoverSize
        {
            get { return this.coverSize; }
            set { SetProperty<double>(ref this.coverSize, value); }
        }

        public double AlbumWidth
        {
            get { return this.albumWidth; }
            set { SetProperty<double>(ref this.albumWidth, value); }
        }

        public double AlbumHeight
        {
            get { return this.albumHeight; }
            set { SetProperty<double>(ref this.albumHeight, value); }
        }

        public ObservableCollection<SongViewModel> Songs
        {
            get { return this.songs; }
            set { SetProperty<ObservableCollection<SongViewModel>>(ref this.songs, value); }
        }

        public CollectionViewSource SongsCvs
        {
            get { return this.songsCvs; }
            set { SetProperty<CollectionViewSource>(ref this.songsCvs, value); }
        }

        public IList<SongViewModel> SelectedAlbums
        {
            get { return this.selectedAlbums; }
            set
            {
                SetProperty<IList<SongViewModel>>(ref this.selectedAlbums, value);
            }
        }

        public long SongsCount
        {
            get { return this.songsCount; }
            set { SetProperty<long>(ref this.songsCount, value); }
        }

        public MusicOrder MusicOrder
        {
            get { return this.musicOrder; }
            set
            {
                SetProperty<MusicOrder>(ref this.musicOrder, value);

                this.UpdateMusicOrderText(value);
            }
        }

        public SongsViewModelBase(IContainerProvider container) : base(container)
        {
            // Dependency injection
            //this.container = container;
            this.collectionService = container.Resolve<ICollectionService>();
            //this.playbackService = container.Resolve<IPlaybackService>();
            //this.dialogService = container.Resolve<IDialogService>();
            this.searchService = container.Resolve<ISearchService>();
            //this.playlistService = container.Resolve<IPlaylistService>();
            this.cacheService = container.Resolve<ICacheService>();
            //this.indexingService = container.Resolve<IIndexingService>();
            this.albumArtworkRepository = container.Resolve<IAlbumArtworkRepository>();

            // Commands
            //this.ToggleAlbumOrderCommand = new DelegateCommand(() => this.ToggleAlbumOrder());
            //this.ShuffleSelectedAlbumsCommand = new DelegateCommand(async () => await this.playbackService.EnqueueSongsAsync(this.SelectedAlbums, null, true, false));
            //this.AddAlbumsToPlaylistCommand = new DelegateCommand<string>(async (playlistName) => await this.AddAlbumsToPlaylistAsync(this.SelectedAlbums, playlistName));
            //this.EditAlbumCommand = new DelegateCommand(() => this.EditSelectedAlbum(), () => !this.IsIndexing);
            //this.AddAlbumsToNowPlayingCommand = new DelegateCommand(async () => await this.AddAlbumsToNowPlayingAsync(this.SelectedAlbums));
            //this.DelaySelectedAlbumsCommand = new DelegateCommand(() => this.delaySelectedAlbums = true);

            // Events
            //this.indexingService.AlbumArtworkAdded += async (_, e) => await this.RefreshAlbumArtworkAsync(e.AlbumKeys);

            //this.SelectedAlbumsCommand = new DelegateCommand<object>(async (parameter) =>
            //{
            //    if (this.delaySelectedAlbums)
            //    {
            //        await Task.Delay(Constants.DelaySelectedAlbumsDelay);
            //    }

            //    this.delaySelectedAlbums = false;
            //    await this.SelectedAlbumsHandlerAsync(parameter);
            //});

            //this.SetCoverSizeCommand = new DelegateCommand<string>(async (coverSize) =>
            //{
            //    if (int.TryParse(coverSize, out int selectedCoverSize))
            //    {
            //        await this.SetCoversizeAsync((CoverSizeType)selectedCoverSize);
            //    }
            //});
        }

        public async Task LoadAlbumArtworkAsync(int delayMilliSeconds)
        {
            await Task.Delay(delayMilliSeconds);

            IList<AlbumArtwork> allAlbumArtwork = await this.albumArtworkRepository.GetAlbumArtworkAsync();

            await this.SetAlbumArtwork(allAlbumArtwork);
        }

        public async Task RefreshAlbumArtworkAsync(IList<string> albumsKeys = null)
        {
            IList<AlbumArtwork> allAlbumArtwork = await this.albumArtworkRepository.GetAlbumArtworkAsync();

            await this.SetAlbumArtwork(allAlbumArtwork, albumsKeys);
        }

        private async Task SetAlbumArtwork(IList<AlbumArtwork> allAlbumArtwork, IList<string> albumsKeys = null)
        {
            if (this.songs != null && this.songs.Count > 0)
            {
                await Task.Run(() =>
                {
                    foreach (SongViewModel alb in this.songs)
                    {
                        try
                        {
                            if (allAlbumArtwork != null && allAlbumArtwork.Count > 0 && albumsKeys != null ? albumsKeys.Contains(alb.AlbumKey) : true)
                            {
                                AlbumArtwork albumArtwork = allAlbumArtwork.Where(a => a.AlbumKey.Equals(alb.AlbumKey)).FirstOrDefault();

                                if (albumArtwork != null)
                                {
                                    alb.ArtworkPath = this.cacheService.GetCachedArtworkPath(albumArtwork.ArtworkID);
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            LogClient.Error("Error while refreshing artwork for Album {0}/{1}. Exception: {2}", alb.AlbumTitle, alb.AlbumArtist, ex.Message);
                        }
                    }
                });
            }
        }

        private void EditSelectedAlbum()
        {
            if (this.SelectedAlbums == null || this.SelectedAlbums.Count == 0)
            {
                return;
            }

            EditAlbum view = this.container.Resolve<EditAlbum>();
            view.DataContext = this.container.Resolve<Func<SongViewModel, EditAlbumViewModel>>()(this.SelectedAlbums.First());

            this.dialogService.ShowCustomDialog(
                0xe104,
                14,
                ResourceUtils.GetString("Language_Edit_Album"),
                view,
                405,
                450,
                false,
                true,
                true,
                true,
                ResourceUtils.GetString("Language_Ok"),
                ResourceUtils.GetString("Language_Cancel"),
                ((EditAlbumViewModel)view.DataContext).SaveAlbumAsync);
        }

        private void SongsCvs_Filter(object sender, FilterEventArgs e)
        {
            SongViewModel avm = e.Item as SongViewModel;
            e.Accepted = Services.Utils.EntityUtils.FilterSongs(avm, this.searchService.SearchText);
        }

        protected void UpdateMusicOrderText(MusicOrder musicOrder)
        {
            switch (musicOrder)
            {
                case MusicOrder.Alphabetical:
                    this.musicOrderText = ResourceUtils.GetString("Language_Toggle_Track_Order_ByDate");
                    break;
                case MusicOrder.ByDateCreated:
                    this.musicOrderText = ResourceUtils.GetString("Language_By_Date_Created");
                    break;
                case MusicOrder.ReverseByDateCreated:
                    this.musicOrderText = ResourceUtils.GetString("Language_By_Reverse_Date_Created");
                    break;
                default:
                    // Cannot happen, but just in case.
                    this.musicOrderText = ResourceUtils.GetString("Language_By_Reverse_Date_Created");
                    break;
            }

            RaisePropertyChanged(nameof(this.musicOrderText));
        }

        protected async Task GetFolderAlbumAsync(IList<string> folders, MusicType musicType, MusicOrder musicOrder)
        {
            if (!folders.IsNullOrEmpty())
            {
                IList<SongViewModel> tracks = null;
                tracks = await this.collectionService.GetFolderAlbumAsync(folders, musicType);
                List<SongViewModel> trackRemove = new List<SongViewModel>() { };
                
                if (folders != null)
                {
                    foreach (var track in tracks)
                    {
                        foreach (var folder in folders)
                        {
                            if (!Path.GetDirectoryName(track.Path).Contains(folder))
                            {
                                trackRemove.Add(track);
                            }
                        }
                    }
                    foreach (var track in trackRemove)
                    {
                        tracks.Remove(track);
                    }
                }

                await this.GetSongsCommonAsync(tracks, musicOrder);

                return;
            }

            await this.GetSongsCommonAsync(await this.collectionService.GetAllMusicAsync(), musicOrder);
        }

        protected async Task GetMusicAsync(IList<string> selectedArtists, MusicType musicType, MusicOrder albumOrder)
        {
            if (!selectedArtists.IsNullOrEmpty())
            {
                await this.GetSongsCommonAsync(await this.collectionService.GetFolderAlbumAsync(selectedArtists, musicType), albumOrder);

                return;
            }

            await this.GetSongsCommonAsync(await this.collectionService.GetAllMusicAsync(), albumOrder);
        }

        protected async Task GetGenreAlbumsAsync(IList<string> selectedGenres, MusicOrder musicOrder)
        {
            if (!selectedGenres.IsNullOrEmpty())
            {
                await this.GetSongsCommonAsync(await this.collectionService.GetGenreMusicAsync(selectedGenres), musicOrder);

                return;
            }

            await this.GetSongsCommonAsync(await this.collectionService.GetAllMusicAsync(), musicOrder);
        }

        protected async Task GetAllAlbumsAsync(MusicOrder musicOrder)
        {
            await this.GetSongsCommonAsync(await this.collectionService.GetAllMusicAsync(), musicOrder);
        }

        protected void ClearAlbums()
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                if (this.SongsCvs != null)
                {
                    this.SongsCvs.Filter -= new FilterEventHandler(SongsCvs_Filter);
                }

                this.SongsCvs = null;
            });

            this.SongsCvs = null;
        }

        protected async Task GetSongsCommonAsync(IList<SongViewModel> albums, MusicOrder musicOrder)
        {
            try
            {
                // Order the incoming Albums
                IList<SongViewModel> orderedAlbums = await this.collectionService.OrderSongsAsync(albums, musicOrder);

                // Create new ObservableCollection
                var albumViewModels = new ObservableCollection<SongViewModel>(orderedAlbums);

                // Unbind to improve UI performance
                this.ClearAlbums();

                // Populate ObservableCollection
                this.Songs = albumViewModels;
            }
            catch (Exception ex)
            {
                LogClient.Error("An error occurred while getting Albums. Exception: {0}", ex.Message);

                // Failed getting Albums. Create empty ObservableCollection.
                Application.Current.Dispatcher.Invoke(() =>
                {
                    this.Songs = new ObservableCollection<SongViewModel>();
                });
            }

            Application.Current.Dispatcher.Invoke(() =>
            {
                // Populate CollectionViewSource
                this.SongsCvs = new CollectionViewSource { Source = this.Songs };
                this.SongsCvs.Filter += new FilterEventHandler(SongsCvs_Filter);

                // Update count
                this.SongsCount = this.SongsCvs.View.Cast<SongViewModel>().Count();
            });

            // Set Album artwork
            this.LoadAlbumArtworkAsync(Constants.ArtworkLoadDelay);
        }

        protected async Task AddAlbumsToPlaylistAsync(IList<SongViewModel> albumViewModels, string playlistName)
        {
            CreateNewPlaylistResult addPlaylistResult = CreateNewPlaylistResult.Success; // Default Success

            // If no playlist is provided, first create one.
            if (playlistName == null)
            {
                var responseText = ResourceUtils.GetString("Language_New_Playlist");

                if (this.dialogService.ShowInputDialog(
                    0xea37,
                    16,
                    ResourceUtils.GetString("Language_New_Playlist"),
                    ResourceUtils.GetString("Language_Enter_Name_For_Playlist"),
                    ResourceUtils.GetString("Language_Ok"),
                    ResourceUtils.GetString("Language_Cancel"),
                    ref responseText))
                {
                    playlistName = responseText;
                    addPlaylistResult = await this.playlistService.CreateNewPlaylistAsync(new EditablePlaylistViewModel(playlistName, PlaylistType.Static));
                }
            }

            // If playlist name is still null, the user clicked cancel on the previous dialog. Stop here.
            if (playlistName == null)
            {
                return;
            }

            // Verify if the playlist was added
            switch (addPlaylistResult)
            {
                case CreateNewPlaylistResult.Success:
                case CreateNewPlaylistResult.Duplicate:
                    // Add items to playlist
                    AddTracksToPlaylistResult result = await this.playlistService.AddMusicToStaticPlaylistAsync(albumViewModels, playlistName);

                    if (result == AddTracksToPlaylistResult.Error)
                    {
                        this.dialogService.ShowNotification(0xe711, 16, ResourceUtils.GetString("Language_Error"), ResourceUtils.GetString("Language_Error_Adding_Songs_To_Playlist").Replace("{playlistname}", "\"" + playlistName + "\""), ResourceUtils.GetString("Language_Ok"), true, ResourceUtils.GetString("Language_Log_File"));
                    }
                    break;
                case CreateNewPlaylistResult.Error:
                    this.dialogService.ShowNotification(
                        0xe711,
                        16,
                        ResourceUtils.GetString("Language_Error"),
                        ResourceUtils.GetString("Language_Error_Adding_Playlist"),
                        ResourceUtils.GetString("Language_Ok"),
                        true,
                        ResourceUtils.GetString("Language_Log_File"));
                    break;
                case CreateNewPlaylistResult.Blank:
                    this.dialogService.ShowNotification(
                        0xe711,
                        16,
                        ResourceUtils.GetString("Language_Error"),
                        ResourceUtils.GetString("Language_Provide_Playlist_Name"),
                        ResourceUtils.GetString("Language_Ok"),
                        false,
                        string.Empty);
                    break;
                default:
                    // Never happens
                    break;
            }
        }

        protected async Task AddAlbumsToNowPlayingAsync(IList<SongViewModel> albumViewModel)
        {
            EnqueueResult result = await this.playbackService.AddMusicToQueueAsync(albumViewModel);

            if (!result.IsSuccess)
            {
                this.dialogService.ShowNotification(0xe711, 16, ResourceUtils.GetString("Language_Error"), ResourceUtils.GetString("Language_Error_Adding_Albums_To_Now_Playing"), ResourceUtils.GetString("Language_Ok"), true, ResourceUtils.GetString("Language_Log_File"));
            }
        }

        protected async virtual Task SelectedAlbumsHandlerAsync(object parameter)
        {
            // This method needs to be awaitable for use in child classes

            if (parameter != null)
            {
                this.SelectedAlbums = new List<SongViewModel>();

                foreach (SongViewModel item in (IList)parameter)
                {
                    this.SelectedAlbums.Add(item);
                }
            }
        }

        protected override void SetEditCommands()
        {
            base.SetEditCommands();

            if (this.EditAlbumCommand != null)
            {
                this.EditAlbumCommand.RaiseCanExecuteChanged();
            }
        }

        protected override void FilterLists()
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                // Albums
                if (this.SongsCvs != null)
                {
                    this.SongsCvs.View.Refresh();
                    this.SongsCount = this.SongsCvs.View.Cast<SongViewModel>().Count();
                }
            });

            base.FilterLists();
        }

        protected virtual async Task SetCoversizeAsync(CoverSizeType coverSize)
        {
            await Task.Run(() =>
            {
                this.selectedCoverSize = coverSize;

                switch (coverSize)
                {
                    case CoverSizeType.Small:
                        this.CoverSize = Constants.CoverSmallSize;
                        break;
                    case CoverSizeType.Medium:
                        this.CoverSize = Constants.CoverMediumSize;
                        break;
                    case CoverSizeType.Large:
                        this.CoverSize = Constants.CoverLargeSize;
                        break;
                    default:
                        this.CoverSize = Constants.CoverMediumSize;
                        this.selectedCoverSize = CoverSizeType.Medium;
                        break;
                }

                // this.AlbumWidth = this.CoverSize + Constants.AlbumTilePadding.Left + Constants.AlbumTilePadding.Right + Constants.AlbumTileMargin.Left + Constants.AlbumTileMargin.Right;
                this.AlbumWidth = this.CoverSize + Constants.AlbumTileMargin.Left + Constants.AlbumTileMargin.Right;
                this.AlbumHeight = this.AlbumWidth + Constants.AlbumTileAlbumInfoHeight + Constants.AlbumSelectionBorderSize;

                RaisePropertyChanged(nameof(this.CoverSize));
                RaisePropertyChanged(nameof(this.AlbumWidth));
                RaisePropertyChanged(nameof(this.AlbumHeight));
                RaisePropertyChanged(nameof(this.UpscaledCoverSize));
                RaisePropertyChanged(nameof(this.IsSmallCoverSizeSelected));
                RaisePropertyChanged(nameof(this.IsMediumCoverSizeSelected));
                RaisePropertyChanged(nameof(this.IsLargeCoverSizeSelected));
            });
        }

        protected virtual void ToggleMusicOrder()
        {
            switch (this.musicOrder)
            {
                case MusicOrder.None:
                    this.MusicOrder = MusicOrder.ByDateCreated;
                    break;
                case MusicOrder.ByDateCreated:
                    this.MusicOrder = MusicOrder.ReverseByDateCreated;
                    break;
                case MusicOrder.ReverseByDateCreated:
                    this.MusicOrder = MusicOrder.ByDateCreated;
                    break;
                default:
                    this.MusicOrder = MusicOrder.ByDateCreated;
                    break;
            }
        }

        protected async Task GetAlbumsCommonAsync(IList<SongViewModel> songs, MusicOrder musicOrder)
        {
            try
            {
                // Order the incoming Albums
                IList<SongViewModel> orderedSong = await this.collectionService.OrderSongsAsync(songs, musicOrder);

                // Create new ObservableCollection
                var songViewModels = new ObservableCollection<SongViewModel>(orderedSong);

                // Unbind to improve UI performance
                this.ClearAlbums();

                // Populate ObservableCollection
                this.songs = songViewModels;
            }
            catch (Exception ex)
            {
                LogClient.Error("An error occurred while getting Albums. Exception: {0}", ex.Message);

                // Failed getting Albums. Create empty ObservableCollection.
                Application.Current.Dispatcher.Invoke(() =>
                {
                    this.songs = new ObservableCollection<SongViewModel>();
                });
            }

            Application.Current.Dispatcher.Invoke(() =>
            {
                // Populate CollectionViewSource
                this.SongsCvs = new CollectionViewSource { Source = this.Songs };
                this.SongsCvs.Filter += new FilterEventHandler(SongsCvs_Filter);

                // Update count
                this.SongsCount = this.SongsCvs.View.Cast<SongViewModel>().Count();
            });

            // Set Album artwork
            this.LoadAlbumArtworkAsync(Constants.ArtworkLoadDelay);
        }
    }
}
