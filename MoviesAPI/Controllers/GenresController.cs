using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


namespace MoviesAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GenresController : ControllerBase
    {
        private readonly ApplicationDbContext context;

        public GenresController(ApplicationDbContext context)
        {
            this.context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllAsync()
        {
            var genres = await context.Genres.OrderBy(n => n.Name).ToListAsync();
            return Ok(genres);
        }

        [HttpPost]
        public async Task<IActionResult> CreateAsync(GenreDTO dtoModel)
        {
            var genre = new Genre { Name = dtoModel.Name };

            await context.Genres.AddAsync(genre);
            context.SaveChangesAsync();

            return Ok(genre);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateAsync(int id,[FromBody] GenreDTO dtoModel)
        {
            var genre = await context.Genres.SingleOrDefaultAsync(n => n.Id == id);
            if(genre==null)
            {
                return NotFound($"No Genre was found by Id {id}");
            }
            genre.Name = dtoModel.Name;
            // all Properties will be sent to the database so we need to send also the Id
            // context.Entry(genre).State = EntityState.Modified;
            context.SaveChanges();
            return Ok(genre);
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteAsync(int id)
        {
            var genre = await context.Genres.SingleOrDefaultAsync(n => n.Id == id);
            if (genre == null)
            {
                return NotFound($"No Genre was found by Id {id}");
            }
            context.Genres.Remove(genre);
            context.SaveChanges();
            return Ok(genre);
        }
    }
}
