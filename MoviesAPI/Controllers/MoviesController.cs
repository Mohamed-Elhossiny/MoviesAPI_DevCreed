using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MoviesAPI.Models;

namespace MoviesAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MoviesController : ControllerBase
    {
        private readonly ApplicationDbContext context;

        private new List<string> allowedExtensions = new List<string>
        {
            ".jpg",".png"
        };
        private long maxAllowedPosterSize = 1048576;

        public MoviesController(ApplicationDbContext context)
        {
            this.context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllAsync()
        {
            var allMovies = await context.Movies
                .OrderByDescending(m => m.Rate)
                .Include(n => n.Genre)
                .Select(m => new MovieDetailsDTO
            {
                Id = m.Id,
                GenreId = m.GenreId,
                GenreName = m.Genre.Name,
                Poster = m.Poster,
                Rate = m.Rate,
                Title = m.Title,
                StoreLine = m.StoreLine,
                Year = m.Year
            }).ToListAsync();
            return Ok(allMovies);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var movie = await context.Movies.Include(m => m.Genre).SingleOrDefaultAsync(m => m.Id == id);
            if (movie == null)
                return NotFound();
            var dto = new MovieDetailsDTO
            {
                Id = movie.Id,
                GenreId = movie.GenreId,
                GenreName = movie.Genre.Name,
                Poster = movie.Poster,
                Rate = movie.Rate,
                Title = movie.Title,
                StoreLine = movie.StoreLine,
                Year = movie.Year
            };
            return Ok(dto);
        }

        [HttpGet("GetByGenreId")]
        public async Task<IActionResult> GetByGenreIdAsync(byte id)
        {
            var allMovies = await context.Movies
                .Where(m=>m.GenreId==id)
                .OrderByDescending(m => m.Rate)
                .Include(n => n.Genre)
                .Select(m => new MovieDetailsDTO
            {
                Id = m.Id,
                GenreId = m.GenreId,
                GenreName = m.Genre.Name,
                Poster = m.Poster,
                Rate = m.Rate,
                Title = m.Title,
                StoreLine = m.StoreLine,
                Year = m.Year
            }).ToListAsync();
            return Ok(allMovies);
        }


        [HttpPost]
        public async Task<IActionResult> CreateAsync([FromForm] MovieDTO dtoModel)
        {
            // How to Store File Formate in Array of Bytes to store in Database
            if (!allowedExtensions.Contains(Path.GetExtension(dtoModel.Poster.FileName).ToLower()))
                return BadRequest("Only .png and .jpg images are allowed");
            if (dtoModel.Poster.Length > maxAllowedPosterSize)
                return BadRequest("Max allowed size i 1 MP");
            var isvalidGener = await context.Genres.AnyAsync(g => g.Id == dtoModel.GenreId);
            if (!isvalidGener)
                return BadRequest("Invalid Genre ID");
            using var dataStream = new MemoryStream();
            await dtoModel.Poster.CopyToAsync(dataStream);

            var movie = new Movie
            {
                GenreId = dtoModel.GenreId,
                Poster = dataStream.ToArray(),
                Rate = dtoModel.Rate,
                StoreLine = dtoModel.StoreLine,
                Title = dtoModel.Title,
                Year = dtoModel.Year,
            };
            await context.AddAsync(movie);
            context.SaveChanges();
            return Ok(movie);
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteAsync(int id)
        {
            var movie = await context.Movies.FindAsync(id);
            if (movie == null)
                return NotFound($"No Movie Found by ID {id}");
            context.Remove(movie);
            context.SaveChanges();
            return Ok(movie);
        }
    }
}
