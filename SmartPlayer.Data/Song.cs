//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace SmartPlayer.Data
{
    using System;
    using System.Collections.Generic;
    
    public partial class Song
    {
        public Song()
        {
            this.UserSongVotes = new HashSet<UserSongVote>();
            this.CorrelationsAsPrimary = new HashSet<SongSongCorrelation>();
            this.CorrelationsAsSecondary = new HashSet<SongSongCorrelation>();
        }
    
        public int Id { get; set; }
        public string Name { get; set; }
        public string Guid { get; set; }
        public double Grade { get; set; }
    
        public virtual ICollection<UserSongVote> UserSongVotes { get; set; }
        public virtual ICollection<SongSongCorrelation> CorrelationsAsPrimary { get; set; }
        public virtual ICollection<SongSongCorrelation> CorrelationsAsSecondary { get; set; }
    }
}
