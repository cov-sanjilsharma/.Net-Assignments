using Ecommerce_DBFirst.Dtos;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Json;

namespace Ecommerce_DBFirst.Controllers
{
    public class InventoryController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<InventoryController> _logger;

        public InventoryController(IHttpClientFactory httpClientFactory, ILogger<InventoryController> logger)
        {
            _httpClientFactory = httpClientFactory;
            _logger = logger;
        }

        // GET /Inventory
        public async Task<IActionResult> Index()
        {
            var client = _httpClientFactory.CreateClient("InventoryApi");
            List<InventoryDTO> items = new();

            try
            {
                var response = await client.GetAsync("api/inventory");
                if (response.IsSuccessStatusCode)
                {
                    items = await response.Content.ReadFromJsonAsync<List<InventoryDTO>>() ?? new();
                }
                else
                {
                    ViewBag.Error = "Inventory service returned an error. Please try again shortly.";
                    _logger.LogError("Inventory API call failed with status {Status}", response.StatusCode);
                }
            }
            catch (HttpRequestException ex)
            {
                ViewBag.Error = "Could not reach the Inventory service. Please try again shortly.";
                _logger.LogError(ex, "Inventory API is unreachable.");
            }

            return View(items);
        }

        // GET /Inventory/ForProduct/5  -- used for "show stock availability for a product"
        public async Task<IActionResult> ForProduct(int id)
        {
            var client = _httpClientFactory.CreateClient("InventoryApi");

            try
            {
                var response = await client.GetAsync($"api/inventory/product/{id}");
                if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    ViewBag.Error = "No stock record for this product yet.";
                    return View(new InventoryDTO { ProductId = id, Quantity = 0 });
                }

                if (!response.IsSuccessStatusCode)
                {
                    ViewBag.Error = "Inventory service returned an error.";
                    return View(new InventoryDTO { ProductId = id });
                }

                var item = await response.Content.ReadFromJsonAsync<InventoryDTO>();
                return View(item);
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "Inventory API is unreachable.");
                ViewBag.Error = "Could not reach the Inventory service. Please try again shortly.";
                return View(new InventoryDTO { ProductId = id });
            }
        }
    }
}