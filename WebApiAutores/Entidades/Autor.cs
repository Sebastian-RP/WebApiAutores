﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WebApiAutores.Validaciones;

namespace WebApiAutores.Entidades
{
    public class Autor
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "El campo {0} es requerido")]
        [StringLength(maximumLength: 99, ErrorMessage = "El campo {0} debe tener maximo 99 caracteres")]
        [PrimeraLetraMayuscula]
        public string? Nombre { get; set; }
        public List<AutorLibro>? AutoresLibros { get; set; }
    }
}
