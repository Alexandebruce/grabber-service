using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AngleSharp.Dom;
using AngleSharp.Html.Parser;
using GrabberService.Models;
using GrabberService.Service.Interfaces;

namespace GrabberService.Service
{
    public class GismeteoParser : IGismeteoParser
    {
        private readonly IGismeteoGetter gismeteoGetter;
        private const int Days = 10;

        public GismeteoParser(IGismeteoGetter gismeteoGetter)
        {
            this.gismeteoGetter = gismeteoGetter;
        }

        public async Task Do()
        {
            //var mainPage = await gismeteoGetter.GetMainPage();
            
            //var weatherInfo = GetWeatherInfo(await ParseMainPage(mainPage));
            
            var volgograd = await gismeteoGetter.GetCityWeatherPage("test");
            var parser = new HtmlParser();
            var document = await parser.ParseDocumentAsync(volgograd);

            var classWigetTemperature = document.GetElementsByClassName("widget-row-chart widget-row-chart-temperature")
                .FirstOrDefault();
            
            var dailyTemperature = classWigetTemperature?.QuerySelectorAll("[class='unit unit_temperature_c']")
                .Select((x, y) => new {Index = y, Value = x.TextContent})
                .GroupBy(x => x.Index / 2)
                .Select( x => new DailyTemperature
                    {
                        Maximum = x.ElementAtOrDefault(0)?.Value ?? string.Empty,
                        Minimum = x.ElementAtOrDefault(1)?.Value ?? string.Empty,
                    })
                .ToList();
            
            var classWigetWeather=  document.GetElementsByClassName("widget-row widget-row-icon")
                .FirstOrDefault();
            var weatherlist  = classWigetWeather?.QuerySelectorAll("[class='weather-icon tooltip']");
            var dailyWeather = weatherlist.Select(x => x.Attributes["data-text"]?.Value ?? string.Empty).ToList();

            
            
            
            var dayWeatherList = new List<DayWeather>();
            for (int i = 0; i < Days; i++)
            {
                dayWeatherList.Add(
                    new DayWeather
                    {
                        TemperatureC = dailyTemperature.ElementAtOrDefault(i),
                        WeatherDescription = dailyWeather.ElementAtOrDefault(i) ?? string.Empty
                    });
            }

            var cityWeather = new CityWeather
            {
                CityName = "Волгоград",
                Date = DateTime.Now,
                WeatherByDays = dayWeatherList,
            };

            Console.WriteLine("Кек");
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