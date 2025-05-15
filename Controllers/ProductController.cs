using Microsoft.AspNetCore.Mvc;

namespace MainProject.Controllers
{
    [ApiController]
    [Route("api/[controller]")] // URL จะเป็น: /api/product
    public class ProductController : ControllerBase
    {
        [HttpGet]
        public IActionResult GetAll()
        {
            var products = new[]
            {
                new { Id = 1, Name = "Keyboard", Price = 599 },
                new { Id = 2, Name = "Mouse", Price = 399 }
            };

            return Ok(products); // ส่ง JSON กลับ
        }

        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            return Ok(new { Id = id, Name = "Sample", Price = 100 });
        }

        [HttpPost]
        public IActionResult Create([FromBody] ProductDto input)
        {
            return Ok(new { Message = "Created", Data = input });
        }
    }

    public class ProductDto()
    {
        public required string Name { get; set; }
        public decimal Price { get; set; }


    }
}
