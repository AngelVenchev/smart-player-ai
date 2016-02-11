using SmartPlayer.Core.DTOs;
using SmartPlayer.Core.Repositories;
using SmartPlayer.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartPlayer.Core.SongAnalyzer
{
    public class PearsonScoreCalculator
    {
        private SmartPlayerEntities _context;
        public PearsonScoreCalculator(SmartPlayerEntities context)
        {
            _context = context;
        }

        public List<UserSongVote> GetUserSongVotes(string userId)
        {
            UserSongRatingRepository repo = new UserSongRatingRepository(_context);

            var userSongVotes = repo.GetUserSongVotes(userId);

            return userSongVotes;
        }


        public List<UserScore> CalculatePearsonScoreForCurrentUser(string userId)
        {
            List<UserSongVote> currentUserPreference = GetUserSongVotes(userId);

            UserRepository userRepo = new UserRepository(_context);

            var allUsers = userRepo.GetAllExcept(userId);

            List<UserScore> scores = new List<UserScore>();
            foreach(User user in allUsers) {
                List<UserSongVote> comparedUserPreference = user.UserSongVotes.ToList();

                double score = CalculcatePearsonScore(currentUserPreference, comparedUserPreference);

                scores.Add(new UserScore { User = user, Score = score });                
            }

            scores = scores.OrderBy(x => Math.Abs(x.Score)).Take(10).ToList();
            return scores;
        }

        public List<Song> GetBestSongsForUser(string userId)
        {
            List<UserScore> userScores = CalculatePearsonScoreForCurrentUser(userId);
            List<Song> bestSongs = new List<Song>();
            foreach (var scoreEntry in userScores)
            {
                ICollection<UserSongVote> userVotes = scoreEntry.User.UserSongVotes;
                if (scoreEntry.Score > 0)
                {
                    bestSongs.AddRange(userVotes.OrderBy(x => x.Rating).Select(x => x.Song));                    
                }
                else if (scoreEntry.Score < 0)
                {
                    bestSongs.AddRange(userVotes.OrderByDescending(x => x.Rating).Select(x => x.Song));
                }
            }

            return bestSongs;
        }

        public double CalculcatePearsonScore(List<UserSongVote> preferences1, List<UserSongVote> preferences2)
        {
            preferences1 = preferences1.Where(item => {
                return preferences2
                    .Select(x => x.SongId)
                    .Contains(item.SongId);
            }).ToList();
            int n = preferences1.Count;

            if (n == 0) {
                return 0.0;
            }

            double sum1 = 0;
            double sum1Sq = 0;
            foreach (UserSongVote p in preferences1)
            {
                sum1 += p.Rating;
                sum1Sq += Math.Pow(p.Rating, 2);
            }

            double sum2 = 0;
            double sum2Sq = 0;
            foreach (UserSongVote p in preferences2)
            {
                if (preferences1.Contains(p)) {
                    sum2 += p.Rating;
                    sum2Sq += Math.Pow(p.Rating, 2);
                }
            }

            double pSum = 0;
            foreach (UserSongVote p in preferences1)
            {
                pSum += p.Rating * preferences2[preferences2.IndexOf(p)].Rating;
            }

            double numerator = pSum - ((sum1 * sum2) / n);
            double denominator = Math.Sqrt((sum1Sq - Math.Pow(sum1, 2) / n) * (sum2Sq - Math.Pow(sum2, 2) / n));
            if (denominator == 0) {
                return 0.0;
            }

            double correlationCoefficient = numerator / denominator;
            return correlationCoefficient;

        }
    }
}
