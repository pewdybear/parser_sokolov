using Parser.ComponentModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Parser.Model
{
    public class Genre : DBEntity
    {
        private string title;
        private string url;
        public bool IsDB = false;
        private string[] fields = new string[] { "ID", "Title", "URL" };
        private static string tableName = "genre";
        public static CollectionModel Genres = new CollectionModel();

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

        public string Title
        {
            get { return title; }
            set { title = value; }
        }

        public string URL
        {
            get { return url; }
            set { url = value; }
        }

        public Genre()
        {
        }

        public Genre(string title, string url) : base()
        {
            this.title = title;
            this.url = url;
        }

        public Genre(long? id, string title, string url) : base(id)
        {
            this.title = title;
            this.url = url;
        }

        public override long? Add(DBEntity dBEntity)
        {
            Genres.Add(dBEntity);
            AddItem?.Invoke(dBEntity);
            return Genres.Last().Id;
        }

        public override CollectionModel Fill(Dictionary<string, List<string>> allRow)
        {
            foreach (KeyValuePair<string, List<string>> s in allRow)
            {
                Genre genres = new Genre(long.Parse(s.Key), s.Value[0], s.Value[1]);
                genres.IsDB = true;
                Genres.Add(genres);
            }
            Console.WriteLine($"Загрузка таблицы {tableName} завершена.");
            return Genres;
        }

        public override CollectionModel ReturnCollection()
        {
            return Genres;
        }

        public override void PrintTable()
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Таблица Жанры:");
            Console.ForegroundColor = ConsoleColor.White;
            foreach (Genre g in Genres)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Write("Название жанра: ");
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine($"{g.title}");
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Write("URL: ");
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine($"{g.url}");
                Console.WriteLine("_________________");
            }
        }

        public override void Save()
        {
            for (int i = 0; i < Genres.Count(); i++)
            {
                if (Genres[i].Id == null)
                {
                    if (i == 0)
                        Genres[i].Id = 1;
                    else Genres[i].Id = Genres[i - 1].Id + 1;
                }
            }
        }

        public override long? Contain(string url)
        {
            foreach (Genre g in Genres)
            {
                if (url == g.URL)
                    return g.Id;
            }
            return 0;
        }

        public override DBEntity SaveLast()
        {
            Genre g = (Genre)Genres[Genres.Count() - 1];
            return g;
        }

        public override DBEntity ReturnStr(long? index)
        {
            foreach (Genre g in Genres)
            {
                if (index == g.Id)
                    return g;
            }
            return null;
        }
    }
}
