using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AIS_Kursach
{
    public class Parser
    {
        public HtmlWeb web = new HtmlWeb();

        //метод получения страниц
        public List<string> GetPages(string url)
        {
            Console.WriteLine("Загрузка страниц");
            List<string> pages = new List<string>();
            HtmlDocument htmlDoc = web.Load(url);
            Console.WriteLine("Считывание ссылок страниц");
            List<HtmlNode> pagesHtml = htmlDoc.DocumentNode.SelectNodes("//td[@class='block_4']/a").ToList();
            pages.Add(url);
            foreach(HtmlNode page in pagesHtml)
            {
                pages.Add(page.GetAttributes().ToList().Find(block => block.Name == "href").Value);
            }
            Console.WriteLine("Получение страниц завершено");
            return pages;
        }

        //метод получения ссылок на аниме
        public List<string> GetAnimeUrls(string url)
        {
            List<string> links = new List<string>();
            HtmlDocument htmlDoc = web.Load(url);
            List<HtmlNode> linksHtml = htmlDoc.DocumentNode.SelectNodes("//div[@class='shortstory']/div[@class='shortstoryHead']/h2/a").ToList();
            foreach (HtmlNode link in linksHtml)
            {
                links.Add(link.GetAttributes().ToList().Find(block => block.Name == "href").Value);
            }
            Console.WriteLine($"Считывание ссылок со страницы {url} завершено");
            return links;
        }

        //метод получения аниме и его комментариев
        public Anime GetAnime(string url)
        {
            Anime anime = new Anime();
            
            try
            {
                HtmlDocument htmlDoc = web.LoadFromBrowser(url);

                //сохранение html документа
                //using (StreamWriter animes = new StreamWriter("anime.html"))
                //{
                //    animes.WriteLine(htmlDoc.Text);
                //}

                //поиск Названия и Серии
                HtmlNode name = htmlDoc.DocumentNode.SelectSingleNode("//div[@class='shortstory']/div[@class='shortstoryHead']/h1");
                string nameFix = name.InnerText.Replace("\n", "");
                bool t = true;
                while (t)
                {
                    if (nameFix[0] == ' ')
                        nameFix = nameFix.Remove(0, 1);
                    else
                        t = false;
                }
                char sep = '[';
                string[] nameSep = nameFix.Split(sep);
                anime.Name = nameSep[0];
                anime.Episodes = nameSep[1].Replace("]","");

                //поиск Года
                HtmlNode year = htmlDoc.DocumentNode.SelectSingleNode("//div[@class='shortstoryContent']/table/tbody/tr/td/p[1]");
                if (year.InnerText == "Год выхода: Неизвестен")
                    anime.YearRelease = 0;
                else
                    anime.YearRelease = Convert.ToInt32(year.InnerText.Substring(year.InnerText.Length - 4));

                //поиск жанров и их добавление в БД
                HtmlNode genre = htmlDoc.DocumentNode.SelectSingleNode("//div[@class='shortstoryContent']/table/tbody/tr/td/p[2]");
                List<string> genreList = GenreSep(genre.InnerText.Remove(0, 6));
                foreach (string gen in genreList)
                {
                    GenreAddNotExist(gen);
                }

                //поиск типов и их добавление в бд
                using (AnimeContext db = new AnimeContext())
                {
                    HtmlNode type = htmlDoc.DocumentNode.SelectSingleNode("//div[@class='shortstoryContent']/table/tbody/tr/td/p[3]");
                    Type typ = db.Types.FirstOrDefault(o => o.Name == type.InnerText.Remove(0, 5));
                    if (typ == null)
                    {
                        db.Types.Add(new Type()
                        {
                            Id_type = Guid.NewGuid(),
                            Name = type.InnerText.Remove(0, 5)
                        });
                        db.SaveChanges();
                    }
                    anime.Id_type = db.Types.FirstOrDefault(o => o.Name == type.InnerText.Remove(0, 5)).Id_type;
                }

                //поиск Режиссера
                HtmlNode producer = htmlDoc.DocumentNode.SelectSingleNode("//div[@class='shortstoryContent']/table/tbody/tr/td/p[5]/span/a");
                if (producer == null)
                    anime.Producer = "Режиссёр не указан";
                else
                    anime.Producer = producer.InnerText;
                
                //поиск рейтинга
                HtmlNode popularity = htmlDoc.DocumentNode.SelectSingleNode("//ul[@class='unit-rating']/li[@class='current-rating']");
                anime.Popularity = Convert.ToInt32(popularity.InnerText);
                
                //поиск описания
                HtmlNode description = htmlDoc.DocumentNode.SelectSingleNode("//span[@itemprop='description']");
                anime.Description = description.InnerText;

                using (AnimeContext db = new AnimeContext())
                {
                    anime.Href = url;
                    anime.Id_anime = Guid.NewGuid();
                    db.Animes.Add(anime);
                    db.SaveChanges();

                    //добавление жанров аниме в промежуточной таблице
                    foreach (string gen in genreList)
                    {
                        anime.Genres.Add(db.Genres.FirstOrDefault(o => o.Name == gen));
                        db.SaveChanges();
                    }
                }


                //Comment
                for (int i = 0; i < 5; i++)
                {
                    HtmlNode nick = htmlDoc.DocumentNode.SelectSingleNode($"//div[{i + 2}]/div/div/div[@class='commentFinalAva']/span[1]/strong/a");
                    HtmlNode datatime = htmlDoc.DocumentNode.SelectSingleNode($"//div[{i + 2}]/div/div/div[@class='commentFinalData']");
                    string dt = datatime.InnerText;
                    DateTime dateTime = new DateTime(Convert.ToInt32(dt.Substring(6, 4)), Convert.ToInt32(dt.Substring(3, 2)), Convert.ToInt32(dt.Substring(0, 2)), Convert.ToInt32(dt.Substring(11, 2)), Convert.ToInt32(dt.Substring(14, 2)), 00);
                    HtmlNode comment = htmlDoc.DocumentNode.SelectSingleNode($"//div[{i + 2}]/div/div/div[@class='commentFinalText']/div");
                    using (AnimeContext db = new AnimeContext())
                    {
                        db.Comments.Add(new Comment()
                        {
                            Id_comment = Guid.NewGuid(),
                            Nickname = nick.InnerText,
                            DTComment = dateTime,
                            TextComment = comment.InnerText.Replace("\r\n", " "),
                            Id_anime = anime.Id_anime
                        });
                    }
                }
            }
            catch
            {
                Console.WriteLine($"Аниме по ссылке {url} не получилось получить");
            }
            return anime;
        }

        //метод добавления несуществующего жанра в БД
        private void GenreAddNotExist(string genre)
        {
            using (AnimeContext db = new AnimeContext())
            {
                Genre genFind = db.Genres.FirstOrDefault(o => o.Name == genre);
                if (genFind == null)
                {
                    db.Genres.Add(new Genre()
                    {
                        Id_genre = Guid.NewGuid(),
                        Name = genre
                    });
                    db.SaveChanges();
                }
            }
        }

        //разделитель жанров
        private List<string> GenreSep(string str)
        {
            char separ = ',';
            List<string> genreList = new List<string>();
            string[] genreArr = str.Split(separ);
            foreach (string gen in genreArr)
            {
                genreList.Add(gen.Replace(" ", ""));
            }
            return genreList;
        }
    }
}
