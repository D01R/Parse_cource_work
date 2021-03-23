using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using word = Microsoft.Office.Interop.Word;
using System.Data.Entity;
using System.Reflection;

namespace AIS_Kursach.COMwork
{
    public class COM_Word
    {
        private word.Application wordapp = new word.Application();
        private word.Documents worddocuments;
        private word.Document worddocument;
        private word.Paragraph wordparagraph;
        private const string outputPath = @"C:\Хранилище\5 семак\Архитектура ИС\Курсовой проект\COM объекты\template.doc";

        public COM_Word(string temlate = @"C:\Хранилище\5 семак\Архитектура ИС\Курсовой проект\COM объекты\шаблон\template.doc")
        {
            wordapp.Visible = true;

            Object newTemplate = false;
            Object documentType = word.WdNewDocumentType.wdNewBlankDocument;
            Object visible = true;

            wordapp.Documents.Add(temlate, newTemplate, ref documentType, ref visible);
            
            worddocuments = wordapp.Documents;
            worddocument = worddocuments.get_Item(1);
            worddocument.PageSetup.Orientation = word.WdOrientation.wdOrientLandscape;
            worddocument.Activate(); 
        }
        public void AddImage(int param)
        {
            word.Range place = worddocument.Paragraphs[param].Range;
            place.InlineShapes.AddPicture(@"C:\Хранилище\5 семак\Архитектура ИС\Курсовой проект\COM объекты\шаблон\диаграмма.bmp");
        }
        public void Replace(string wordr, string replacement)
        {
            word.Range range = worddocument.StoryRanges[word.WdStoryType.wdMainTextStory];
            range.Find.ClearFormatting();

            range.Find.Execute(FindText: wordr, ReplaceWith: replacement);

            TrySave();
        }
        public void ReplaceBookmark(string bookmark, string replacement)
        {
            worddocument.Bookmarks[bookmark].Range.Text = replacement;
            TrySave();
        }
        public void AddTable()
        {
            int columns = 8;
            int rows = 11;
            List<AnimeWord> animeList = new List<AnimeWord>();
            using (AnimeContext db = new AnimeContext())
            {
                List<Anime> animeBd = db.Animes.Include(obj=>obj.Type).Include(o=>o.Genres).ToList();
                animeList = AnimeBdToWord(animeBd);
            }
            wordparagraph = worddocument.Paragraphs[5];
            word.Range wordrange = wordparagraph.Range;
            Object defaultTableBehavior = word.WdDefaultTableBehavior.wdWord9TableBehavior;
            Object autoFitBehavior = word.WdAutoFitBehavior.wdAutoFitWindow;
            word.Table wordTable = worddocument.Tables.Add(wordrange, rows, columns, ref defaultTableBehavior, ref autoFitBehavior);
            List<string> animeColums = new List<string>() { "№","Название", "Серии", "Год", "Рейтинг", "Ссылка", "Тип", "Жанр" };
            for(int i = 1; i <= columns; i++)
            {
                wordTable.Cell(1, i).Range.Text = animeColums[i - 1];
            }  
            for(int i= 2; i <= rows; i++)
            {
                PropertyInfo[] propertyInfo = typeof(AnimeWord).GetProperties();
                for(int j = 1; j <= columns; j++)
                {
                    if (j==1)
                        wordTable.Cell(i, j).Range.Text = $"{i - 1}";
                    else
                        wordTable.Cell(i, j).Range.Text = propertyInfo[j - 2].GetValue(animeList[i - 2]).ToString();
                }
            }
            TrySave();
        }
        private List<AnimeWord> AnimeBdToWord(List<Anime> animeBd)
        {
            List<AnimeWord> animeList = new List<AnimeWord>();
            foreach (Anime anime in animeBd)
            {
                AnimeWord ani = new AnimeWord();
                ani.Name = anime.Name;
                ani.YearRelease = anime.YearRelease;
                ani.Episodes = anime.Episodes;
                ani.Popularity = anime.Popularity;
                ani.Href = anime.Href;
                ani.Type = anime.Type.Name;
                List<Genre> genres = anime.Genres.ToList();
                string gen = "";
                foreach (Genre genre in genres)
                {
                    gen += genre.Name;
                    if (genre != genres[genres.Count - 1])
                        gen += ", ";
                }
                ani.Genre = gen;
                animeList.Add(ani);
            }
            return animeList;
        }
        private void TrySave()
        {
            try
            {
                worddocument.SaveAs(outputPath, word.WdSaveFormat.wdFormatDocument);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
        public void Close()
        {
            Object saveChanges = word.WdSaveOptions.wdPromptToSaveChanges;
            Object originalFormat = word.WdOriginalFormat.wdWordDocument;
            Object routeDocument = System.Type.Missing;
            wordapp.Quit(ref saveChanges, ref originalFormat, ref routeDocument);
        }
        public COM_Word(string temlate, List<GenreDif> genres)
        {
            wordapp.Visible = true;

            Object newTemplate = false;
            Object documentType = word.WdNewDocumentType.wdNewBlankDocument;
            Object visible = true;

            wordapp.Documents.Add(temlate, newTemplate, ref documentType, ref visible);

            worddocuments = wordapp.Documents;
            worddocument = worddocuments.get_Item(1);
            //worddocument.PageSetup.Orientation = word.WdOrientation.wdOrientLandscape;
            worddocument.Activate();
        }
        public void AddTable(int param, List<GenreDif> genres)
        {
            int columns = 4;
            int rows = genres.Count+1;
            //List<AnimeWord> animeList = new List<AnimeWord>();
            //using (AnimeContext db = new AnimeContext())
            //{
            //    List<Anime> animeBd = db.Animes.Include(obj => obj.Type).Include(o => o.Genres).ToList();
            //    animeList = AnimeBdToWord(animeBd);
            //}
            wordparagraph = worddocument.Paragraphs[param];
            word.Range wordrange = wordparagraph.Range;
            Object defaultTableBehavior = word.WdDefaultTableBehavior.wdWord9TableBehavior;
            Object autoFitBehavior = word.WdAutoFitBehavior.wdAutoFitWindow;
            word.Table wordTable = worddocument.Tables.Add(wordrange, rows, columns, ref defaultTableBehavior, ref autoFitBehavior);
            List<string> animeColums = new List<string>() {"№", "Сочитаемый жанр", "Кол-во аниме", "Средний рейтинг"};
            for (int i = 1; i <= columns; i++)
            {
                wordTable.Cell(1, i).Range.Text = animeColums[i - 1];
            }
            for (int i = 2; i <= rows; i++)
            {
                PropertyInfo[] propertyInfo = typeof(GenreDif).GetProperties();
                for (int j = 1; j <= columns; j++)
                {
                    if (j == 1)
                        wordTable.Cell(i, j).Range.Text = $"{i - 1}";
                    else
                        wordTable.Cell(i, j).Range.Text = propertyInfo[j - 2].GetValue(genres[i - 2]).ToString();
                }
            }
            TrySave();
        }
    }
}
