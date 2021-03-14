using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using CollectionManager.DataTypes;
using CollectionManager.Modules.FileIO.FileCollections;
using CollectionManager.Modules.FileIO.OsuDb;
using OMM.Desktop.Data.OmmApi;

namespace OMM.Desktop.Data.OsuCollections
{
    public class OsuCollectionsService
    {
        private static MapCacher _mapCacher = new MapCacher();
        private static OsdbCollectionHandler _osdbCollectionHandler = new OsdbCollectionHandler(null);

        public async Task<MemoryStream> GetOsdbCollection(string collectionName, List<MapMatch> mapMatches)
        {
            var collection = new Collection(_mapCacher)
            {
                Name = collectionName
            };
            foreach (var mapMatch in mapMatches)
            {
                collection.AddBeatmap(new BeatmapExtension
                {
                    ArtistRoman = mapMatch.Artist,
                    TitleRoman = mapMatch.Title,
                    ArtistUnicode = mapMatch.ArtistUnicode,
                    TitleUnicode = mapMatch.TitleUnicode,
                    MapId = mapMatch.BeatmapId,
                    MapSetId = mapMatch.BeatmapSetId ?? 0,
                    DiffName = mapMatch.DifficultyName,
                    ApproachRate = Convert.ToSingle(mapMatch.AR),
                    OverallDifficulty = Convert.ToSingle(mapMatch.OD),
                    CircleSize = Convert.ToSingle(mapMatch.CS)
                });
            }

            await using var stream = new MemoryStream();
            _osdbCollectionHandler.WriteOsdb(new Collections { collection }, stream, string.Empty);
            stream.Position = 0;
            return stream;
        }
    }
}