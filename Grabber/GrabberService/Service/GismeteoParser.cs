using System;
using System.Linq;
using System.Threading.Tasks;
using AngleSharp.Html.Parser;
using GrabberService.Service.Interfaces;

namespace GrabberService.Service
{
    public class GismeteoParser : IGismeteoParser
    {
        private readonly IGismeteoGetter gismeteoGetter;

        public GismeteoParser(IGismeteoGetter gismeteoGetter)
        {
            this.gismeteoGetter = gismeteoGetter;
        }

        public async Task Do()
        {
            var mainPage = await gismeteoGetter.GetMainPage();
            
            await ParseMainPage(mainPage);
            //Console.WriteLine(mainPage);
            //получаем модель с городами
            //запускаем асинхронно получение данных для всех городов
            //waitAll все задачи получения городов и парсинга
            //формируем объекты для монго
            //пишем в монго данные
            //пишем в лог, что всё ок?
        }

        private async Task<List<string>> ParseMainPage(string htmlString)
        {
            var parser = new HtmlParser();
            var document = await parser.ParseDocumentAsync(htmlString);

            return document.GetElementsByClassName("cities-popular")
                .FirstOrDefault()?
                .QuerySelectorAll("[class='list-item']")
                .Select(l => l.FirstElementChild?.Attributes["href"]?.Value ?? string.Empty)
                .ToList();
        }

        private async Task<List<CityWeather>> GetWeatherInfo(List<string> cityUrls)
        {
            var getTasks = cityUrls.Select(i => Task.Run(() => gismeteoGetter.GetCityWeatherPage(i))).ToList();
            await Task.WhenAll(getTasks);
            
            var parseTasks = getTasks.Select(i => Task.Run(() => ParseWeatherInfo(i.Result))).ToList();
            await Task.WhenAll(parseTasks);

            return parseTasks.Select(i => i.Result).ToList();
        }

        private async Task<CityWeather> ParseWeatherInfo(string cityPageHtml)
        {
            var parser = new HtmlParser();
            var document = await parser.ParseDocumentAsync(cityPageHtml);
            
            /*return document.GetElementsByClassName("cities-popular")
                .FirstOrDefault()?
                .QuerySelectorAll("[class='list-item']")
                .Select(l => l.FirstElementChild?.Attributes["href"]?.Value ?? string.Empty)
                .ToList();*/
            return null;
        }
    }
}