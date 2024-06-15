using Digimezzo.Foundation.Core.Logging;
using Digimezzo.Foundation.Core.Settings;
using Digimezzo.Foundation.Core.Utils;
using Dopamine.Core.Base;
using Dopamine.Core.Prism;
using Dopamine.Data;
using Dopamine.Services.Collection;
using Dopamine.Services.Dialog;
using Dopamine.Services.Entities;
using Dopamine.Services.Folders;
using Dopamine.Services.Indexing;
using Dopamine.Services.Playback;
using Dopamine.Services.Playlist;
using Dopamine.Services.Search;
using Dopamine.Services.Utils;
using Dopamine.ViewModels.Common.Base;
using Prism.Commands;
using Prism.Events;
using Prism.Ioc;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace Dopamine.ViewModels.FullPlayer.Collection
{
    public class CollectionMusicViewModel : SongsViewModelBase
    {
        private ICollectionService collectionService;
        private IPlaybackService playbackService;
        private IPlaylistService playlistService;
        private IIndexingService indexingService;
        private IDialogService dialogService;
        private ISearchService searchService;
        private IEventAggregator eventAggregator;
        private ObservableCollection<FolderViewModel> folder;
        private CollectionViewSource folderCvs;
        private IList<string> selectedMusic;
        private ObservableCollection<ISemanticZoomSelector> musicZoomSelectors;
        private bool isMusicZoomVisible;
        private long folderCount;
        private long songsCount;
        private double leftPaneWidthPercent;
        private double rightPaneWidthPercent;
        private MusicType musicType;
        private string folderTypeText;

        public DelegateCommand<string> AddMusicToPlaylistCommand { get; set; }

        public DelegateCommand<object> SelectedMusicCommand { get; set; }

        public DelegateCommand ShowMusicZoomCommand { get; set; }

        public DelegateCommand<string> SemanticJumpCommand { get; set; }

        public DelegateCommand AddMusicToNowPlayingCommand { get; set; }

        public DelegateCommand ShuffleSelectedMusicCommand { get; set; }

        public MusicType MusicType
        {
            get { return this.musicType; }
            set
            {
                SetProperty<MusicType>(ref this.musicType, value);
                //this.UpdateFolderTypeText();
            }
        }


        public string FolderTypeText
        {
            get { return this.folderTypeText; }
        }

        public double LeftPaneWidthPercent
        {
            get { return this.leftPaneWidthPercent; }
            set
            {
                SetProperty<double>(ref this.leftPaneWidthPercent, value);
                SettingsClient.Set<int>("ColumnWidths", "MusicLeftPaneWidthPercent", Convert.ToInt32(value));
            }
        }

        public double RightPaneWidthPercent
        {
            get { return this.rightPaneWidthPercent; }
            set
            {
                SetProperty<double>(ref this.rightPaneWidthPercent, value);
                SettingsClient.Set<int>("ColumnWidths", "MusicRightPaneWidthPercent", Convert.ToInt32(value));
            }
        }

        public ObservableCollection<FolderViewModel> Folder
        {
            get { return this.folder; }
            set { SetProperty<ObservableCollection<FolderViewModel>>(ref this.folder, value); }
        }

        public CollectionViewSource FolderCvs
        {
            get { return this.folderCvs; }
            set { SetProperty<CollectionViewSource>(ref this.folderCvs, value); }
        }

        public IList<string> SelectedMusic
        {
            get { return this.selectedMusic; }
            set { SetProperty<IList<string>>(ref this.selectedMusic, value); }
        }

        public long FolderCount
        {
            get { return this.folderCount; }
            set { SetProperty<long>(ref this.folderCount, value); }
        }

        public long SongsCount
        {
            get { return this.songsCount; }
            set { SetProperty<long>(ref this.songsCount, value); }
        }

        public bool IsMusicZoomVisible
        {
            get { return this.isMusicZoomVisible; }
            set { SetProperty<bool>(ref this.isMusicZoomVisible, value); }
        }

        public bool HasSelectedMusic
        {
            get
            {
                return (this.SelectedMusic != null && this.SelectedMusic.Count > 0);
            }
        }

        public CollectionMusicViewModel(IContainerProvider container) : base(container)
        {
            // Dependency injection
            this.collectionService = container.Resolve<ICollectionService>();
            this.playbackService = container.Resolve<IPlaybackService>();
            this.playlistService = container.Resolve<IPlaylistService>();
            this.indexingService = container.Resolve<IIndexingService>();
            this.dialogService = container.Resolve<IDialogService>();
            this.searchService = container.Resolve<ISearchService>();
            this.eventAggregator = container.Resolve<IEventAggregator>();

            // Commands
            this.ToggleTrackOrderCommand = new DelegateCommand(async () => await this.ToggleTrackOrderAsync());
            this.ToggleMusicOrderCommand = new DelegateCommand(async () => await this.ToggleMusicOrderAsync());
            this.RemoveSelectedTracksCommand = new DelegateCommand(async () => await this.RemoveTracksFromCollectionAsync(this.SelectedTracks), () => !this.IsIndexing);
            this.AddMusicToPlaylistCommand = new DelegateCommand<string>(async (playlistName) => await this.AddMusicToPlaylistAsync(this.SelectedMusic, playlistName));
            this.SelectedMusicCommand = new DelegateCommand<object>(async (parameter) => await this.SelectedMusicHandlerAsync(parameter));
            this.AddMusicToNowPlayingCommand = new DelegateCommand(async () => await this.AddMusicToNowPlayingAsync(this.SelectedMusic));
            this.ShuffleSelectedMusicCommand = new DelegateCommand(async () => await this.playbackService.EnqueueArtistsAsync(this.SelectedMusic, true, false));
            this.ClearNowPlayingListCommand = new DelegateCommand(async () => await this.ClearNowPlayingListAsync());

            this.SemanticJumpCommand = new DelegateCommand<string>((header) =>
            {
                this.HideSemanticZoom();
                this.eventAggregator.GetEvent<PerformSemanticJump>().Publish(new Tuple<string, string>("Artists", header));
            });

            // Settings
            SettingsClient.SettingChanged += async (_, e) =>
            {
                if (SettingsClient.IsSettingChanged(e, "Behaviour", "EnableRating"))
                {
                    this.EnableRating = (bool)e.Entry.Value;
                    this.SetTrackOrder("ArtistsTrackOrder");
                    //await this.GetFolderAsync(this.SelectedMusic, this.TrackOrder);
                }

                if (SettingsClient.IsSettingChanged(e, "Behaviour", "EnableLove"))
                {
                    this.EnableLove = (bool)e.Entry.Value;
                    this.SetTrackOrder("ArtistsTrackOrder");
                    //await this.GetFolderAsync(this.SelectedMusic, this.TrackOrder);
                }
            };

            // PubSub Events
            this.eventAggregator.GetEvent<ShellMouseUp>().Subscribe((_) => this.IsMusicZoomVisible = false);
            this.eventAggregator.GetEvent<ToggleArtistOrderCommand>().Subscribe((_) => this.ToggleArtistTypeAsync());

            // Set the initial ArtistOrder			
            //this.MusicType = (MusicType)SettingsClient.Get<int>("Ordering", "ArtistsArtistType");
            this.MusicType = MusicType.Folder;

            // Set the initial AlbumOrder
            this.MusicOrder = MusicOrder.None;

            // Set the initial TrackOrder
            this.SetTrackOrder("ArtistsTrackOrder");

            // Set width of the panels
            this.LeftPaneWidthPercent = SettingsClient.Get<int>("ColumnWidths", "MusicLeftPaneWidthPercent");
            this.RightPaneWidthPercent = SettingsClient.Get<int>("ColumnWidths", "MusicRightPaneWidthPercent");

            // Cover size
            this.SetCoversizeAsync((CoverSizeType)SettingsClient.Get<int>("CoverSizes", "ArtistsCoverSize"));

            this.folderTypeText = ResourceUtils.GetString("Language_All_Folders");

            RaisePropertyChanged(nameof(this.FolderTypeText));
        }

        public void HideSemanticZoom()
        {
            this.IsMusicZoomVisible = false;
        }

        public void UpdateSemanticZoomHeaders()
        {
            string previousHeader = string.Empty;

            foreach (FolderViewModel fvm in this.FolderCvs.View)
            {
                if (string.IsNullOrEmpty(previousHeader) || !fvm.Header.Equals(previousHeader))
                {
                    previousHeader = fvm.Header;
                    fvm.IsHeader = true;
                }
                else
                {
                    fvm.IsHeader = false;
                }
            }
        }

        private void SetArtistOrder(string settingName)
        {
            this.MusicType = (MusicType)SettingsClient.Get<int>("Ordering", settingName);
        }

        private void ClearMusic()
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                if (this.FolderCvs != null)
                {
                    this.FolderCvs.Filter -= new FilterEventHandler(FolderCvs_Filter);
                }

                this.FolderCvs = null;
            });

            this.folder = null;
        }

        private async Task GetMusicAsync(MusicType musicType)
        {
            try
            {
                // Get the music
                var folderViewModels = new ObservableCollection<FolderViewModel>(await this.collectionService.GetAllFoldersAsync());

                // Unbind to improve UI performance
                this.ClearMusic();

                // Populate ObservableCollection
                this.Folder = new ObservableCollection<FolderViewModel>(folderViewModels);
            }
            catch (Exception ex)
            {
                LogClient.Error("An error occurred while getting Music. Exception: {0}", ex.Message);

                // Failed getting Music. Create empty ObservableCollection.
                this.Folder = new ObservableCollection<FolderViewModel>();
            }

            Application.Current.Dispatcher.Invoke(() =>
            {
                // Populate CollectionViewSource
                this.FolderCvs = new CollectionViewSource { Source = this.Folder };
                this.FolderCvs.Filter += new FilterEventHandler(FolderCvs_Filter);

                // Update count
                this.FolderCount = ((ListCollectionView)this.FolderCvs.View).Count;
            });

            // Update Semantic Zoom Headers
            //this.UpdateSemanticZoomHeaders();
        }

        private async Task SelectedMusicHandlerAsync(object parameter)
        {
            if (parameter != null)
            {
                this.SelectedMusic = new List<string>();

                foreach (FolderViewModel item in (IList)parameter)
                {
                    this.SelectedMusic.Add(item.Directory);
                }
            }

            this.RaisePropertyChanged(nameof(this.HasSelectedMusic));
            this.MusicOrder = MusicOrder.Alphabetical;
            await this.GetFolderAlbumAsync(this.SelectedMusic, this.MusicType, this.MusicOrder);
            this.SetTrackOrder("ArtistsTrackOrder");
            //await this.GetFolderAsync(this.SelectedMusic, this.TrackOrder);
            this.SongsCount = this.Songs.Count;
        }

        private async Task AddMusicToPlaylistAsync(IList<string> music, string playlistName)
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
            if (playlistName == null) return;

            // Verify if the playlist was added
            switch (addPlaylistResult)
            {
                case CreateNewPlaylistResult.Success:
                case CreateNewPlaylistResult.Duplicate:
                    // Add items to playlist
                    AddTracksToPlaylistResult result = await this.playlistService.AddMusicToStaticPlaylistAsync(music, playlistName);

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

        private void FolderCvs_Filter(object sender, FilterEventArgs e)
        {
            FolderViewModel fvm = e.Item as FolderViewModel;

            e.Accepted = Services.Utils.EntityUtils.FilterFolder(fvm, this.searchService.SearchText);
        }

        private async Task AddMusicToNowPlayingAsync(IList<string> music)
        {
            EnqueueResult result = await this.playbackService.AddMusicToQueueAsync(music);

            if (!result.IsSuccess)
            {
                this.dialogService.ShowNotification(0xe711, 16, ResourceUtils.GetString("Language_Error"), ResourceUtils.GetString("Language_Error_Adding_Music_To_Now_Playing"), ResourceUtils.GetString("Language_Ok"), true, ResourceUtils.GetString("Language_Log_File"));
            }
        }

        private async Task ToggleArtistTypeAsync()
        {
            this.HideSemanticZoom();

            switch (this.MusicType)
            {
                case MusicType.All:
                    this.MusicType = MusicType.Folder;
                    break;
                case MusicType.Track:
                    this.MusicType = MusicType.Track;
                    break;
                default:
                    // Cannot happen, but just in case.
                    this.MusicType = MusicType.Folder;
                    break;
            }

            SettingsClient.Set<int>("Ordering", "MusicArtistType", (int)this.MusicType);
            await this.GetMusicAsync(this.MusicType);
        }

        private async Task ToggleTrackOrderAsync()
        {
            base.ToggleTrackOrder();

            SettingsClient.Set<int>("Ordering", "ArtistsTrackOrder", (int)this.TrackOrder);
            await this.GetTracksCommonAsync(this.Tracks, this.TrackOrder);
        }

        private async Task ToggleMusicOrderAsync()
        {

            base.ToggleMusicOrder();

            //SettingsClient.Set<int>("Ordering", "ArtistsAlbumOrder", (int)this.MusicOrder);
            await this.GetAlbumsCommonAsync(this.Songs, this.MusicOrder);
        }

        protected async override Task SetCoversizeAsync(CoverSizeType coverSize)
        {
            await base.SetCoversizeAsync(coverSize);
            SettingsClient.Set<int>("CoverSizes", "MusicCoverSize", (int)coverSize);
        }

        protected async override Task FillListsAsync()
        {
            await this.GetMusicAsync(this.MusicType);
            await this.GetFolderAlbumAsync(this.SelectedMusic, this.MusicType, this.MusicOrder);
            //await this.GetFolderAsync(this.SelectedMusic, this.TrackOrder);
            this.SongsCount = this.Songs.Count;
        }

        protected async override Task EmptyListsAsync()
        {
            //this.ClearMusic();
            this.ClearAlbums();
            this.ClearTracks();
        }

        protected override void FilterLists()
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                // Music
                if (this.FolderCvs != null)
                {
                    this.FolderCvs.View.Refresh();
                    this.FolderCount = this.FolderCvs.View.Cast<ISemanticZoomable>().Count();
                    this.UpdateSemanticZoomHeaders();
                }
            });

            base.FilterLists();
        }

        protected async override Task SelectedAlbumsHandlerAsync(object parameter)
        {
            await base.SelectedAlbumsHandlerAsync(parameter);

            this.SetTrackOrder("ArtistsTrackOrder");
            //await this.GetFolderAsync(this.SelectedMusic, this.TrackOrder);
        }

        protected override void RefreshLanguage()
        {
            this.UpdateMusicOrderText(this.MusicOrder);
            this.UpdateTrackOrderText(this.TrackOrder);
            base.RefreshLanguage();
        }
    }
}
