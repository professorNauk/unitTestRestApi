using NUnit.Framework;
using RestSharp;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace RestApiTest
{
    [TestFixture]
    public class Tests
    {
        //GetData делает запрос и возвращает список из 50 первых результатов
        public List<string> GetData()
        {
            #region info 
            //чтобы получить 50 первых результатов, нужно выполнить 2 запроса 
            //так как максимальное количество результатов в 1 запросе 25
            //поиск "LINQ" 
            // https://docs.microsoft.com/api/Search?locale=ru-ru&search=linq&$skip=0&$top=25
            // https://docs.microsoft.com/api/Search?locale=ru-ru&search=linq&$skip=25&$top=25
            #endregion

            var client = new RestClient("https://docs.microsoft.com/");
            var request = new RestRequest("api/search/", Method.GET, DataFormat.Xml);
            request.AddParameter("search", "linq");
            request.AddParameter("locale", "ru-ru");
            request.AddParameter("$skip", "0");
            request.AddParameter("$top", "25");

            List<string> results = new List<string>();        //список для сохранения результатов запроса

            var content = client.Execute(request).Content;    //данные после первого запроса 
            var jo = JObject.Parse(content);                  //парсим json
            foreach (var a in jo["results"])
            {
                string s = "";
                try
                {
                    s += a["description"].ToString();
                    s += a["descriptions"][0]["content"].ToString();
                }
                catch { }
                results.Add(s);
            }

            request.AddOrUpdateParameter("$skip", "25");        //меняю параметр запроса
            content = client.Execute(request).Content;          //данные после второго запроса
            jo = JObject.Parse(content);

            foreach (var a in jo["results"])
            {
                string s = "";
                try
                {
                    s += a["description"].ToString();
                    s += a["descriptions"][0]["content"].ToString();
                }
                catch { }
                results.Add(s);
            }

            return results;
        }

        //linqContainsInAllList проверяет содержимое полученного списка 
        //если все строки списка содержат "linq", метод возвращает true 
        public bool linqContainsInAllList(List<string> data)
        {
            foreach (string str in data)
            {
                if (!str.ToLower().Contains("linq"))
                    return false;
            }
            return true;
        }

        [Test]
        public void linqInAllLineTest()
        {
            //если linqContainsInAllList возвратит true, тест выполнится успешно
            bool e = linqContainsInAllList(GetData());
            Assert.AreEqual(e, true);
        }
    }
}