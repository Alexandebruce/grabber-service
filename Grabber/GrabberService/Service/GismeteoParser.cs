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

        private async Task ParseMainPage(string htmlString)
        {
            var parser = new HtmlParser();
            var document = await parser.ParseDocumentAsync(htmlString);

            var classCities = document.GetElementsByClassName("cities-popular").FirstOrDefault();
            
            var links = classCities?.QuerySelectorAll("[class='list-item']");

            foreach (var link in links)
            {
                var sss = link.FirstElementChild?.Attributes["href"]?.Value;
                Console.WriteLine(sss);
            }
        }
    }
}