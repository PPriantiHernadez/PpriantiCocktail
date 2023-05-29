using Microsoft.AspNetCore.Mvc;
using ML;
using Newtonsoft.Json;
using System.Drawing.Text;
using System.Net.Http;

namespace PL.Controllers
{
    public class DrinksController : Controller
    {
        [HttpGet]
        public IActionResult GetAll()
        {
            ML.Drink drink = new ML.Drink();
            drink.drinks = new List<object>();

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://www.thecocktaildb.com/api/json/v1/1/");

                var responseTask = client.GetAsync("search.php?f=a");
                responseTask.Wait(); //esperar a que se resuelva la llamada al servicio

                var result = responseTask.Result;

                if (result.IsSuccessStatusCode)
                {
                    var readTask = result.Content.ReadAsAsync<dynamic>();
                    readTask.Wait();

                    foreach (var resultItem in readTask.Result.drinks)
                    {
                        ML.Drink resultItemList = Newtonsoft.Json.JsonConvert.DeserializeObject<ML.Drink>(resultItem.ToString());
                        drink.drinks.Add(resultItemList);
                    }
                }
            }
            return View(drink);
        }

        [HttpPost]
        public IActionResult GetAll(ML.Drink drink)
        {
           
            drink.drinks = new List<object>();

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://www.thecocktaildb.com/api/json/v1/1/");

                    var responseTask = client.GetAsync("search.php?s=" + drink.strDrink);
             
                responseTask.Wait(); //esperar a que se resuelva la llamada al servicio

                var result = responseTask.Result;

                if (result.IsSuccessStatusCode)
                {
                    var readTask = result.Content.ReadAsAsync<dynamic>();
                    readTask.Wait();

                    foreach (var resultItem in readTask.Result.drinks)
                    {
                        ML.Drink resultItemList = Newtonsoft.Json.JsonConvert.DeserializeObject<ML.Drink>(resultItem.ToString());
                        drink.drinks.Add(resultItemList);
                    }
                }
            }
            return View(drink);
        }

    
        private readonly HttpClient _httpClient;

        [HttpPost]
        public async Task<IActionResult> SearchByIngredient(string ingredient)
        {
            var url = $"https://www.thecocktaildb.com/api/json/v1/1/filter.php?i={ingredient}";
            var response = await _httpClient.GetAsync(url);

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadAsAsync<dynamic>();
                return Json(result);
            }

            return BadRequest();
        }


    }
}
