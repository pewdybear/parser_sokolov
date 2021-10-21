using Parser.ComponentModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parser.Model
{
    class CompositionGenreView
    {
        private string title;
        private string annotation;
        private string genre;
        public int CountGenre;
        public List<CompositionGenreView> dataSet;
        public static CollectionModel compositions;
        public static CollectionModel compositionGenres;
        public static CollectionModel genres;
        Composition c;
        CompositionGenre cg;
        Genre g;
        public string Tiitle
        {
            get { return title; }
            set { title = value; }
        }

        public string Annotation
        {
            get { return annotation; }
            set { annotation = value; }
        }

        public string Genre
        {
            get { return genre; }
            set { genre = value; }
        }

        public CompositionGenreView()
        { 
        
        }

        public CompositionGenreView(Composition c, CompositionGenre cg, Genre g)
        {
            dataSet = new List<CompositionGenreView>();
            this.c = c;
            this.cg = cg;
            this.g = g;
            //compositions = c.ReturnCollection();
            compositionGenres = cg.ReturnCollection();
            //genres = g.ReturnCollection();
            FillDataSet();
        }

        public CompositionGenreView(string title, string annotation, string genre, int count)
        {
            this.title = title;
            this.annotation = annotation;
            this.genre = genre;
            CountGenre = count;
        }

        public void FillDataSet()
        {
            foreach (CompositionGenre cg2 in compositionGenres)
            {
                string CompositionTitle = ((Composition)c.ReturnStr(cg2.CompositionID)).Title.Replace($"\r","").Replace($"\r\n", "").Replace(@"  ", "");
                string CompositionGenre = ((Genre)g.ReturnStr(cg2.GenreID)).Title.Replace($"\r", "").Replace($"\r\n", "").Replace(@"  ", "");
                string CompositionAnnotation = ((Composition)c.ReturnStr(cg2.CompositionID)).Annotation.Replace($"\r", "").Replace($"\r\n", "").Replace(@"  ", "");
                if (CompositionAnnotation.Contains("Аннотация отсутствует.") == false)
                {
                    dataSet.Add(new CompositionGenreView(CompositionTitle, CompositionAnnotation, CompositionGenre, 1));
                    Console.WriteLine($"В dataSet добавлен ID = {cg2.Id}");
                }
            }

            for (int i = 1; i < dataSet.Count(); i++)
            {
                if (dataSet[i].Tiitle == dataSet[i - 1].Tiitle)
                {
                    dataSet[i - 1].Genre += "\t" + dataSet[i].Genre;
                    dataSet[i - 1].CountGenre += 1;
                    dataSet.RemoveAt(i);
                    i--;
                }
            }

            for (int i = 0; i < dataSet.Count(); i++)
            {
                if (dataSet[i].CountGenre < 3)
                {
                    if (dataSet[i].CountGenre == 2)
                    {
                        dataSet[i].Genre += $"\tнет";
                        dataSet[i].CountGenre = 3;
                    }
                    if (dataSet[i].CountGenre == 1)
                    {
                        dataSet[i].Genre += $"\tнет\tнет";
                        dataSet[i].CountGenre = 3;
                    }
                }
                Console.WriteLine(dataSet[i].Genre);
            }
            WriteToFile();           
        }

        public void WriteToFile()
        {
            Random random = new Random();
            for (int i = dataSet.Count - 1; i >= 1; i--)
            {
                int j = random.Next(i + 1);
                // обменять значения data[j] и data[i]
                var temp = dataSet[j];
                dataSet[j] = dataSet[i];
                dataSet[i] = temp;
            }

            StreamWriter f = new StreamWriter("DataSet.txt");
            for (int i = 0; i < 3*dataSet.Count()/4; i++)
            {
                f.WriteLine(dataSet[i].ToString());
            }
            f.Close();

            StreamWriter f2 = new StreamWriter("DataSetTest.txt");

            for (int i = 3 * dataSet.Count() / 4; i < dataSet.Count(); i++)
            {
                f2.WriteLine(dataSet[i].ToString());
            }
            f2.Close();
            Console.WriteLine("Запись завершена");
        }

        public override string ToString()
        {
            
            return $"{title}\t{Genre}\t{Annotation}";
        }
    }
}
