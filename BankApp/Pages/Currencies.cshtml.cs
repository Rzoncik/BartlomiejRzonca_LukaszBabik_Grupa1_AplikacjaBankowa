using System.Globalization;
using System.Net;
using System.Text.RegularExpressions;
using HtmlAgilityPack;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BankApp.Pages
{
    public class CurrenciesModel : PageModel
    {
        private const string SourceUrl = "https://www.bankier.pl/waluty/kursy-walut/nbp";

        private static readonly string[] Wanted = 
        [
            "USD",
            "EUR",
            "JPY",
            "GBP",
            "AUD",
            "CAD",
            "CHF",
            "CNH",
            "HKD",
            "NZD"
        ];

        public record CurrencyRate(string Name, string Code, decimal RatePln);
        
        public List<CurrencyRate> Rates { get; private set; } = [];

        public async Task OnGetAsync()
        {
            using var http = new HttpClient(new HttpClientHandler { AutomaticDecompression = DecompressionMethods.All });

            http.DefaultRequestHeaders.UserAgent.ParseAdd("BankApp/1.0 (+https://example.com)");

            var html = await http.GetStringAsync(SourceUrl);

            // Strip tags → pure text (makes the parser robust to markup tweaks)
            var doc = new HtmlDocument();
            
            doc.LoadHtml(html);
            
            var text = WebUtility.HtmlDecode(doc.DocumentNode.InnerText);

            // rowRx determinuje, jak mają wyglądać kolumny.
            var rowRx = new Regex(
                @"^(?<name>.+?)\s+(?<qty>\d+)\s+(?<code>[A-Z]{3})\s+(?<rate>\d+,\d+)",
                RegexOptions.Multiline | RegexOptions.Compiled);

            var tmp = new List<CurrencyRate>();

            foreach (Match m in rowRx.Matches(text))
            {
                var code = m.Groups["code"].Value;
                if (code == "CNY") code = "CNH";
                if (!Wanted.Contains(code)) continue;

                var qty = int.Parse(m.Groups["qty"].Value, CultureInfo.InvariantCulture);

                var nbp = decimal.Parse(m.Groups["rate"].Value.Replace(',', '.'), CultureInfo.InvariantCulture);

                // Przeliczenie 100 JPY na 1 JPY.
                tmp.Add(new CurrencyRate(
                    Name: m.Groups["name"].Value.Trim(),
                    Code: code,
                    RatePln: nbp / qty));
            }

            // return list in exact order requested
            Rates = Wanted.Join(tmp, w => w, r => r.Code, (_, r) => r).ToList();
        }
    }
}