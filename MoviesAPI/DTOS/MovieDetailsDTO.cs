﻿using System.ComponentModel.DataAnnotations.Schema;

namespace MoviesAPI.DTOS
{
    public class MovieDetailsDTO
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public int Year { get; set; }
        public double Rate { get; set; }
        public string StoreLine { get; set; }
        public byte[] Poster { get; set; }
        public byte? GenreId { get; set; }
        public string GenreName { get; set; }
    }
}
