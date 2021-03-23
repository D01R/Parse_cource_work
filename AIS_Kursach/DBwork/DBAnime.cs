using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AIS_Kursach
{
    public class Anime
    {
        public Anime()
        {
            Genres = new HashSet<Genre>();
            Comments = new List<Comment>();
        }

        [Key]
        public Guid Id_anime { get; set; }
        public string Name { get; set; }
        public string Episodes { get; set; }
        public int YearRelease { get; set; }
        public string Producer { get; set; }
        public int Popularity { get; set; }
        public string Description { get; set; }
        public string Href { get; set; }
        public Guid? Id_type { get; set; }
        public Type Type { get; set; }
        public virtual ICollection<Genre> Genres { get; set; }
        public virtual ICollection<Comment> Comments { get; set; }
    }
    public class Type
    {
        public Type()
        {
            Animes = new List<Anime>(); 
        }
        [Key]
        public Guid Id_type { get; set; }
        public string Name { get; set; }
        public virtual ICollection<Anime> Animes { get; set; }
    }
    public class Genre
    {
        public Genre()
        {
            Animes = new HashSet<Anime>();
        }
        [Key]
        public Guid Id_genre { get; set; }
        public string Name { get; set; }
        public virtual ICollection<Anime> Animes { get; set; }
    }
    public class Comment
    {
        [Key]
        public Guid Id_comment { get; set; }
        public string Nickname { get; set; }
        public DateTime DTComment { get; set; }
        public string TextComment { get; set; }
        public Guid? Id_anime { get; set; }
        public Anime Anime { get; set; }
    }
    class AnimeContext : DbContext
    {
        public AnimeContext() : base("AnimeConection") { }
        public DbSet<Type> Types { get; set; }
        public DbSet<Genre> Genres { get; set; }
        public DbSet<Anime> Animes { get; set; }
        public DbSet<Comment> Comments { get; set; }
    }
}
