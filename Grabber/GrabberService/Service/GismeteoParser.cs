using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AngleSharp.Html.Dom;
using AngleSharp.Html.Parser;
using GrabberService.Dao.Interfaces;
using GrabberService.Models;
using GrabberService.Service.Interfaces;

namespace GrabberService.Service
{
    public class GismeteoParser : IGismeteoParser
    {
        private readonly IGismeteoGetter gismeteoGetter;
        private readonly IMongoContext mongoContext;
        private const int Days = 10;

        public GismeteoParser(IGismeteoGetter gismeteoGetter, IMongoContext mongoContext)
        {
            this.gismeteoGetter = gismeteoGetter;
            this.mongoContext = mongoContext;
        }

        public async Task<string> Do()
        {
            await mongoContext.Add(
                new
                {
                    Date = DateTime.Now,
                    Data = await GetWeatherInfo(await ParseMainPage(await gismeteoGetter.GetMainPage()))
                }
            );

            return $"Ок at {DateTime.Now}";
        }

        private async Task<Dictionary<string, string>> ParseMainPage(string htmlString)
        {
            var parser = new HtmlParser();
            var document = await parser.ParseDocumentAsync(htmlString);

            return document.GetElementsByClassName("cities-popular")
                .FirstOrDefault()?
                .QuerySelectorAll("[class='list-item']")
                .Select(l => new KeyValuePair<string, string>(
                        l.FirstElementChild?.Attributes["href"]?.Value ?? string.Empty,
                        l.FirstElementChild?.TextContent
                    )).ToList().ToDictionary(x => x.Key, x => x.Value);
        }

        private async Task<List<CityWeather>> GetWeatherInfo(Dictionary<string, string> mainCities)
        {
            var getTasks = mainCities.Keys.Select(i => Task.Run(() => gismeteoGetter.GetCityWeatherPage(i, mainCities))).ToList();
            await Task.WhenAll(getTasks);
            
            var parseTasks = getTasks.Select(i => Task.Run(() => 
                ParseWeatherInfo(i.Result.Value,i.Result.Key))).ToList();
            await Task.WhenAll(parseTasks);

            return parseTasks.Select(i => i.Result).ToList();
        }

        private async Task<CityWeather> ParseWeatherInfo(string cityPageHtml, string cityName)
        {
            var parser = new HtmlParser();
            var document = await parser.ParseDocumentAsync(cityPageHtml);

            var dayWeatherList = new List<DayWeather>();
            for (int i = 0; i < Days; i++)
            {
                dayWeatherList.Add(
                    new DayWeather
                    {
                        TemperatureC = ParseDailyTemperature(document).ElementAtOrDefault(i),
                        WeatherDescription = ParseDailyWeather(document).ElementAtOrDefault(i) ?? string.Empty,
                        WindSpeed = ParseDailyWidSpeed(document).ElementAtOrDefault(i) ?? string.Empty,
                        Precipitation = ParseDailyPrecipitation(document).ElementAtOrDefault(i) ?? string.Empty,
                        PressureAtm = ParseDailyPressure(document).ElementAtOrDefault(i),
                        Humidity = ParseDailyHumidity(document).ElementAtOrDefault(i) ?? string.Empty,
                        Geomagnetic = ParseDailyGeomagnetic(document).ElementAtOrDefault(i) ?? string.Empty,
                    });
            }

            return new CityWeather
            {
                CityName = cityName,
                Date = DateTime.Now,
                WeatherByDays = dayWeatherList,
            };
        }

        private List<DailyTemperature> ParseDailyTemperature(IHtmlDocument document)
        {
            var classWidgetTemperature = document.GetElementsByClassName("widget-row-chart widget-row-chart-temperature")
                .FirstOrDefault();
            
            return classWidgetTemperature?.QuerySelectorAll("[class='unit unit_temperature_c']")
                .Select((x, y) => new {Index = y, Value = x.TextContent})
                .GroupBy(x => x.Index / 2)
                .Select( x => new DailyTemperature
                {
                    Maximum = x.ElementAtOrDefault(0)?.Value ?? string.Empty,
                    Minimum = x.ElementAtOrDefault(1)?.Value ?? string.Empty,
                })
                .ToList();
        }

        private List<string> ParseDailyWeather(IHtmlDocument document)
        {
            var classWigetWeather=  document.GetElementsByClassName("widget-row widget-row-icon")
                .FirstOrDefault();

            return classWigetWeather?.QuerySelectorAll("[class='weather-icon tooltip']")
                .Select(x => x.Attributes["data-text"]?.Value ?? string.Empty)
                .ToList();
        }

        private List<string> ParseDailyWidSpeed(IHtmlDocument document)
        {
            var classWigetWindSpeed=  document.GetElementsByClassName("widget-row widget-row-wind-gust row-with-caption")
                .FirstOrDefault();
            
            return classWigetWindSpeed?.QuerySelectorAll("[class='row-item']")
                .Select(x => x.Children)
                .Select(x => x.ElementAtOrDefault(1)?.TextContent ?? string.Empty)
                .ToList();
        }

        private List<string> ParseDailyPrecipitation(IHtmlDocument document)
        {
            var precipitationClass = document.GetElementsByClassName("widget-row widget-row-precipitation-bars row-with-caption")
                .FirstOrDefault();
            
           return precipitationClass?.QuerySelectorAll("[class='row-item']")
               .Select(x => x?.TextContent ?? string.Empty)
               .ToList();
        }

        private List<DailyPressure> ParseDailyPressure(IHtmlDocument document)
        {
            var pressure = document.GetElementsByClassName("widget-row-chart widget-row-chart-pressure").FirstOrDefault();
            
            return pressure?.QuerySelectorAll("[class='unit unit_pressure_mm_hg_atm']")
                .Select((x, y) => new {Index = y, Value = x.TextContent})
                .GroupBy(x => x.Index / 2)
                .Select( x => new DailyPressure
                {
                    Maximum = x.ElementAtOrDefault(0)?.Value ?? string.Empty,
                    Minimum = x.ElementAtOrDefault(1)?.Value ?? string.Empty,
                })
                .ToList();
        }

        private List<string> ParseDailyHumidity(IHtmlDocument document)
        {
            var humidity = document.GetElementsByClassName("widget-row widget-row-humidity").FirstOrDefault();
            
            return humidity?.Children
                .Select(x => x?.TextContent ?? string.Empty)
                .ToList();
        }

        private List<string> ParseDailyGeomagnetic(IHtmlDocument document)
        {
            var geomagnetic = document.GetElementsByClassName("widget-row widget-row-geomagnetic").FirstOrDefault();
            return geomagnetic?.Children
                .Select(x => x?.TextContent ?? string.Empty)
                .ToList();
        }
    }
}