using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.Entity;

namespace AIS_Kursach
{
    public partial class AnimeEdit : Form
    {
        public AnimeEdit()
        {
            InitializeComponent();
            FillIn();
        }
        AnimeEdit_model anime = new AnimeEdit_model();
        //заполнение полей
        private void FillIn()
        {
            string chosen = AnimeWork.AnimeData.ChosenAnime;
            if (chosen != null)
            {
                anime = GetAnime(chosen);
                bunifuTextBoxName.Text = anime.Name;
                bunifuTextBoxDescription.Text = anime.Description;
                bunifuTextBoxEpisodes.Text = anime.Episodes;
                bunifuTextBoxGenre.Text = anime.Genres;
                bunifuTextBoxHref.Text = anime.Href;
                bunifuTextBoxProducer.Text = anime.Producer;
                bunifuTextBoxType.Text = anime.Type;
                bunifuTextBoxYear.Text = anime.YearRelease.ToString();
                bunifuTextBoxPopularity.Text = anime.Popularity.ToString();
            }
            else
            {
                Return();
            }
        }
        //получение данных выбранного аниме
        private AnimeEdit_model GetAnime(string chosen)
        {
            using (AnimeContext db = new AnimeContext())
            {
                AnimeEdit_model ani = new AnimeEdit_model();
                Anime anime =  db.Animes.FirstOrDefault(o => o.Name == chosen);
                ani.Id_anime = anime.Id_anime;
                ani.Name = anime.Name;
                ani.Episodes = anime.Episodes;
                ani.YearRelease = anime.YearRelease;
                ani.Type = db.Types.FirstOrDefault(o=>o.Id_type == anime.Id_type).Name;
                ani.Producer = anime.Producer;
                ani.Popularity = anime.Popularity;
                ani.Href = anime.Href;
                ani.Description = anime.Description;
                string gen = "";
                List<Genre> genres = anime.Genres.ToList();
                foreach(Genre genre in genres)
                {
                    gen += genre.Name + ", ";
                }
                ani.Genres = gen.Remove(gen.Length - 2, 2);
                return ani;
            }
        }
        //сохранение изменений
        private void bunifuButton1_Click(object sender, EventArgs e)
        {
            try
            {
                using (AnimeContext db = new AnimeContext())
                {
                    Anime anime = db.Animes.FirstOrDefault(o => o.Id_anime == this.anime.Id_anime);
                    anime.Name = bunifuTextBoxName.Text;
                    anime.Description = bunifuTextBoxDescription.Text;
                    anime.Episodes = bunifuTextBoxEpisodes.Text;
                    anime.Href = bunifuTextBoxHref.Text;
                    anime.Producer = bunifuTextBoxProducer.Text;
                    anime.YearRelease = Convert.ToInt32(bunifuTextBoxYear.Text.Trim(' '));
                    anime.Popularity = Convert.ToInt32(bunifuTextBoxPopularity.Text.Trim(' '));
                    anime.Id_type = db.Types.FirstOrDefault(o => o.Name == bunifuTextBoxType.Text).Id_type;
                    string[] gens = GenresSep(bunifuTextBoxGenre.Text);
                    List<Genre> genreDel = anime.Genres.ToList();
                    foreach (Genre genre in genreDel)
                    {
                        anime.Genres.Remove(genre);
                        db.SaveChanges();
                    }
                    db.Entry(anime).State = EntityState.Modified;
                    db.SaveChanges();
                    foreach (string gen in gens)
                    {
                        anime.Genres.Add(db.Genres.FirstOrDefault(o => o.Name == gen));
                        db.SaveChanges();
                    }
                    MessageBox.Show("Данные сохранены");
                }
            }
            catch
            {
                MessageBox.Show("Введите значения корректно");
            }
        }
        //возврат к просмотр данных
        private void label9_Click(object sender, EventArgs e)
        {
            Return();
        }
        private void Return()
        {
            FormAnime form = new FormAnime();
            form.Show();
            form.Location = this.Location;
            this.Close();
        }
        //удаление записи
        private void bunifuButton2_Click(object sender, EventArgs e)
        {
            using (AnimeContext db = new AnimeContext())
            {
                List<Comment> comments = db.Comments.Where(o => o.Id_anime == anime.Id_anime).ToList();
                foreach(Comment comment in comments)
                {
                    db.Comments.Remove(comment);
                    db.SaveChanges();
                }
                db.Animes.Remove(db.Animes.FirstOrDefault(o => o.Id_anime == anime.Id_anime));
                db.SaveChanges();
            }
            Return();
        }
        //преобразование жанров в необходимы формат
        private string[] GenresSep (string genres)
        {
            char sep = ',';
            string[] genreList = genres.Split(sep);
            for (int i=0;i< genreList.Length;i++)
            {
                genreList[i] = genreList[i].Trim(' ');
            }
            return genreList;
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
    }
}
