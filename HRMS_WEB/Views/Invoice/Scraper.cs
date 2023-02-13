using System;
using System.Text.RegularExpressions;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;
using Microsoft.AspNetCore.Mvc;

namespace HRMS_WEB.Views.Invoice
{
    public class Scraper : Controller
    {
        // GET
        public void Index()
        {
            string pdfFile = "C:\\Users\\Sandev\\Desktop\\DEFAULT_WEB-master\\HRMS_WEB\\wwwroot\\invoices\\pdf-test.pdf";
            ITextExtractionStrategy starStrategy = new SimpleTextExtractionStrategy();
            using (PdfReader reader = new PdfReader(pdfFile))
            {
                for (int i = 1; i <= reader.NumberOfPages; i++)
                {
                    string page = PdfTextExtractor.GetTextFromPage(reader, i, starStrategy);
                    Console.WriteLine(page);
                }
            }
            
            
            string input = "Hello, World!";
            string pattern = @"\w+";

            MatchCollection matches = Regex.Matches(input, pattern);
            foreach (Match match in matches)
            {
                Console.WriteLine(match.Value);
            }
        }
    }
}