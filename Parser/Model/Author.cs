using Parser.ComponentModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Parser.Model
{
    public class Author : DBEntity
    {
        private string name;
        private string url;
        public bool IsDB = false;
        private string[] fields = new string[] { "ID", "Name", "URL" };
        private static string tableName = "author";
        public static CollectionModel Authors = new CollectionModel();

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

        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        public string URL
        {
            get { return url; }
            set { url = value; }
        }

        public Author()
        {
        }

        public Author(string name, string url) : base()
        {
            this.name = name;
            this.url = url;
        }

        public Author(long? id, string name, string url) : base(id)
        {
            this.name = name;
            this.url = url;
        }

        public override long? Add(DBEntity dBEntity)
        {
            Authors.Add(dBEntity);
            AddItem?.Invoke(dBEntity);
            //Save();
            return Authors.Last().Id;
        }

        public override CollectionModel Fill(Dictionary<string, List<string>> allRow)
        {
            foreach (KeyValuePair<string, List<string>> s in allRow)
            {
                Author author = new Author(long.Parse(s.Key), s.Value[0], s.Value[1]);
                author.IsDB = true;
                Authors.Add(author);
            }
            Console.WriteLine($"Загрузка таблицы {tableName} завершена.");
            return Authors;
        }

        public override CollectionModel ReturnCollection()
        {
            return Authors;
        }

        public override void PrintTable()
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Таблица Авторы:");
            Console.ForegroundColor = ConsoleColor.White;
            foreach (Author a in Authors)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Write("ID: ");
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine($"{a.Id}");
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Write("Name: ");
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine($"{a.name}");
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Write("URL: ");
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine($"{a.url}");
                Console.WriteLine("_________________");
            }
        }

        public override void Save()
        {
            for (int i = 0; i < Authors.Count(); i++)
            {
                if (Authors[i].Id == null)
                {
                    if (i == 0)
                        Authors[i].Id = 1;
                    else Authors[i].Id = Authors[i - 1].Id + 1;
                }
            }
        }

        public override long? Contain(string url)
        {
            foreach (Author a in Authors)
                if (url == a.URL)
                    return a.Id;

            return 0;
        }

        public override DBEntity SaveLast()
        {
            Author a = (Author)Authors[Authors.Count() - 1];
            return a;
        }

        public override DBEntity ReturnStr(long? index)
        {
            foreach (Author a in Authors)
                if (index == a.Id)
                    return a;

            return null;
        }
    }
}
