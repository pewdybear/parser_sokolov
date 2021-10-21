using Parser.ComponentModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Parser.Model
{
    class CompositionGenre : DBEntity
    {
        private long? compositionID;
        private long? genreID;
        public bool IsDB = false;
        private string[] fields = new string[] { "ID", "CompositionID", "GenreID" };
        private static string tableName = "compositiongenre";
        public static CollectionModel CompositionGenres = new CollectionModel();

        public override string TableName { get => tableName; protected set => tableName = value; }
        public override string[] Fields { get => fields; protected set => fields = value; }

        public delegate void AddHandler(DBEntity dBEntity);
        public event AddHandler AddItem;

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged == null) return;
            PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        public long? CompositionID
        {
            get { return compositionID; }
            set { compositionID = value; }
        }

        public long? GenreID
        {
            get { return genreID; }
            set { genreID = value; }
        }

        public CompositionGenre()
        {

        }

        public CompositionGenre(long? compositionID, long? genreID) : base()
        {
            this.compositionID = compositionID;
            this.genreID = genreID;
        }

        public CompositionGenre(long? id, int compositionID, int genreID) : base(id)
        {
            this.compositionID = compositionID;
            this.genreID = genreID;
        }

        public override long? Add(DBEntity dBEntity)
        {
            CompositionGenres.Add(dBEntity);
            AddItem?.Invoke(dBEntity);
            return CompositionGenres.Last().Id;
        }

        public override CollectionModel Fill(Dictionary<string, List<string>> allRow)
        {
            foreach (KeyValuePair<string, List<string>> s in allRow)
            {
                CompositionGenre compositionGenre = new CompositionGenre(long.Parse(s.Key), int.Parse(s.Value[0]), int.Parse(s.Value[1]));
                compositionGenre.IsDB = true;
                CompositionGenres.Add(compositionGenre);
            }
            Console.WriteLine($"Загрузка таблицы {tableName} завершена.");
            return CompositionGenres;
        }

        public override CollectionModel ReturnCollection()
        {
            return CompositionGenres;
        }

        public override void PrintTable()
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Словарь Произведение-Жанры:");
            Console.ForegroundColor = ConsoleColor.White;
            foreach (CompositionGenre comp in CompositionGenres)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Write("ID произведения: ");
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine($"{comp.compositionID}");
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Write("ID жанра: ");
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine($"{comp.genreID}");
                Console.WriteLine("_________________");
            }
        }

        public override void Save()
        {
            for (int i = 0; i < CompositionGenres.Count(); i++)
            {
                if (CompositionGenres[i].Id == null)
                {
                    if (i == 0)
                        CompositionGenres[i].Id = 1;
                    else CompositionGenres[i].Id = CompositionGenres[i - 1].Id + 1;
                }
            }
        }

        public override long? Contain(string url)
        {
            throw new NotImplementedException();
        }

        public override DBEntity SaveLast()
        {
            CompositionGenre cg = (CompositionGenre)CompositionGenres[CompositionGenres.Count() - 1];
            return cg;
        }

        public override DBEntity ReturnStr(long? index)
        {
            foreach (CompositionGenre cg in CompositionGenres)
                if (cg.Id == index)
                    return cg;
            return null;
        }
    }
}
