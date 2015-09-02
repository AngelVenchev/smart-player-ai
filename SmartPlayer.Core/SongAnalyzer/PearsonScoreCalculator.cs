using SmartPlayer.Core.Repositories;
using SmartPlayer.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartPlayer.Core.SongAnalyzer
{
    class PearsonScoreCalculator
    {

        public List<UserSongVote> getUserSongVotes(string userId)
        {
            SmartPlayerEntities context = new SmartPlayerEntities();

            UserSongRatingRepository repo = new UserSongRatingRepository(context);

            var userSongVotes = repo.GetAll()
                .Where(x => x.UserId == userId)
                .ToList();
            return userSongVotes;
        }
        public List<UserSongVote> getSongVotes(int songId)
        {
            SmartPlayerEntities context = new SmartPlayerEntities();

            UserSongRatingRepository repo = new UserSongRatingRepository(context);
            
            var songVotes = repo.GetAll()
                .Where(x => x.SongId == songId)
                .ToList();
            return songVotes;
        }

        public List<Tuple<string, double>> calculatePearsonScoreForCurrentUser(string userId)
        {
            List<UserSongVote> preference1 = getUserSongVotes(userId);

            SmartPlayerEntities context = new SmartPlayerEntities();
            UserRepository userRepo = new UserRepository(context);
            var allUsers = userRepo.GetAllExcept(userId);

            List<Tuple<string, double>> scores = new List<Tuple<string, double>>();
            foreach(User user in allUsers) {
                List<UserSongVote> preference2 = user.UserSongVotes.ToList();

                double score = calculcatePearsonScore(preference1, preference2);

                scores.Add(Tuple.Create(user.Id, score));                
            }

            scores.OrderBy(x => x.Item1).ToList();
            scores.Take(10).ToList();
            return scores;
        }

        //public List<Tuple<int, double>> calculatePearsonScoreForTopUserSongs(int currentSongId, List<Tuple<string, double>> topTenUsers)
        //{
        //    List<Tuple<int, double>> scores = new List<Tuple<int, double>>();
        //    return scores;
        //}

        public double calculcatePearsonScore(List<UserSongVote> preferences1, List<UserSongVote> preferences2)
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
