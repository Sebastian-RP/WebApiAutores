using System.ComponentModel.DataAnnotations;
using WebApiAutores.Validaciones;

namespace WebApiAutores.DTOs
{
    public class LibroCreacionDTO
    {
        //DTO es agregar una capa extra donde este sera el objeto que se entregara al controlador en vez de la entidad directamentes
        [PrimeraLetraMayuscula]
        public string? Titulo { get; set; }
    }
}
