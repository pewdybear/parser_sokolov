using Parser.Adapter;
using Parser.ComponentModel;
using System;
using System.Collections.Generic;

namespace Parser.Model
{
    class DataModel
    {
        static public ConnectionSettings connectionSettings = new ConnectionSettings();
        public MySQLDataAdapter my = new MySQLDataAdapter();

        Author author;
        Composition composition;
        CompositionGenre compositionGenre;
        Genre genre;

        static public CollectionModel Authors = new CollectionModel();
        static public CollectionModel Compositions = new CollectionModel();
        static public CollectionModel CompositionGenres = new CollectionModel();
        static public CollectionModel Genres = new CollectionModel();

        static Dictionary<string, List<string>> allAuthors { get; set; }
        static Dictionary<string, List<string>> allCompositions { get; set; }
        static Dictionary<string, List<string>> allCompositionGenres { get; set; }
        static Dictionary<string, List<string>> allGenres { get; set; }

        static List<string> CrowlerURL = new List<string>();

        public DataModel(Author author, Composition composition, CompositionGenre compositionGenre, Genre genre)
        {
            this.author = author;
            this.composition = composition;
            this.compositionGenre = compositionGenre;
            this.genre = genre;
            ConnectDB();
            author.AddItem += AddAuthors;
            composition.AddItem += AddCompositions;
            compositionGenre.AddItem += AddCompositionsGenres;
            genre.AddItem += AddGenres;
            FillAllTble();
            // PrintTable();
        }

        public void ConnectDB()
        {
            connectionSettings.Host = Settings.Host;
            connectionSettings.Port = Settings.Port;
            connectionSettings.User = Settings.User;
            connectionSettings.Password = Settings.Password;
            connectionSettings.DefaultSchema = Settings.DefaultSchema;
            connectionSettings.CharSet = Settings.CharSet;
            my.Connect(connectionSettings);
        }

        public void FillAllTble()
        {
            allAuthors = my.GetTable(author);
            author.Fill(allAuthors);

            allCompositions = my.GetTable(composition);
            composition.Fill(allCompositions);

            allCompositionGenres = my.GetTable(compositionGenre);
            compositionGenre.Fill(allCompositionGenres);

            allGenres = my.GetTable(genre);
            genre.Fill(allGenres);

            Console.WriteLine("Заполнение данных завершено...");
        }

        public void PrintTable()
        {
            author.PrintTable();
            composition.PrintTable();
            compositionGenre.PrintTable();
            genre.PrintTable();

            foreach (Author a in Authors)
            {
                Console.WriteLine($"{a.Name} {a.URL}");
            }
        }

        public void Save()
        {
            //author.Save();
            //composition.Save();
            //compositionGenre.Save();
            //genre.Save();
            foreach (Genre g in Genres)
                if (!g.IsDB)
                {
                    my.InsertRow($"Insert into genre (ID, Title, URL)" +
                        $" values ({g.Id}, '{g.Title}', '{g.URL}')");
                    g.IsDB = true;
                }
            foreach (Author a in Authors)
                if (!a.IsDB)
                {
                    my.InsertRow($"Insert into author (ID, Name, URL) " +
                        $"values ({a.Id},'{a.Name}', '{a.URL}');");
                    a.IsDB = true;
                }
            foreach (Composition comp in Compositions)
                if (!comp.IsDB)
                {
                    my.InsertRow($"Insert into composition (ID, Author_ID, Annotation, Status, Title,  URL) values" +
                        $" ({comp.Id}, {comp.Author_id},'{comp.Annotation}', '{comp.Status}', '{comp.Title}','{comp.Url}');");
                    comp.IsDB = true;
                }
            foreach (CompositionGenre comp in CompositionGenres)
                if (!comp.IsDB)
                {
                    my.InsertRow($"Insert into compositiongenre (ID,CompositionID, GenreID)" +
                        $" values ({comp.Id}, {comp.CompositionID}, {comp.GenreID})");
                    comp.IsDB = true;
                }
        }


        public void SaveLastAuthor()
        {
            Author aLast = (Author)author.SaveLast();
            my.InsertRow($"Insert into author (ID, Name, URL) " +
                        $"values ({aLast.Id},'{aLast.Name}', '{aLast.URL}');");
            aLast.IsDB = true;
        }

        public void SaveLastGenre()
        {
            Genre gLast = (Genre)genre.SaveLast();
            my.InsertRow($"Insert into genre (ID, Title, URL)" +
                        $" values ({gLast.Id}, '{gLast.Title}', '{gLast.URL}')");
            gLast.IsDB = true;
        }

        public void SaveLastComposition()
        {
            Composition cLast = (Composition)composition.SaveLast();
            my.InsertRow($"Insert into composition (ID, Author_ID, Annotation, Status, Title,  URL) values" +
                       $" ({cLast.Id}, {cLast.Author_id},'{cLast.Annotation}', '{cLast.Status}', '{cLast.Title}','{cLast.Url}');");
            cLast.IsDB = true;
        }

        public void SaveLastCompositionGenre()
        {
            CompositionGenre cgLast = (CompositionGenre)compositionGenre.SaveLast();
            my.InsertRow($"Insert into compositiongenre (ID,CompositionID, GenreID)" +
                        $" values ({cgLast.Id}, {cgLast.CompositionID}, {cgLast.GenreID})");
            cgLast.IsDB = true;
        }

        public void AddAuthors(DBEntity dBEntity)
        {
            Authors.Add(dBEntity);
            author.Save();
            SaveLastAuthor();
        }

        public void AddCompositions(DBEntity dBEntity)
        {
            Compositions.Add(dBEntity);
            composition.Save();
            SaveLastComposition();
        }

        public void AddCompositionsGenres(DBEntity dBEntity)
        {
            CompositionGenres.Add(dBEntity);
            compositionGenre.Save();
            SaveLastCompositionGenre();
        }

        public void AddGenres(DBEntity dBEntity)
        {
            Genres.Add(dBEntity);
            genre.Save();
            SaveLastGenre();
        }
    }
}
