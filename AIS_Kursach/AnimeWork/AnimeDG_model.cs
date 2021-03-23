using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AIS_Kursach
{
    class AnimeDG_model
    {
        public string Name { get; set; }
        public string Genres { get; set; }
        public int Popularity { get; set; }
        public int Year { get; set; }
        public string Type { get; set; }
    }
    public class AnimeEdit_model
    {
        public Guid Id_anime { get; set; }
        public string Name { get; set; }
        public string Episodes { get; set; }
        public int YearRelease { get; set; }
        public string Producer { get; set; }
        public int Popularity { get; set; }
        public string Description { get; set; }
        public string Href { get; set; }
        public string Type { get; set; }
        public string Genres { get; set; }
    }
}
