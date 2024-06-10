using Digimezzo.Foundation.Core.Logging;
using Dopamine.Core.Prism;
using Dopamine.Services.Utils;
using Dopamine.Views.Common.Base;
using Prism.Commands;
using System;
using System.Windows;
using System.Windows.Input;

namespace Dopamine.Views.FullPlayer.Collection
{
    /// <summary>
    /// Interaction logic for CollectionMusic.xaml
    /// </summary>
    public partial class CollectionMusic : TracksViewBase
    {
        public CollectionMusic() : base()
        {
            InitializeComponent();

            // Commands
            this.ViewInExplorerCommand = new DelegateCommand(() => this.ViewInExplorer(this.ListBoxTracks));
            this.JumpToPlayingTrackCommand = new DelegateCommand(async () => await this.ScrollToPlayingSongAsync(this.ListBoxSongs));

            // PubSub Events
            this.eventAggregator.GetEvent<ScrollToPlayingTrack>().Subscribe(async (_) => await this.ScrollToPlayingTrackAsync(this.ListBoxTracks));
            this.eventAggregator.GetEvent<ScrollToPlayingTrack>().Subscribe(async (_) => await this.ScrollToPlayingSongAsync(this.ListBoxSongs));

            this.eventAggregator.GetEvent<PerformSemanticJump>().Subscribe(async (data) =>
            {
                try
                {
                    if (data.Item1.Equals("Music"))
                    {
                        await SemanticZoomUtils.SemanticScrollAsync(this.ListBoxMusic, data.Item2);
                    }
                }
                catch (Exception ex)
                {
                    LogClient.Error("Could not perform semantic zoom on Music. Exception: {0}", ex.Message);
                }
            });
        }

        private async void ListBoxFolder_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            await this.ActionHandler(sender, e.OriginalSource as DependencyObject, true);
        }

        private async void ListBoxArtists_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                await this.ActionHandler(sender, e.OriginalSource as DependencyObject, true);
            }
        }

        private async void ListBoxSongs_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            await this.ActionHandler(sender, e.OriginalSource as DependencyObject, true);
        }

        private async void ListBoxSongs_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                await this.ActionHandler(sender, e.OriginalSource as DependencyObject, true);
            }
        }

        private async void ListBoxTracks_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            await this.ActionHandler(sender, e.OriginalSource as DependencyObject, true);
        }

        private async void ListBoxTracks_KeyUp(object sender, KeyEventArgs e)
        {
            await this.KeyUpHandlerAsync(sender, e);
        }

        private async void ListBoxTracks_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                await this.ActionHandler(sender, e.OriginalSource as DependencyObject, true);
            }
        }

        private void ArtistsButton_Click(object sender, RoutedEventArgs e)
        {
            if (this.ListBoxMusic.SelectedItem == null)
            {
                this.eventAggregator.GetEvent<ToggleArtistOrderCommand>().Publish(null);
            }
            else
            {
                this.ListBoxMusic.SelectedItem = null;
            }
        }

        private void AlbumsButton_Click(object sender, RoutedEventArgs e)
        {
            this.ListBoxSongs.SelectedItem = null;
        }
    }
}
