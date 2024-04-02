using Digimezzo.Foundation.Core.Logging;
using Digimezzo.Foundation.Core.Settings;
using Digimezzo.Foundation.WPF.Controls;
using Dopamine.Core.Utils;
using Dopamine.Services.Appearance;
using Dopamine.Services.Entities;
using Dopamine.Services.Metadata;
using Dopamine.Services.Playback;
using Dopamine.Services.Scrobbling;
using Prism.Mvvm;
using System;
using System.Drawing;
using System.Threading.Tasks;
using System.Timers;

namespace Dopamine.ViewModels.Common
{
    public class PlaybackInfoMusicControlViewModel : BindableBase
    {
        private PlaybackInfoViewModel playbackInfoViewModel;
        private IPlaybackService playbackService;
        private IMetadataService metadataService;
        private IScrobblingService scrobblingService;
        private SlideDirection slideDirection;
        private IAppearanceService appearanceService;
        private TrackViewModel previousTrack;
        private TrackViewModel track;
        private Timer refreshTimer = new Timer();
        private int refreshTimerIntervalMilliseconds = 250;

        public PlaybackInfoViewModel PlaybackInfoViewModel
        {
            get { return this.playbackInfoViewModel; }
            set { SetProperty<PlaybackInfoViewModel>(ref this.playbackInfoViewModel, value); }
        }

        public SlideDirection SlideDirection
        {
            get { return this.slideDirection; }
            set { SetProperty<SlideDirection>(ref this.slideDirection, value); }
        }

        public PlaybackInfoMusicControlViewModel(IPlaybackService playbackService, IMetadataService metadataService, IScrobblingService scrobblingService, IAppearanceService appearanceService)
        {
            this.playbackService = playbackService;
            this.metadataService = metadataService;
            this.scrobblingService = scrobblingService;
            this.appearanceService = appearanceService;

            this.refreshTimer.Interval = this.refreshTimerIntervalMilliseconds;
            this.refreshTimer.Elapsed += RefreshTimer_Elapsed;

            this.playbackService.PlaybackSuccess += (_, e) =>
            {
                this.SlideDirection = e.IsPlayingPreviousTrack ? SlideDirection.UpToDown : SlideDirection.DownToUp;
                this.refreshTimer.Stop();
                this.refreshTimer.Start();
            };

            this.playbackService.PlayingTrackChanged += (_, __) => this.RefreshPlaybackInfoAsync(this.playbackService.CurrentTrack, true);

            // Defaults
            this.SlideDirection = SlideDirection.DownToUp;
            this.RefreshPlaybackInfoAsync(this.playbackService.CurrentTrack, false);
        }

        private void RefreshTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            this.refreshTimer.Stop();
            this.RefreshPlaybackInfoAsync(this.playbackService.CurrentTrack, false);
        }

        private void ClearPlaybackInfo()
        {
            this.PlaybackInfoViewModel = new PlaybackInfoViewModel
            {
                Title = string.Empty,
                Artist = string.Empty,
                Album = string.Empty,
                Year = string.Empty,
                CurrentTime = string.Empty,
                TotalTime = string.Empty
            };

            this.track = null;
        }

        private async void RefreshPlaybackInfoAsync(TrackViewModel track, bool allowRefreshingCurrentTrack)
        {
            await Task.Run(() =>
            {
                this.previousTrack = this.track;

                // No track selected: clear playback info.
                if (track == null)
                {
                    this.ClearPlaybackInfo();
                    return;
                }

                this.track = track;

                // The track didn't change: leave the previous playback info.
                if (!allowRefreshingCurrentTrack & this.track.Equals(this.previousTrack)) return;

                // The track changed: we need to show new playback info.
                try
                {
                    this.PlaybackInfoViewModel = new PlaybackInfoViewModel
                    {
                        Title = "        Title:  " + (string.IsNullOrEmpty(track.TrackTitle) ? track.FileName : track.TrackTitle),
                        Artist = "      Artist:  " + track.ArtistName,
                        Album = track.AlbumTitle,
                        Year = track.Year,
                        CurrentTime = FormatUtils.FormatTime(new TimeSpan(0)),
                        TotalTime = FormatUtils.FormatTime(new TimeSpan(0)),
                        Bitrate = "    Bitrate:  " + track.Bitrate,
                        Location = "       Path:  " + track.Path,
                        Type = "       Type:  " + track.Track.MimeType.Replace("taglib/","").ToUpper(),
                        Duration = "Duration:  " + track.Duration,
                        Size = "        Size:  " + track.Track.FileSize/1000000 + " MB"
                    };
                    appearanceService.UpdateMusicColor(int.Parse(track.Bitrate.Replace(" kbps","")));
                }
                catch (Exception ex)
                {
                    LogClient.Error("Could not show playback information for Track {0}. Exception: {1}", track.Path, ex.Message);
                    this.ClearPlaybackInfo();
                }
            });
        }
    }
}
