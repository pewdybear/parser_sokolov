using Parser.ComponentModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Parser.Model
{
    class Composition : DBEntity
    {
        private long? author_id;
        private string title;
        private string annotation;
        private string status;
        private string url;
        public bool IsDB = false;
        private string[] fields = new string[] { "ID", "Author_ID", "Title", "Annotation", "Status", "URL" };
        private static string tableName = "composition";
        public static CollectionModel Compositions = new CollectionModel();

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

        public long? Author_id
        {
            get { return author_id; }
            set { author_id = value; }
        }

        public string Title
        {
            get { return title; }
            set { title = value; }
        }

        public string Annotation
        {
            get { return annotation; }
            set { annotation = value; }
        }

        public string Status
        {
            get { return status; }
            set { status = value; }
        }

        public string Url
        {
            get { return url; }
            set { url = value; }
        }

        public Composition()
        {

        }

        public Composition(long? author_id, string title, string annotation, string status, string url) : base()
        {
            this.author_id = author_id;
            this.title = title;
            this.annotation = annotation;
            this.status = status;
            this.url = url;
        }

        public Composition(long? id, long? author_id, string title, string annotation, string status, string url) : base(id)
        {
            this.author_id = author_id;
            this.title = title;
            this.annotation = annotation;
            this.status = status;
            this.url = url;
        }

        public override long? Add(DBEntity dBEntity)
        {
            Compositions.Add(dBEntity);
            AddItem?.Invoke(dBEntity);
            return Compositions.Last().Id;
        }

        public override CollectionModel Fill(Dictionary<string, List<string>> allRow)
        {
            foreach (KeyValuePair<string, List<string>> s in allRow)
            {
                Composition composition = new Composition(long.Parse(s.Key), int.Parse(s.Value[0]), s.Value[1], s.Value[2], s.Value[3], s.Value[4]);
                composition.IsDB = true;
                Compositions.Add(composition);
            }
            Console.WriteLine($"Загрузка таблицы {tableName} завершена.");
            return Compositions;
        }

        public override CollectionModel ReturnCollection()
        {
            return Compositions;
        }

        public override void PrintTable()
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Таблица Произведения:");
            Console.ForegroundColor = ConsoleColor.White;
            foreach (Composition comp in Compositions)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Write("Author_ID: ");
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine($"{comp.author_id}");
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Write("Название: ");
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine($"{comp.title}");
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Write("Аннотация: ");
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine($"{comp.annotation}");
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Write("URL: ");
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine($"{comp.status}");
                Console.WriteLine("_________________");
            }
        }

        public override void Save()
        {
            for (int i = 0; i < Compositions.Count(); i++)
            {
                if (Compositions[i].Id == null)
                {
                    if (i == 0)
                        Compositions[i].Id = 1;
                    else Compositions[i].Id = Compositions[i - 1].Id + 1;
                }
            }
        }

        public override long? Contain(string url)
        {
            foreach (Composition comp in Compositions)
            {
                if (comp.Url == url)
                    return comp.Id;
            }
            return 0;
        }

        public override DBEntity SaveLast()
        {
            Composition c = (Composition)Compositions[Compositions.Count() - 1];
            return c;
        }

        public override DBEntity ReturnStr(long? index)
        {
            foreach (Composition comp in Compositions)
            {
                if (comp.Id == index)
                    return comp;
            }
            return null;
        }
    }
}
