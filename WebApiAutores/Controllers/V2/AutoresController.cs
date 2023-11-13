using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApiAutores.DTOs;
using WebApiAutores.Entidades;
using System.Linq;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;

namespace WebApiAutores.Controllers.V2
{

    [ApiController]
    [Route("api/v2/autores")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "EsAdmin")]
    public class AutoresController : ControllerBase
    {
        private readonly ApplicationDbContext context;
        private readonly IMapper mapper;
        private readonly IAuthorizationService authorizationService;

        //principio de inversion de dependencias(relacionado a inyeccion de dependencias), "nuestras clases deberian depender de abstracciones y no de tipos concretos"
        //razon por la cual se le pasa una interfaz y no el tipo en concreto 
        // ej mal: AutoresController(ApplicationDbContext context, ServicioA servicio) 
        // ej bien: AutoresController(ApplicationDbContext context, IServicios servicio) 
        public AutoresController(
            ApplicationDbContext context,
            IMapper mapper,
            IAuthorizationService authorizationService
            )
        {
            this.context = context;
            this.mapper = mapper;
            this.authorizationService = authorizationService;
        }

        [HttpGet(Name = "obtenerAutoresv2")] //api/autores
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "EsAdmin")]
        [AllowAnonymous] //no solicita token
        public async Task<IActionResult> Get([FromBody] bool incluirHATEAOS = true)
        {
            var autores = await context.Autores.ToListAsync();
            autores.ForEach(autor => autor.Nombre = autor.Nombre.ToUpper());

            var dtos = mapper.Map<List<AutorDTO>>(autores);
            var esAdmin = await authorizationService.AuthorizeAsync(User, "esAdmin");

            if (incluirHATEAOS)
            {
                dtos.ForEach(dtos => GenerarEnlaces(dtos, esAdmin.Succeeded));

                var resultado = new ColeccionDeRecursos<AutorDTO> { Valores = dtos };
                resultado.Enlaces.Add(new DatoHATEOAS(
                    enlace: Url.Link("obtenerautores", new { }),
                    descripcion: "self",
                    metodo: "GET"));

                if (esAdmin.Succeeded)
                {
                    resultado.Enlaces.Add(new DatoHATEOAS(
                    enlace: Url.Link("crearAutor", new { }),
                    descripcion: "crear-autor",
                    metodo: "POST"));
                }

                return Ok();
            }

            return Ok(dtos);
        }

        //[HttpGet("configuraciones")]
        //public ActionResult<string> ObtenerConfiguracion()
        //{
        //    //prioriza variable de ambiente antes que appSetting - si hay coincidencia de campos toma la ultima modificacion (cambio más reciente)

        //    //viendoe el codigo de IConfiguration variables de ambiente tiene precedencia sobre el userSecret que es json y userSecret tiene precedencia sobre appSetting
        //    //configuration["apellido"];
        //    return configuration["ConnectionStrings:defaultConnection"];
        //}

        [HttpGet("{id:int}", Name = "obtenerAutorv2")]
        [AllowAnonymous]
        public async Task<ActionResult<AutorDTOConLibros>> Get(int id)
        {
            var autor = await context.Autores
                .Include(autorDB => autorDB.AutoresLibros)
                .ThenInclude(autorLibroDB => autorLibroDB.Libro)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (autor is null)
            {
                return NotFound();
            }

            var dto = mapper.Map<AutorDTOConLibros>(autor);
            var esAdmin = await authorizationService.AuthorizeAsync(User, "esAdmin");
            GenerarEnlaces(dto, esAdmin.Succeeded);
            return dto;
        }

        private void GenerarEnlaces(AutorDTO autorDTO, bool esAdmin)
        {
            autorDTO.Enlaces.Add(new DatoHATEOAS(
                enlace: Url.Link("obtenerAutor", new { id = autorDTO.Id }),
                descripcion: "self",
                metodo: "GET"));

            autorDTO.Enlaces.Add(new DatoHATEOAS(
                enlace: Url.Link("actualizarAutor", new { id = autorDTO.Id }),
                descripcion: "autor-actualizar",
                metodo: "PUT"));

            autorDTO.Enlaces.Add(new DatoHATEOAS(
                enlace: Url.Link("borrarrAutor", new { id = autorDTO.Id }),
                descripcion: "self",
                metodo: "DELETE"));
        }

        [HttpGet("{nombre}", Name = "obtenerAutorPorNombrev2")]
        public async Task<ActionResult<List<AutorDTO>>> Get([FromRoute] string nombre)
        {
            var autores = await context.Autores.Where(autorBD => autorBD.Nombre.Contains(nombre)).ToListAsync();

            return mapper.Map<List<AutorDTO>>(autores);
        }

        [HttpGet("primero")]//querystring
        public async Task<ActionResult<Autor>> PrimerAutor([FromHeader] int miValor, [FromQuery] string nombre)
        {
            return await context.Autores.FirstOrDefaultAsync();
        }

        [HttpPost(Name = "crearAutorv2")]
        public async Task<ActionResult> Post([FromBody] AutorCreacionDTO AutorCreacionDTO)
        {
            var existeAutorConElMismoNombre = await context.Autores.AnyAsync(x => x.Nombre == AutorCreacionDTO.Nombre);
            if (existeAutorConElMismoNombre)
            {
                return BadRequest($"Ya existe un autor con el nombre {AutorCreacionDTO.Nombre}");
            }

            var autor = mapper.Map<Autor>(AutorCreacionDTO);

            context.Add(autor);
            await context.SaveChangesAsync();

            var autorDTO = mapper.Map<AutorDTO>(autor);
            return CreatedAtRoute("obtenerAutor", new { id = autor.Id }, autorDTO);
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult> Put(Autor autor, int id)
        {
            if (autor.Id != id)
            {
                return BadRequest("El id del autor no coincide con el id de la URL");
            }

            var existe = await context.Autores.AnyAsync(x => x.Id == id);

            if (existe)
            {
                return NotFound();
            }

            context.Update(autor);
            await context.SaveChangesAsync();
            return Ok();
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult> Delete(int id)
        {
            var existe = await context.Autores.AnyAsync(x => x.Id == id);

            if (!existe)
            {
                return NotFound();
            }

            context.Remove(new Autor() { Id = id });
            await context.SaveChangesAsync();
            return NoContent();
        }
    }
}
