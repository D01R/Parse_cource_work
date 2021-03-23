using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace AIS_Kursach
{
    public partial class FormParsing : Form
    {
        const string url = @"https://a92.agorov.org/page/{0}/";
        
        static Parser parser = new Parser();
        public FormParsing()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                int pageChoosen = Convert.ToInt32(textBox1.Text);
                string uri = String.Format(url, pageChoosen);
                Pars(uri);
            }
            catch (System.ArgumentNullException)
            {
                MessageBox.Show("Введите пожалуйста существующую страницу");
            }
            catch (System.FormatException)
            {
                MessageBox.Show("Введите страницу, пожалуйста");
            }
        }
        private void Pars(string uri)
        {
            List<string> pages = parser.GetPages(uri);
            List<string> linksAnime = new List<string>();
            for (int i = 0; i < pages.Count; i++)
            {
                linksAnime.AddRange(parser.GetAnimeUrls(pages[i]));
            }
            Console.WriteLine($"Общее количество ссылок на аниме: {linksAnime.Count()}");
            label3.Text = "0";
            label5.Text = "0";
            label8.Text = "0";
            int fail = 0;
            int suc = 0;
            int have = 0;
            Anime anime = new Anime();
            foreach (string link in linksAnime)
            {
                anime = null;
                if (!checkBox1.Checked)
                {
                    using (AnimeContext db = new AnimeContext())
                    {
                        anime = db.Animes.FirstOrDefault(o => o.Href == link);
                    }
                }
                else
                {
                    using (AnimeContext db = new AnimeContext())
                    {
                        List<Anime> animes = db.Animes.Where(o => o.Href == link).ToList();
                        foreach(Anime ani in animes)
                        {
                            List<Comment> comments = db.Comments.Where(o => o.Id_anime == ani.Id_anime).ToList();
                            foreach (Comment comment in comments)
                            {
                                db.Comments.Remove(comment);
                                db.SaveChanges();
                            }
                            db.Animes.Remove(ani);
                            db.SaveChanges();
                        }
                    }
                }
                if (anime == null)
                {
                    anime = parser.GetAnime(link);
                    if (anime.Name != null)
                    {
                        suc++;
                        label3.Text = String.Format(@"{0}", suc);
                    }
                    else
                    {
                        fail++;
                        label5.Text = String.Format(@"{0}", fail);
                    }
                }
                else
                {
                    have++;
                    label8.Text = String.Format(@"{0}", have);
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            FormCOM com = new FormCOM();
            com.Show();
            com.Location = this.Location;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            FormAnime form = new FormAnime();
            form.Show();
            form.Location = this.Location;
        }

        private void bunifuImageButton2_Click(object sender, EventArgs e)
        {
            FormHome form = new FormHome();
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
