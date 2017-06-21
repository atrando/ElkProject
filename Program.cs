using System;
using System.IO;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;

namespace ReadHTMLToJson
{
    class Program
    {
        static void Main(string[] args)
        {
            //========================================================================================================
            Console.WriteLine("Type the website url in format: www.website.com");
            string linkToHtml = "";
            string path = "file.xml";

            linkToHtml = Console.ReadLine();
            Console.ReadKey();

            if (linkToHtml != null && !linkToHtml.Equals(string.Empty))
            {
                WebRequest request = WebRequest.Create("http://" + linkToHtml);
                WebResponse response = request.GetResponse();

                Stream data = response.GetResponseStream();
                string htmlCode = string.Empty;
                using (StreamReader sr = new StreamReader(data, Encoding.GetEncoding("utf-8")))
                {
                    htmlCode = sr.ReadToEnd();
                }

                if (htmlCode != null && !htmlCode.Equals(""))
                {
                    Console.WriteLine("HTML downloaded");
                }
                else
                {
                    Console.WriteLine("No HTML downloaded from the website");
                }

                var sbParsedXml = new StringBuilder();
                var stringWriter = new StringWriter(sbParsedXml);

                var htmlDoc = new HtmlAgilityPack.HtmlDocument();
                htmlDoc.LoadHtml(htmlCode);
                htmlDoc.OptionOutputAsXml = true;
                htmlDoc.OptionCheckSyntax = true;
                htmlDoc.OptionFixNestedTags = true;

                htmlDoc.Save(stringWriter);

                Console.WriteLine("Let's convert it to JSON");
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(sbParsedXml.ToString());

                string json = Newtonsoft.Json.JsonConvert.SerializeXmlNode(doc);
                File.WriteAllText(path, json);
                Console.ReadLine();
            }
        }

        static string RemoveBetween(string s, string begin, string end)
        {
            Regex regex = new Regex(string.Format("\\{0}.*?\\{1}", begin, end));
            return regex.Replace(s, string.Empty);
        }
    }
}