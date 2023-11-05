using System.ComponentModel.DataAnnotations;
using WebApiAutores.Validaciones;

namespace WebApiAutores.DTOs
{
    public class AutorCreacionDTO
    {
        //DTO es agregar una capa extra donde este sera el objeto que se entregara al controlador en vez de la entidad directamentes
        [Required(ErrorMessage = "El campo {0} es requerido")]
        [StringLength(maximumLength: 99, ErrorMessage = "El campo {0} debe tener maximo 99 caracteres")]
        [PrimeraLetraMayuscula]
        public string? Nombre { get; set; }
    }
}
