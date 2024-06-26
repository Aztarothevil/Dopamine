﻿namespace Dopamine.Data
{
    public enum AddFolderResult
    {
        Error = 0,
        Success = 1,
        Duplicate = 2,
        Inaccessible = 3
    }

    public enum RemoveFolderResult
    {
        Error = 0,
        Success = 1
    }

    public enum ArtistType
    {
        All = 0,
        Track = 1,
        Album = 2
    }

    public enum TrackOrder
    {
        Alphabetical = 1,
        ByAlbum = 2,
        ByFileName = 3,
        ByRating = 4,
        ReverseAlphabetical = 5,
        None = 6
    }

    public enum AlbumOrder
    {
        Alphabetical = 1,
        ByDateAdded = 2,
        ByDateCreated = 3,
        ByAlbumArtist = 4,
        ByYearDescending = 5,
        ByYearAscending = 6
    }

    public enum RemoveTracksResult
    {
        Error = 0,
        Success = 1
    }

    public enum MusicType
    {
        All = 0,
        Track = 1,
        Folder = 2
    }

    public enum MusicOrder
    {
        Alphabetical = 1,
        ByDateCreated = 2,
        ReverseByDateCreated = 3,
        ByDateModified = 4,
        ReverseByDateModified = 5,
        ByAlbumArtist = 6,
        ReverseAlphabetical = 7,
        None = 8
    }
}
