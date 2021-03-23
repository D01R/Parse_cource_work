using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AIS_Kursach
{
    //Модель для подсчета аниме определенного жанра Excel
    public class CountGenres
    {
        public IEnumerable<Genre> genre { get; set; }
        public IEnumerable<Anime> anime { get; set; }
        public CountGenres(IEnumerable<Anime> anime)
        {
            this.anime = anime;
        }
    }
    //Модель для вывода таблицы Word 1-го отчета
    public class AnimeWord
    {
        public string Name { get; set; }
        public string Episodes { get; set; }
        public int YearRelease { get; set; }
        public int Popularity { get; set; }
        public string Href { get; set; }
        public string Type { get; set; }
        public string Genre { get; set; }
    }
    //Модель для подсчета одновременно использованных жанров
    public class GenreDif
    {
        public string Name { get; set; }
        public int Number { get; set; }
        public double Rating { get; set; }
    }
}
