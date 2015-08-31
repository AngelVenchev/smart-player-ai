using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartPlayer.Core.DTOs
{
    public class NextSongDto
    {
        public List<int> PlayedSongIds { get; set; }
        public int CurrentSongId { get; set; }
    }
}
