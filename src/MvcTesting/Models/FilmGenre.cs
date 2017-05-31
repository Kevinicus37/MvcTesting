namespace MvcTesting.Models
{
    public class FilmGenre
    {
        public int FilmID { get; set; }
        public Film Film { get; set; }

        public int GenreID { get; set; }
        public Genre Genre { get; set; }
    }
}
