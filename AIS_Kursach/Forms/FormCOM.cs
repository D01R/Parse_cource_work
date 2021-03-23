using AIS_Kursach.COMwork;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AIS_Kursach
{
    public partial class FormCOM : Form
    {
        public FormCOM()
        {
            InitializeComponent();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            COM_Excel comExcel = new COM_Excel();
            comExcel.GenreDiagramBuild();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            COM_Excel comExcel = new COM_Excel();
            comExcel.GenreDiagramBuild();
            COM_Word comWord = new COM_Word();
            using (AnimeContext db = new AnimeContext())
            {
                comWord.Replace("{NumAnimes}", db.Animes.Count().ToString());
                comWord.Replace("{NumGenres}", db.Genres.Count().ToString());
                comWord.Replace("{NumTypes}", db.Types.Count().ToString());
            }
            comWord.ReplaceBookmark("mark", $"Изменено {DateTime.Now}");
            comWord.AddImage(6);
            comWord.AddTable();
            
            comWord.Close();
        }

        private void bunifuImageButton2_Click(object sender, EventArgs e)
        {
            FormHome form = new FormHome();
            form.Show();
            this.Close();
        }

        private void bunifuImageButton1_Click(object sender, EventArgs e)
        {
            FormParsing form = new FormParsing();
            form.Show();
            this.Close();
        }

        private void bunifuImageButton4_Click(object sender, EventArgs e)
        {
            FormAnime form = new FormAnime();
            form.Show();
            this.Close();
        }

        private void bunifuImageButton5_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void FormCOM_Load(object sender, EventArgs e)
        {
            using (AnimeContext db = new AnimeContext())
            {
                List<Genre> genres = db.Genres.ToList();
                foreach(Genre genre in genres)
                {
                    comboBox1.Items.Add(genre.Name);
                }
            }
        }

        private void bunifuButton1_Click(object sender, EventArgs e)
        {
            string chosenGenre = comboBox1.Text;
            int numAnimes = 0;
            List<GenreDif> genreDifs = new List<GenreDif>();
            using (AnimeContext db = new AnimeContext())
            {
                numAnimes = db.Animes.Where(o => o.Genres.FirstOrDefault(g => g.Name == chosenGenre) != null).Count();
                List<Genre> genres = db.Genres.Where(o => o.Name != chosenGenre).ToList();
                foreach (Genre genre in genres)
                {
                    GenreDif genreDif = new GenreDif();
                    genreDif.Name = genre.Name;
                    List<Anime> animes = db.Animes.Where(a => a.Genres.FirstOrDefault(g => g.Name == chosenGenre) != null && a.Genres.FirstOrDefault(o => o.Name == genre.Name) != null).ToList();
                    genreDif.Number = animes.Count();
                    int pop = 0;
                    foreach (Anime anime in animes)
                    {
                        pop += anime.Popularity;
                    }
                    if (genreDif.Number == 0)
                        genreDif.Rating = 0;
                    else
                        genreDif.Rating = Math.Round((double)pop / genreDif.Number, 2);
                    genreDifs.Add(genreDif);
                }
            }
            List<string> genreList = new List<string>();
            List<int> genreCount = new List<int>();
            foreach (GenreDif gen in genreDifs)
            {
                genreList.Add(gen.Name);
                genreCount.Add(gen.Number);
            }
            COM_Excel comExcel = new COM_Excel();
            comExcel.ExcelBuild(genreList, genreCount);

            string str = @"C:\Хранилище\5 семак\Архитектура ИС\Курсовой проект\COM объекты\шаблон\template1.doc";
            COM_Word comWord = new COM_Word( str, genreDifs);
            comWord.Replace("{Genre}", chosenGenre);
            comWord.Replace("{NumAnimes}", numAnimes.ToString());
            comWord.Replace("{GenresFit}", genreDifs.OrderByDescending(o => o.Number).First().Name);
            comWord.ReplaceBookmark("mark", $"Изменено {DateTime.Now}");
            comWord.AddImage(8);
            comWord.AddTable(6, genreDifs);

            comWord.Close();
        }
    }
}