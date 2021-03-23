using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Excel = Microsoft.Office.Interop.Excel;

namespace AIS_Kursach
{
    class COM_Excel
    {
        Excel.Application excelApp = new Excel.Application();
        Excel.Workbook workBook;
        Excel.Worksheet workSheet;
        
        const string outputPath = @"C:\Хранилище\5 семак\Архитектура ИС\Курсовой проект\COM объекты\Диаграмма.xls";

        public void GenreDiagramBuild()
        {
            List<string> genreList = new List<string>();
            List<int> genreCount = new List<int>();
            using (AnimeContext db = new AnimeContext()) 
            {
                CountGenres item = new CountGenres(db.Animes);
                item.genre = db.Genres;
                foreach(Genre genre in item.genre)
                {
                    genreList.Add(genre.Name);
                    genreCount.Add(item.anime.Count(anime => anime.Genres.FirstOrDefault(gen => gen.Name == genre.Name) != null));
                }
            }
            ExcelBuild(genreList,genreCount);
        }

        public void ExcelBuild(List<string> genre, List<int> genreNumber)
        {
            excelApp.DisplayAlerts = false;
            excelApp.ScreenUpdating = false;
            workBook = excelApp.Workbooks.Add();
            workSheet = (Excel.Worksheet)workBook.Worksheets.get_Item(1);
            for (int i = 0; i < genre.Count; i++)
            {
                workSheet.Cells[i + 1, 1] = genre[i];
                workSheet.Cells[i + 1, 2] = genreNumber[i];
            }

            Excel.Range rng = workSheet.Range[$"A{genre.Count + 1}"];
            Excel.Borders border = rng.Borders;
            border.LineStyle = Excel.XlLineStyle.xlContinuous;

            Excel.ChartObjects chartObjs = (Excel.ChartObjects)workSheet.ChartObjects();
            Excel.ChartObject chartObj = chartObjs.Add(100, 5, 500, 300);
            Excel.Chart xlChart = chartObj.Chart;
            Excel.Range rng2 = workSheet.Range[$"A1:A{genre.Count}"];
            Excel.Range rng3 = workSheet.Range[$"A{genre.Count + 1}:A{genre.Count}"];

            xlChart.ChartType = Excel.XlChartType.xlColumnStacked;

            Excel.SeriesCollection seriesCollection = (Excel.SeriesCollection)xlChart.SeriesCollection(System.Type.Missing);

            Excel.Series series = seriesCollection.NewSeries();
            series.XValues = workSheet.get_Range("A1", $"A{genre.Count}");
            series.Values = workSheet.get_Range("B1", $"B{genre.Count}");

            xlChart.HasTitle = true;
            xlChart.ChartTitle.Text = "Диаграмма жанров аниме";

            xlChart.HasLegend = true;
            series.Name = "Кол.";

            excelApp.Visible = true;
            excelApp.UserControl = true;

            xlChart.Export(@"C:\Хранилище\5 семак\Архитектура ИС\Курсовой проект\COM объекты\шаблон\диаграмма.bmp");
            TrySave();
            excelApp.ScreenUpdating = true;
            Close();
        }
        private void TrySave()
        {
            try
            {
                workBook.SaveAs(outputPath, Excel.XlSaveAction.xlSaveChanges);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
        public void Close()
        {
            excelApp.Quit();
        }
    }
}
