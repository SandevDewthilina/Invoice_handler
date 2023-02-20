using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using HRMS_WEB.Models;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;
using Microsoft.AspNetCore.Hosting;
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

        public List<object> Scrape(string filepath, List<RegexComponent> regexComponents)
        {
            string content = "";
            string fullPath = Path.Combine(_hostEnvironment.WebRootPath, filepath);
            ITextExtractionStrategy starStrategy = new SimpleTextExtractionStrategy();
            using (PdfReader reader = new PdfReader(fullPath))
            {
                for (int i = 1; i <= reader.NumberOfPages; i++)
                {
                    content += PdfTextExtractor.GetTextFromPage(reader, i, starStrategy);
                }
            }

            var fields = new List<object>();
                // string pattern = "(?<Heading>^.*)\n";
            foreach (var regex in regexComponents)
            {
                string pattern = regex.Value;
                string groupName = regex.Key;

                var obj = GetCapturedGroup(content, pattern, groupName);
                if (obj != null)
                {
                    fields.Add(obj);
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
}