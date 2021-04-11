using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Net.Http;
using System.Threading.Tasks;
using Debt.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Configuration;

namespace Debt.Controllers
{
    public class HomeController : Controller
    {
        static HttpClient client = new HttpClient();

        public async Task<ActionResult> Index()
        {
            try
            {            
                List<Countries> countries = new List<Countries>();
                var baseAddress = new Uri(ConfigurationManager.AppSettings["CountryURI"]);
                using (var client = new HttpClient { BaseAddress = baseAddress })
                {
                    HttpResponseMessage response = await client.GetAsync("countries");
               
                    string responseData = await response.Content.ReadAsStringAsync();
                    var objCountries = JsonConvert.DeserializeObject(responseData);
                    var jo = JObject.Parse(responseData);

                    foreach (var countryObj in jo.Root["results"])
                    {
                        Countries country = new Countries();
                        country.Code = countryObj["code"].ToString();
                        country.Name = countryObj["name"].ToString();
                        countries.Add(country);
                    }
                }

                return View(countries);
            }
            catch (AggregateException ex)
            {
                return View(ex.Message);
            }
            catch (Exception ex)
            {
                return View(ex.Message);
            }
        }

        public async Task<ActionResult> GetCity(string countryCode)
        {
            try
            {            
                List<City> cities = new List<City>();
                var baseAddress = new Uri(ConfigurationManager.AppSettings["CityURI"]);
                using (var client = new HttpClient { BaseAddress = baseAddress })
                {
                    HttpResponseMessage response = await client.GetAsync("?country="+ countryCode);
                
                    string responseData = await response.Content.ReadAsStringAsync();
                    var jo = JObject.Parse(responseData);
                    foreach(var cityObj in jo.Root["results"])
                    {
                        City city = new City();
                        city.Name = cityObj["city"].ToString();
                        string strParams = cityObj["parameters"].ToString().Replace("\r\n", string.Empty);
                        strParams = strParams.Trim(new Char[] { '[', ']' });
                        strParams = strParams.Replace(@"\", "");
                        strParams = strParams.Replace(@"""", "");
                        city.Parameters = strParams.Split(',').ToList(); ;
                        cities.Add(city);
                    }
                }
                return View(cities);
            }
            catch (AggregateException ex)
            {
                return View(ex.Message);
            }
            catch (Exception ex)
            {
                return View(ex.Message);
            }
        }

        public async Task<ActionResult> GetAQ(string city, string param)
        {
            try
            {
                var baseAddress = new Uri(ConfigurationManager.AppSettings["MeasurementURI"]);            
                using (var client = new HttpClient { BaseAddress = baseAddress })
                {
                    HttpResponseMessage response = await client.GetAsync("?city=" + city + "&parameter=" + param + "&limit=1");
                    string responseData = await response.Content.ReadAsStringAsync();
                    var jo = JObject.Parse(responseData);
                
                    foreach (var aq in jo.Root["results"])
                    {                    
                        ViewBag.LocationId = int.Parse(aq["locationId"].ToString());
                        ViewBag.Location = aq["location"].ToString();
                        ViewBag.Parameter = param;
                        ViewBag.Value = double.Parse(aq["value"].ToString());
                    }
                }
                return View();
            }
            catch (AggregateException ex)
            {
                return View(ex.Message);
            }
            catch (Exception ex)
            {
                return View(ex.Message);
            }
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}