﻿using Microsoft.AspNetCore.Identity;

namespace WebApiAutores.Entidades
{
    public class Comentario
    {
        public int Id { get; set; }
        public string? Contenido { get; set; }
        public int LibroId { get; set; }
        public Libro? Libro { get; set; }
        public string? UsuarioID { get; set; }
        public IdentityUser? User { get; set; } 
    }
}
