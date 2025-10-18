using CommonServiceLocator;
using Dopamine.Services.Metadata;
using Dopamine.Services.Playback;

namespace Dopamine.ViewModels.Common
{
    public class CoverPlaybackInfoControlViewModel : PlaybackInfoControlViewModel
    {
        public CoverPlaybackInfoControlViewModel() : base(
            ServiceLocator.Current.GetInstance<IPlaybackService>(), 
            ServiceLocator.Current.GetInstance<IMetadataService>())
        {
        }
    }
}