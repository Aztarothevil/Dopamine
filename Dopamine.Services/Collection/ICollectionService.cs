using Dopamine.Data;
using Dopamine.Services.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Dopamine.Services.Collection
{
    public interface ICollectionService
    {
        Task<RemoveTracksResult> RemoveTracksFromCollectionAsync(IList<TrackViewModel> selectedTracks);

        Task<RemoveTracksResult> RemoveTracksFromDiskAsync(IList<TrackViewModel> selectedTracks);

        Task<IList<ArtistViewModel>> GetAllArtistsAsync(ArtistType artistType);

        Task<IList<FolderViewModel>> GetAllFoldersAsync();

        Task<IList<GenreViewModel>> GetAllGenresAsync();

        Task<IList<AlbumViewModel>> GetAllAlbumsAsync();

        Task<IList<SongViewModel>> GetAllMusicAsync();

        Task<IList<AlbumViewModel>> GetArtistAlbumsAsync(IList<string> selectedArtists, ArtistType artistType);

        Task<IList<SongViewModel>> GetFolderAlbumAsync(IList<string> selectedFolders, MusicType artistType);

        Task<IList<AlbumViewModel>> GetGenreAlbumsAsync(IList<string> selectedGenres);

        Task<IList<SongViewModel>> GetGenreMusicAsync(IList<string> selectedGenres);

        Task<IList<AlbumViewModel>> OrderAlbumsAsync(IList<AlbumViewModel> albums, AlbumOrder albumOrder);

        Task<IList<SongViewModel>> OrderSongsAsync(IList<SongViewModel> songs, MusicOrder musicOrder);

        event EventHandler CollectionChanged;
       
    }
}
