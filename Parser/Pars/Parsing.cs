using Parser.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Parser.Pars
{
    class Parsing
    {
        public Author a;
        public Composition c;
        public CompositionGenre cg;
        public Genre g;
        public DataModel dataModel;
        public int IdThread;
        List<string> replace = new List<string>() { @"'", @"'", @"'", @"'", @"👹", @"😀", @"✅", @"😨", @"🔗", @"🚀", @"😻", 
                                                    @"\", @"/", @"🔥", @"🚭", @"🦸🏻‍♂️", @"🎉", @")",
            @";", @"^", @"-", @"+", @"_", @"=", @"🅑", @"🅔", @"🅓", @"🅣", @"🅘", @"🅜", @"🅔", @"🅢", @"🅣", @"🅞", @"🅡", @"🅨", @"  "};

        public Parsing(Author a, Composition c, CompositionGenre cg, Genre g, DataModel dataModel)
        {
            this.a = a;
            this.c = c;
            this.cg = cg;
            this.g = g;
            this.dataModel = dataModel;
        }

        public long? GetAuthor(string Name, string AuthorURL)
        {
            long? AuthorId;
            lock (a)
            {
                AuthorId = a.Contain($"https://author.today/{AuthorURL}");
            }
            if (AuthorId == 0)
            {
                lock (a)
                {
                    return a.Add(new Author(ReplaceString(Name),
                        $"https://author.today/{AuthorURL}")) ;
                }
            }
            return AuthorId;
        }

        public void AddComposition(string URL)
        {
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();
            long? composition;
            string rx = @"<div class=""book-row-content"">(.*?)<div class=""book-title"">(.*?)<a data-pjax href=""(.*?)"">(.*?)<\/a>(.*?)href=""(.*?)"">(.*?)<\/a>";
            var html = GetPage(URL);
            RegexOptions regexOption = RegexOptions.Multiline | RegexOptions.Singleline;
            MatchCollection urls = Regex.Matches(html.ToString(), rx, regexOption);
            long? compID = -1;
            foreach (Match match in urls)
            {
                GroupCollection groups = match.Groups;
                lock(c)
                    composition = c.Contain($"https://author.today/{groups[3].ToString()}");

                if (composition == 0)
                {
                    string url = groups[3].ToString();
                    lock (c)
                    {
                        try
                        {
                            compID = c.Add(new Composition(GetAuthor(groups[7].ToString(), groups[6].ToString()), ReplaceString(groups[4].ToString()),
                                ReplaceString(GetAnnotation(url)),
                                GetStatus(url).Replace("'", ""), $"https://author.today/{url}"));
                            GetCompositionGenre(url, compID);
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e);
                        }
                    }
                    //Console.WriteLine($"Добавлена книга: {groups[4].ToString().Replace("  ", "").Replace($"\r\n", "")}\r\n" +
                    //$"URL: {URL}\r\n" +
                    //$"Поток: { IdThread}");
                    
                }
                //else Console.WriteLine($"Книга: {groups[4].ToString().Replace("  ", "").Replace($"\r\n", "")} уже добавленна\r\n" +
                //    $"Поток: {IdThread}");
            }
            stopWatch.Stop();
            TimeSpan ts = stopWatch.Elapsed;
            Console.WriteLine($"Для парсинга страницы {URL} потребовалось {ts.TotalMilliseconds} в потоке {IdThread}");
        }

        public string GetAnnotation(string urlComposition)
        {
            var html = GetPage(urlComposition);
            string rx = @"<div class=""rich-content"">(.*?)<\/div>";
            RegexOptions regexOption = RegexOptions.Multiline | RegexOptions.Singleline;
            Match urls = Regex.Match(html.ToString(), rx, regexOption);
            string s = urls.Groups[1].ToString().Replace(@"<div class=""mt-sm"">", " ").Replace(@"<br>", "").Replace(@"</br>", "").Replace(@"  ", "").Replace(@"<br/>", "");
            string pattern = @"<a href(.*)";
            string replacement = "";
            string result = Regex.Replace(s, pattern, replacement);
            return result;
        }

        public string GetStatus(string urlComposition)
        {
            var html = GetPage(urlComposition);
            string rx = @"<i class=""icon-pencil book-status-icon""><\/i>(.*?)<\/span>";
            RegexOptions regexOption = RegexOptions.Multiline | RegexOptions.Singleline;
            Match urls = Regex.Match(html.ToString(), rx, regexOption);
            return urls.Groups[1].ToString();
        }

        public void ParsPage(string url, int id)
        {
            Console.WriteLine($"Запущен поток для парсинга {url}");
            var html = GetPage(url);
            Regex rx = new Regex(@"<a href=""(.*?)?page=(.*?)"">(.*?)<");
            MatchCollection urls = rx.Matches(html.ToString());
            List<int> pageList = new List<int>();
            for (int i = 0; i < urls.Count - 1; i++)
            {
                pageList.Add(int.Parse(urls[i].Groups[2].ToString()));
            }

            int page = pageList.Max();
            this.IdThread = id;
            Console.WriteLine($"Страниц в {IdThread} потоке: {page}");


            for (int i = 1; i <= page; i++)
            {
                AddComposition(url + $"?page={i}");
                Console.WriteLine($"Страница {i} из  {page} распаршена для {url}");
            }
        }

        public void GetCompositionGenre(string url, long? composition)
        {
            string html = GetPage(url);
            Regex rx = new Regex(@"<div data-pjax class=""book-genres"">(.*?)<\/div>");
            Match match = rx.Match(html);
            Regex rx2 = new Regex(@"href=""(.*?)"">(.*?)<\/a>");
            MatchCollection matchCollection = rx2.Matches(match.Groups[0].ToString());

            bool first = true;
            long? GenreId;

            foreach (Match m in matchCollection)
            {
                if (first) first = false;
                else
                {
                    lock (g)
                    {
                        GenreId = g.Contain($"https://author.today/{m.Groups[1].ToString()}");
                    }
                    lock (cg)
                    {
                        cg.Add(new CompositionGenre(composition, GenreId));
                    }
                }
            }

        }

        public string GetPage(string url)
        {
            try
            {
                WebClient client = new WebClient();
                var data = client.DownloadData($"https://author.today/{url}");
                return Encoding.UTF8.GetString(data);
            }
            catch (Exception e)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(e.ToString());
                Console.ForegroundColor = ConsoleColor.White;
            }
            return "";
        }

        public string ReplaceString(string str)
        {
            for (int i = 0; i < replace.Count(); i++)
            {
                str.Replace($"{replace[i]}", " ");
            }
            str = Regex.Replace(str, "(?<=')(.*?)'(?=.*')", "$1");
            str.Replace(@"'", "");          
            return str;
        }
    }
}
