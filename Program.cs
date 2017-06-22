using HtmlAgilityPack;
using System;
using System.Collections.Generic;
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

                var htmlDoc = new HtmlDocument();
                htmlDoc.LoadHtml(htmlCode);
                htmlDoc.OptionOutputAsXml = true;
                htmlDoc.OptionCheckSyntax = true;
                htmlDoc.OptionFixNestedTags = true;

                htmlDoc.Save(stringWriter);

                Console.WriteLine("Let's convert it to JSON");
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(sbParsedXml.ToString());

                List<int> startIndexes = new List<int>();
                List<int> endIndexes = new List<int>();

                string json = Newtonsoft.Json.JsonConvert.SerializeXmlNode(xmlDoc);

                int counter = 0;

                for (int index = json.IndexOf("/*"); index >= 0; index = json.IndexOf("/*", index + 1))
                {
                    counter++;
                }

                if (counter > 0)
                {
                    for (int i = 0; i < counter; i++)
                    {
                        for (int index = json.IndexOf("/*"); index >= 0; index = json.IndexOf("/*", index + 1))
                        {
                            startIndexes.Add(index);
                            break;
                        }
                        for (int index = json.IndexOf("*/"); index >= 0; index = json.IndexOf("*/", index + 1))
                        {
                            endIndexes.Add(index);
                            break;
                        }

                        if (endIndexes[i] + 2 > startIndexes[i])
                        {
                            string toCut = json.Substring(startIndexes[i], endIndexes[i] + 2 - startIndexes[i]);
                            json = json.Replace(toCut, string.Empty);
                        }
                    }
                }


                File.WriteAllText(path, json);
                Console.WriteLine("File saved");

                //===========================================Read Content==========================================================

                /*var webGet = new HtmlWeb();
                var document = webGet.Load("http://"+linkToHtml);

                var aTags = document.DocumentNode.SelectNodes("//a");
                int counter2 = 1;
                StringBuilder sbb = new StringBuilder();
                if (aTags != null)
                {
                    foreach (var aTag in aTags)
                    {
                        sbb.Append(counter2 + ". " + aTag.InnerHtml + " - " + aTag.Attributes["href"].Value + "\t" + "<br />");
                        counter++;
                    }
                }

                 File.WriteAllText(path, document.ToString());    */
                 
            }
        }
    }
}