using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Xml;

namespace ElkWindowProject
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void btnStart_Click(object sender, RoutedEventArgs e)
        {
            string linkToHtml = "";
            string path = "file.xml";

            linkToHtml = tbWebsite.Text;
            string pattern = @"^(www.|[a-zA-Z].)[a-zA-Z0-9\-\.]+\.(com|pl|edu|ru|gov|mil|net|org|biz|info|name|museum|us|ca|uk)(\:[0-9]+)*(/($|[a-zA-Z0-9\.\,\;\?\'\\\+&amp;%\$#\=~_\-]+))*$";
            Regex url = new Regex(pattern, RegexOptions.Compiled | RegexOptions.IgnoreCase);

            if (url.IsMatch(linkToHtml))
            {
                try
                {
                    WebRequest request;
                    if (linkToHtml.StartsWith("http://") || linkToHtml.StartsWith("https://"))
                    {
                        request = WebRequest.Create(linkToHtml);
                    }
                    else
                    {
                        request = WebRequest.Create("http://" + linkToHtml);
                    }

                    WebResponse response = request.GetResponse();

                    Stream data = response.GetResponseStream();
                    string htmlCode = string.Empty;
                    using (StreamReader sr = new StreamReader(data, Encoding.GetEncoding("utf-8")))
                    {
                        htmlCode = sr.ReadToEnd();
                    }

                    var sbParsedXml = new StringBuilder();
                    var stringWriter = new StringWriter(sbParsedXml);

                    var htmlDoc = new HtmlDocument();
                    htmlDoc.LoadHtml(htmlCode);
                    htmlDoc.OptionOutputAsXml = true;
                    htmlDoc.OptionCheckSyntax = true;
                    htmlDoc.OptionFixNestedTags = true;

                    htmlDoc.Save(stringWriter);

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
                    lbInfo.Foreground = System.Windows.Media.Brushes.Green;
                    lbInfo.Content = "File sucessfuly saved";
                }
                catch (Exception ex)
                {
                    lbInfo.Foreground = System.Windows.Media.Brushes.Red;
                    if (ex.InnerException != null)
                    {
                        lbInfo.Content = ex.InnerException.Message;
                    }
                    else
                    {
                        lbInfo.Content = ex.Message;
                    }
                }
            }
            else
            {
                lbInfo.Foreground = System.Windows.Media.Brushes.Red;
                lbInfo.Content = "Wrong url format";
            }
        }
    }
}
