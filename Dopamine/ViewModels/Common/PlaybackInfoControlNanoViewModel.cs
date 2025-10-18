using Dopamine.Services.Metadata;
using Dopamine.Services.Playback;

namespace Dopamine.ViewModels.Common
{
    public class PlaybackInfoControlNanoViewModel : PlaybackInfoControlViewModel
    {
        public PlaybackInfoControlNanoViewModel(
            IPlaybackService playbackService, 
            IMetadataService metadataService) : base(
            playbackService, 
            metadataService
            )
        {
        }
    }
}