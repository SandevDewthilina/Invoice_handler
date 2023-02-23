using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using HRMS_WEB.DbContext;
using HRMS_WEB.Entities;
using HRMS_WEB.Models;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;
using Microsoft.AspNetCore.Hosting;
using Newtonsoft.Json;
using Path = System.IO.Path;

namespace HRMS_WEB.Repositories
{
    public class Scraper : IScraper
    {
        private readonly IWebHostEnvironment _hostEnvironment;

        public Scraper(IWebHostEnvironment hostEnvironment)
        {
            _hostEnvironment = hostEnvironment;
        }

        public async Task<List<object>> Scrape(string filepath, List<RegexComponent> regexComponents, Upload upload)
        {
            string content = "";
            string fullPath = Path.Combine(_hostEnvironment.WebRootPath, filepath);
            var fields = new List<object>();

            ITextExtractionStrategy starStrategy = new SimpleTextExtractionStrategy();

            // read the content from the pdf
            using (PdfReader reader = new PdfReader(fullPath))
            {
                for (int i = 1; i <= reader.NumberOfPages; i++)
                {
                    content += PdfTextExtractor.GetTextFromPage(reader, i, starStrategy);
                }
            }


            // run offline regex
            foreach (var regex in regexComponents)
            {
                if (regex.IsArea)
                {
                    continue;
                }

                string pattern = regex.Value;
                string groupName = regex.Key;

                // without using OCR
                var obj = GetCapturedGroup(content, pattern, groupName);
                if (obj != null)
                {
                    fields.Add(obj);
                }
            }

            // use OCR for online regex
            var onlineRegex = regexComponents.Where(c => c.IsArea).ToList();
            // prepare the request
            var body = new
            {
                file_url = "http://localhost:8100/" + upload.FilePath,
                file_name = upload.FileName,
                upload_name = upload.ID,
                regexComponents = onlineRegex
            };
            var json = JsonConvert.SerializeObject(body);
            using (var client = new HttpClient())
            {
                var stringContent = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await client.PostAsync("http://localhost:6000/getTextOfArea", stringContent);
                response.EnsureSuccessStatusCode();
                var textList = await response.Content.ReadAsStringAsync();


                var responses = JsonConvert.DeserializeObject<List<TextResponse>>(textList);
                if (responses != null)
                {
                    foreach (TextResponse keyTextPair in responses)
                    {
                        var key = keyTextPair.key;
                        var text = keyTextPair.text;
                        string pattern = "";

                        var regexPatternForKey = onlineRegex.FirstOrDefault(c => c.Key.Equals(key));
                        if (regexPatternForKey != null)
                        {
                            pattern = regexPatternForKey.Value;
                        }

                        if (pattern != null && !pattern.Equals(""))
                        {
                            var obj = GetCapturedGroup(text, pattern, key);
                            if (obj != null)
                            {
                                fields.Add(obj);
                            }
                        }
                        else
                        {
                            fields.Add(new
                            {
                                Key = key,
                                Value = text
                            });
                        }
                    }
                }
            }


            return fields;
        }

        public object GetCapturedGroup(string content, string pattern, string groupName)
        {
            MatchCollection matches = Regex.Matches(content, pattern);
            var stop = false;
            foreach (Match match in matches)
            {
                foreach (Group group in match.Groups)
                {
                    if (group.Name == groupName)
                    {
                        return new
                        {
                            Key = groupName,
                            Value = group.Value
                        };
                    }
                }
            }

            return null;
        }
    }

    public class TextResponse
    {
        public string key { get; set; }
        public string text { get; set; }
    }
}