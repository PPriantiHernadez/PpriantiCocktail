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

                    foreach (dynamic resultItem in readTask.Result.drinks)
                    {

                        ML.Drink resultItemList = new ML.Drink();
                        resultItemList.idDrink = resultItem.idDrink;
                        resultItemList.strDrinkThumb = resultItem.strDrinkThumb;
                        resultItemList.strDrink = resultItem.strDrink;
                        resultItemList.strCategory = resultItem.strCategory;
                        resultItemList.strIngredient1 = resultItem.strIngredient1;
                        resultItemList.strIngredient2 = resultItem.strIngredient2;
                        resultItemList.strIngredient3 = resultItem.strIngredient3;
                        resultItemList.strIngredient4 = resultItem.strIngredient4;
                        resultItemList.strIngredient1 = "https://www.thecocktaildb.com/images/ingredients/" + resultItemList.strIngredient1 + "-Small.png";
                        resultItemList.strIngredient2 = "https://www.thecocktaildb.com/images/ingredients/" + resultItemList.strIngredient2 + "-Small.png";
                        resultItemList.strIngredient3 = "https://www.thecocktaildb.com/images/ingredients/" + resultItemList.strIngredient3 + "-Small.png";
                        resultItemList.strIngredient4 = "https://www.thecocktaildb.com/images/ingredients/" + resultItemList.strIngredient4 + "-Small.png";
                        resultItemList.strIngredient5 = "https://www.thecocktaildb.com/images/ingredients/" + resultItemList.strIngredient5 + "-Small.png";
                        resultItemList.strInstructions = resultItem.strInstructions;
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

                        drink.strIngredient1 = "https://www.thecocktaildb.com/images/ingredients/" + resultItemList.strIngredient1 + "-Small.png";
                        drink.strIngredient2 = "https://www.thecocktaildb.com/images/ingredients/" + resultItemList.strIngredient2 + "-Small.png";
                        drink.strIngredient3 = "https://www.thecocktaildb.com/images/ingredients/" + resultItemList.strIngredient3 + "-Small.png";
                        drink.strIngredient4 = "https://www.thecocktaildb.com/images/ingredients/" + resultItemList.strIngredient4 + "-Small.png";
                        drink.strIngredient5 = "https://www.thecocktaildb.com/images/ingredients/" + resultItemList.strIngredient5 + "-Small.png";

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
                ML.Drink drink = new ML.Drink();

            
                return Json(result);

            }

            return BadRequest();
        }



    }
}
