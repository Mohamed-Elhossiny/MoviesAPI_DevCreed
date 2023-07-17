namespace MoviesAPI.DTOS
{
    public class GenreDTO
    {
        [MaxLength(100)]
        public string Name { get; set; }
    }
}
