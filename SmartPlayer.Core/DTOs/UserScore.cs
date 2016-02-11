using SmartPlayer.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartPlayer.Core.DTOs
{
    public class UserScore
    {
        public User User { get; set; }
        public double Score { get; set; }
    }
}
