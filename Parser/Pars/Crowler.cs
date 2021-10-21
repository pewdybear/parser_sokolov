using Parser.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;

namespace Parser.Pars
{
    public class ThreadParams
    {
        public string url;
        public int IdThread;


    }

    class Crowler
    {
        public Author a;
        public Composition c;
        public CompositionGenre cg;
        public Genre g;
        public DataModel dataModel;
        Parsing parsing;
        List<string> categories;
        public int ThreadID = 0;

        public Crowler()
        {
            a = new Author();
            c = new Composition();
            cg = new CompositionGenre();
            g = new Genre();
            dataModel = new DataModel(a, c, cg, g);
            
            GetGenre();
        }

        public void GetGenre()
        {
            WebClient client = new WebClient();
            var data = client.DownloadData(Settings.URL);
            var html = Encoding.UTF8.GetString(data);
            Regex rx = new Regex(@"<a href=""(.*?)"" class=""(.*?)"">(.*?)<\/a>");
            MatchCollection urls = rx.Matches(html.ToString());
            categories = new List<string>();
            int iter = 0;
            foreach (Match match in urls)
            {
                GroupCollection groups = match.Groups;
                //if (iter > 12 && iter < 72)
                //{
                if (groups[1].ToString().Contains("genre") && Char.IsLetter(groups[2].ToString()[0]))
                {
                    if (g.Contain($"https://author.today/{groups[1].ToString()}") == 0)
                        g.Add(new Genre($"{groups[3].ToString()}", $"https://author.today/{groups[1].ToString()}"));
                    categories.Add(groups[1].ToString());
                    iter++;
                }
                //}
                //else iter++;
            }

            for (int i = 0; i < categories.Count(); i++)
                Console.WriteLine(i + ": " + categories[i]);
            int ThreadAll = 0;
            //for (int i = 0; i < Math.Ceiling(categories.Count() / (double)Settings.Threads); i++)
            //{
            //    Thread[] threads;
            //    ThreadParams[] threadParams;
            //    int countThread = categories.Count() - 1;
            //    threads = new Thread[Settings.Threads];
            //    threadParams = new ThreadParams[Settings.Threads];
            //    int a = i * Settings.Threads;
            //    int b = (i * Settings.Threads + Settings.Threads);
            //    if (b > categories.Count())
            //    {
            //        b = categories.Count();
            //        threads = new Thread[b - a + 1];
            //        threadParams = new ThreadParams[b - a + 1];
            //    }

                
            //    int thread = 0;
            //    for (int j = a; j < b; j++) 
            //    {
            //        threads[thread] = new Thread(FindAll);
            //        threadParams[thread] = new ThreadParams
            //        {
            //            url = categories[j + 1],
            //            IdThread = j
            //        };
            //        thread++;
            //        ThreadAll++;
            //    }

            //    for (int k = 0; k < Settings.Threads; k++) threads[k].Start(threadParams[k]);
            //    for (int k = 0; k < Settings.Threads; k++) threads[k].Join();
            //    Console.WriteLine($"{i} СТАК ЗАВЕРШИЛ РАБОТУ");
            //}

            Thread[] threads;
            ThreadParams[] threadParams;
            int countThread = categories.Count() - 1;
            threads = new Thread[Settings.Threads];
            threadParams = new ThreadParams[Settings.Threads];

            int thread = 0;
            for (int i = 0; i < Settings.Threads; i++)
            {
                threads[thread] = new Thread(FindAll);
                threadParams[thread] = new ThreadParams
                {
                    url = categories[i + 1],
                    IdThread = i
                };
                thread++;
                ThreadAll++;
                ThreadID++;
            }

            for (int k = 0; k < Settings.Threads; k++) threads[k].Start(threadParams[k]);
            for (int k = 0; k < Settings.Threads; k++) threads[k].Join();
            Console.WriteLine($"Парсинг завершен для потоков: {ThreadAll}");
            Console.Read();
        }

        public void FindAll(object all)
        {
            parsing = new Parsing(a, c, cg, g, dataModel);
            ThreadParams thread = (ThreadParams)all;
            parsing.ParsPage(thread.url, thread.IdThread);
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"ПОТОК {thread.IdThread} ЗАВЕРШИЛ РАБОТУ");
            Console.ForegroundColor = ConsoleColor.White;

            //lock (categories)
            //{
            if (ThreadID + 1 < categories.Count())
            {
                ThreadParams p = new ThreadParams
                {
                    url = categories[ThreadID + 1],
                    IdThread = ThreadID + 1
                };
                ThreadID++;
                FindAll(p);
            }
            //}
        }
    }
}
