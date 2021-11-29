using NoDapo.Domain;

namespace NoDapo.Utilities
{
    public static class Helpers
    {
        /// <summary>
        /// This converts the given string to respective Genre in the Enum if the string matches,
        /// else Genre.Empty is returned
        /// </summary>
        /// <param name="genre">The string representation of Genre</param>
        /// <returns><see cref="Genre"/></returns>
        public static Genre ToGenre(this string genre)
        {
            return genre.ToLower() switch
            {
                "adventure" => Genre.Adventure,
                "biography" => Genre.Biography,
                "comic" => Genre.Comic,
                "fantasy" => Genre.Fantasy,
                _ => Genre.Empty
            };
        }
    }
}