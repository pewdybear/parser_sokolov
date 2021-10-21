using Parser.Model;
using Parser.Pars;
using System;

namespace Parser
{
    class Program
    {
        static public Author a;
        static public Composition c;
        static public CompositionGenre cg;
        static public Genre g;
        static public DataModel dataModel;
        static public CompositionGenreView cgv;

        static void Main(string[] args)
        {
            Console.WriteLine("Идет загрузка данных из БД. Ожидайте...");


            a = new Author();
            c = new Composition();
            cg = new CompositionGenre();
            g = new Genre();
            dataModel = new DataModel(a, c, cg, g);

            Console.WriteLine($"Count str in Author: {Author.Authors.Count()}");
            Console.WriteLine($"Count str in Composition: {Composition.Compositions.Count()}");
            Console.WriteLine($"Count str in CompositionGenre: {CompositionGenre.CompositionGenres.Count()}");
            Console.WriteLine($"Count str in Genre: {Genre.Genres.Count()}");

            cgv = new CompositionGenreView(c, cg, g);

            //Parser
            //Crowler crowler = new Crowler();
            Console.Read();
        }
    }
}
