using SmartPlayer.Core.Repositories;
using SmartPlayer.Core.SongAnalyzer;
using SmartPlayer.Data;
using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration.Assemblies;
using System.Configuration;
using System.IO;
using SmartPlayer.Core.DTOs;
using SmartPlayer.Core.Utility;

namespace SmartPlayer.Core.BusinessServices
{
    public class MusicService
    {
        public void Store(string originalFileName, string guid)
        {
            string mediaServerUrlBase = ConfigurationManager.AppSettings["MediaServerSaveBaseUrl"];
            string fullName = Path.Combine(mediaServerUrlBase, guid);
            using(SmartPlayerEntities context = new SmartPlayerEntities())
            {
                MusicRepository repo = new MusicRepository(context);

                var allSongs = repo.GetAll()
                    .Select(x => new AnalyzableSong { Id = x.Id, PhysicalFileName = x.Guid })
                    .ToList();

                allSongs.ForEach(x => x.PhysicalFileName = Path.Combine(mediaServerUrlBase, x.PhysicalFileName));

                List<double> correlationCoefficients = Analyzer.GetCorreleationCoefficientsFor(fullName, allSongs);

                Song song = new Song()
                {
                    Name = originalFileName,
                    Guid = guid,
                    Grade = 5
                };

                for(int i = 0; i < correlationCoefficients.Count; i++)
                {
                    song.CorrelationsAsPrimary.Add(new SongSongCorrelation { SecondarySongId = allSongs[i].Id, CorrelationScore = correlationCoefficients[i] });
                }

                repo.Create(song);
            }
        }

        public List<SongDto> GetAllSongs()
        {
            using(SmartPlayerEntities context = new SmartPlayerEntities())
            {
                MusicRepository repo = new MusicRepository(context);

                var allSongs = repo.GetAll()
                    .Select(x => new SongDto
                    {
                        Id = x.Id,
                        SongName = x.Name
                    })
                    .ToList();

                return allSongs;
            }
        }

        public PlayerSongDto GetNextSong(NextSongDto songRequest, string username = null)
        {
            using (SmartPlayerEntities context = new SmartPlayerEntities())
            {
                var selectedSong = GetNextSong(songRequest, username, context);
                var songUrl = ExtractSongUrl(selectedSong);

                PlayerSongDto song = ExtractSongResult(username, context, selectedSong, songUrl);

                return song;
            }
        }

        private static string ExtractSongUrl(Song selectedSong)
        {
            var songUrl = string.Format("http://{0}{1}{2}",
                IpV4Provider.GetLocalIPAddress(),
                ConfigurationManager.AppSettings["MediaServerLoadPort"],
                selectedSong.Guid);
            return songUrl;
        }

        private static Song GetNextSong(NextSongDto songRequest, string username, SmartPlayerEntities context)
        {
            MusicRepository musicRepo = new MusicRepository(context);
            var currentSong = musicRepo.GetSongById(songRequest.CurrentSongId);
            var excludedSongIdList = songRequest.PlayedSongIds;

            var recommendedSongs = GetRecommendedSongsForUser(username, context);
            recommendedSongs = recommendedSongs.Where(x => !excludedSongIdList.Contains(x.Id)).ToList();

            var similarSongs = musicRepo.GetNextSongBasedOnUserAndGrade(currentSong.Grade);
            similarSongs = similarSongs.Where(x => !excludedSongIdList.Contains(x.Id)).ToList();

            var safetySet = new Lazy<List<Song>>(() => musicRepo.GetNextSongBasedOnUserAndGrade(currentSong.Grade, excludedSongIdList));

            var selectedSong = GetNextSong(recommendedSongs, similarSongs, safetySet);
            return selectedSong;
        }

        private static Song GetNextSong(List<Song> recommendedSongs, List<Song> similarSongs, Lazy<List<Song>> safetySet)
        {
            var bestSongsForUser = new List<Song>();
            if (recommendedSongs.Any() && similarSongs.Any())
            {
                bestSongsForUser = recommendedSongs.Intersect(similarSongs).ToList();
                if (!bestSongsForUser.Any())
                {
                    bestSongsForUser = recommendedSongs.Union(similarSongs).ToList();
                }
            }
            else
            {
                bestSongsForUser = recommendedSongs.Union(similarSongs).ToList();
            }

            if(!bestSongsForUser.Any())
            {
                bestSongsForUser = safetySet.Value;
            }

            var selectedSong = bestSongsForUser.First();
            return selectedSong;
        }

        private static List<Song> GetRecommendedSongsForUser(string username, SmartPlayerEntities context)
        {
            var recommendedSongs = new List<Song>();
            if (username != null)
            {
                UserRepository userRepo = new UserRepository(context);
                var userId = userRepo.GetUserByUsername(username).Id;

                PearsonScoreCalculator calculator = new PearsonScoreCalculator(context);
                recommendedSongs = calculator.GetBestSongsForUser(userId);
            }
            return recommendedSongs;
        }

        public PlayerSongDto GetSong(int songId, string username)
        {
            using(SmartPlayerEntities context = new SmartPlayerEntities())
            {
                MusicRepository repo = new MusicRepository(context);

                var requestedSong = repo.GetSongById(songId);

                var songUrl = ExtractSongUrl(requestedSong);

                PlayerSongDto song = ExtractSongResult(username, context, requestedSong, songUrl);

                return song;
            }
        }

        private PlayerSongDto ExtractSongResult(string username, SmartPlayerEntities context, Song requestedSong, string songUrl)
        {
            PlayerSongDto song = new PlayerSongDto()
            {
                Id = requestedSong.Id,
                Name = requestedSong.Name,
                Url = songUrl,
                CurrentUserVote = GetUserRatingForSong(context, username, requestedSong)
            };
            return song;
        }

        public List<SongDto> SearchSong(string query)
        {
            using(SmartPlayerEntities context = new SmartPlayerEntities())
            {
                MusicRepository repo = new MusicRepository(context);
                var requestedSongs = repo.SearchByTerm(query);
                return requestedSongs.Select(x => new SongDto() { Id = x.Id, SongName = x.Name }).ToList();
            }
        }

        public void RateSong(SongRatingDto rating, string userName)
        {
            using (SmartPlayerEntities context = new SmartPlayerEntities())
            {
                UserRepository userRepo = new UserRepository(context);

                User currentUser = userRepo.GetAll().First(x => x.Email == userName);
                context.UserSongVotes.Add(new UserSongVote()
                    {
                        Rating = rating.Rating,
                        SongId = rating.SongId,
                        UserId = currentUser.Id
                    });
                context.SaveChanges();
            }
        }

        private int? GetUserRatingForSong(SmartPlayerEntities context, string username, Song song)
        {
            var currentUser = default(User);
            if (!string.IsNullOrWhiteSpace(username))
            {
                var userRepo = new UserRepository(context);
                currentUser = userRepo.GetUserByUsername(username);
            }

            var songVote = default(int?);
            if (currentUser != null && song.UserSongVotes.Where(x => x.UserId == currentUser.Id).Any())
            {
                songVote = song.UserSongVotes.First(x => x.UserId == currentUser.Id).Rating;
            }

            return songVote;
        }
    }
}
