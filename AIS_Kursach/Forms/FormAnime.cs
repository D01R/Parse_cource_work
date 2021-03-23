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
    public partial class FormAnime : Form
    {
        public FormAnime()
        {
            InitializeComponent();
        }
        List<Anime> animeDbList = new List<Anime>();
        //инициализация формы
        private void FormAnime_Load(object sender, EventArgs e)
        {
            List<Genre> genres = new List<Genre>();
            using (AnimeContext db = new AnimeContext())
            {
                animeDbList = db.Animes.ToList();
                genres = db.Genres.ToList();
            }
            FillInDataGrid(animeDbList);

            comboBox1.Items.Add("Все жанры");
            foreach(Genre genre in genres)
            {
                comboBox1.Items.Add(genre.Name);
            }
        }
        //заполнение DataGrid
        private void FillInDataGrid(List<Anime> animeDbList)
        {
            bunifuDataGridView1.DataSource = AnimeConverter(animeDbList);
            bunifuDataGridView1.Columns[0].HeaderText = "Название";
            bunifuDataGridView1.Columns[1].HeaderText = "Жанр";
            bunifuDataGridView1.Columns[2].HeaderText = "Рейтинг";
            bunifuDataGridView1.Columns[3].HeaderText = "Год";
            bunifuDataGridView1.Columns[4].HeaderText = "Тип";
            bunifuDataGridView1.CellClick += new DataGridViewCellEventHandler(Link);
        }
        //действие при выборе аниме
        private void Link(object sender, EventArgs e)
        {
            dynamic form = sender;
            var tb = (System.Drawing.Point)form.CurrentCellAddress;
            var indexY = tb.Y;
            if (indexY >= 0)
            {
                AnimeWork.AnimeData.ChosenAnime = bunifuDataGridView1[0, indexY].Value.ToString();
                RedirectEdit();
            }
        }
        //переход на форму редактирования
        private void RedirectEdit()
        {
            AnimeEdit form = new AnimeEdit();
            form.Show();
            form.Location = this.Location;
            this.Close();
        }
        private List<AnimeDG_model> AnimeConverter(List<Anime> animes)
        {
            List<AnimeDG_model> anis = new List<AnimeDG_model>();
            foreach(Anime anime in animes)
            {
                AnimeDG_model ani = new AnimeDG_model();
                ani.Name = anime.Name;
                ani.Genres = GenresConverter(anime);
                ani.Popularity = anime.Popularity;
                ani.Year = anime.YearRelease;
                string typ = "";
                using (AnimeContext db = new AnimeContext())
                {
                    typ = db.Types.FirstOrDefault(o=>o.Id_type == anime.Id_type).Name;
                }
                ani.Type = typ;
                anis.Add(ani);
            }
            return anis;
        }
        private string GenresConverter(Anime anime)
        {
            string genres = "";
            List<Genre> gens = new List<Genre>();
            using (AnimeContext db = new AnimeContext()) 
            {
                gens = db.Animes.FirstOrDefault(o=>o.Id_anime == anime.Id_anime).Genres.ToList();
            }
            foreach (Genre gen in gens)
            {
                genres += gen.Name+", ";
            }
            return genres.Remove(genres.Length - 2, 2);
        }
        //выбор по жанру
        private void bunifuButton1_Click(object sender, EventArgs e)
        {
            string genreChosen = comboBox1.SelectedItem.ToString();
            if (genreChosen== "Все жанры")
            {
                using (AnimeContext db = new AnimeContext())
                {
                    animeDbList = db.Animes.ToList();
                }
                FillInDataGrid(animeDbList);
            }
            else
            {
                using (AnimeContext db = new AnimeContext())
                {
                    animeDbList = db.Animes.Where(o=>o.Genres.FirstOrDefault(j=>j.Name == genreChosen) != null).ToList();
                }
                FillInDataGrid(animeDbList);
            }
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

        private void bunifuImageButton3_Click(object sender, EventArgs e)
        {
            FormCOM form = new FormCOM();
            form.Show();
            this.Close();
        }

        private void bunifuImageButton5_Click(object sender, EventArgs e)
        {
            this.Close();
        }

    }
}
