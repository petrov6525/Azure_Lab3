using Azure_Lab3.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Text;
using System.Text.Json;

namespace Azure_Lab3.Controllers
{
    public class HomeController : Controller
    {
        private DataClass? data { get; set; }

        public HomeController()
        {
            data = new DataClass();
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {

            return View();
        }


        public async Task<IActionResult> AddNewLot()
        {
            string currency = Request.Form["currency"];
            string lastName = Request.Form["lastname"];
            string sumString = Request.Form["sum"];
            double sum = double.Parse( sumString.Replace(".", ","));
            

            Lot lot = new Lot()
            {
                Currency = currency,
                LastName = lastName,
                Sum = sum
            };

            var message=JsonSerializer.Serialize(lot);

            await QueueService.SendMessageAsync(message);

            

            return View("Index");
        }


        public async Task<IActionResult> GetLotsByCurrency()
        {
            data=await Request.ReadFromJsonAsync<DataClass>();


            var list = await QueueService.GetLotsByCurrency(data.Data);


            

            

            return Json(list);
        }


        public async Task<IActionResult> ByLot()
        {
            var data = await Request.ReadFromJsonAsync<DataClass>();

            var id = data.Data;



            var isDelete = await QueueService.DeleteMessageById(id);

            if (!isDelete) data.Data = null;
            



            return Json(data);
        }




    }

}