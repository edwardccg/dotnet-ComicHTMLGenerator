using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Web.UI;
using System.Collections;

namespace ComicHTMLGenerator
{
    //This solution is abondon, move to Java
    class Program
    {
        static void Main(string[] args)
        {
            while (true)
            {
                Console.WriteLine(@"Please enter the directory: ");
                var searchdir = Console.ReadLine();

                if (!Directory.Exists(searchdir))
                    Console.WriteLine(@"Directory is not valid");

                try
                {
                    GenerateHtml(searchdir);
                    Console.WriteLine(@"Process Done");
                    Console.WriteLine();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    Console.WriteLine(ex.StackTrace);
                    Console.WriteLine();
                }
            }       
        }

        private static void GenerateHtml(string comicdir)
        {
            var comicname = comicdir.Split(Path.DirectorySeparatorChar).Last();
            var imgfiles = Directory.GetFiles(comicdir, "*.*", SearchOption.TopDirectoryOnly)
                .Where(x => x.EndsWith(".jpg") || x.EndsWith(".jpeg") || x.EndsWith(".png") || x.EndsWith(".bmp")).ToArray();
            
            Array.Sort(imgfiles, new ComicNameComparer());

            //generate html for image file from current directory
            if (imgfiles.Length > 0)
            {
                var sb = new StringBuilder("<!DOCTYPE HTML>" + Environment.NewLine);
                var strWriter = new StringWriter(sb);

                using (var wtr = new HtmlTextWriter(strWriter))
                {
                    //head
                    wtr.RenderBeginTag(HtmlTextWriterTag.Html);
                    wtr.RenderBeginTag(HtmlTextWriterTag.Head);
                    wtr.RenderBeginTag(HtmlTextWriterTag.Title);
                    wtr.Write(comicname);
                    wtr.RenderEndTag(); //end title
                    wtr.RenderEndTag(); //end head

                    //body
                    wtr.RenderBeginTag(HtmlTextWriterTag.Body);

                    //start of looping of img file
                    foreach (var imgfile in imgfiles)
                    {
                        wtr.AddAttribute(HtmlTextWriterAttribute.Src, Path.GetFileName(imgfile));
                        wtr.AddAttribute(HtmlTextWriterAttribute.Alt, "");
                        wtr.AddAttribute(HtmlTextWriterAttribute.Width, "1300");
                        wtr.RenderBeginTag(HtmlTextWriterTag.Img);
                        wtr.RenderEndTag(); // end img
                        wtr.RenderBeginTag(HtmlTextWriterTag.Br);
                        wtr.RenderEndTag(); // end br
                    }

                    wtr.RenderEndTag(); //end body
                    wtr.RenderEndTag(); //end html

                    File.WriteAllText(Path.Combine(comicdir, "~index.html"), sb.ToString(), Encoding.UTF8);

                }
            }

            //recursive call when there is sub directory
            var subdirs = Directory.GetDirectories(comicdir).ToList();
            if(subdirs.Count > 0)
            {
                foreach (var subdir in subdirs)
                {
                    GenerateHtml(subdir);
                }
            }

        }
        
        public class ComicNameComparer : IComparer
        {
            int IComparer.Compare(object o1, object o2)
            {
                var o1Num = extractNumber(o1.ToString());
                var o2Num = extractNumber(o2.ToString());

                if (o1Num == 0 && o1Num == 0)
                {
                    return (o1.ToString().CompareTo(o2.ToString()));
                }
                else
                {
                    return (o1Num - o2Num);
                }
            }

            private int extractNumber(string filename)
            {
                int i = 0;
                try
                {
                    var start = filename.LastIndexOf("(") + 1;
                    var end = filename.LastIndexOf(".") - 1;
                    var num = filename.Substring(start, end-start);
                    i = int.Parse(num);
                }
                catch (Exception ex)
                {
                    i = 0;
                }
                return i;
            }
        }

    }
    
}
