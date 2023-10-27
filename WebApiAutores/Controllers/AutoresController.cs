using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApiAutores.Entidades;

namespace WebApiAutores.Controllers
{
   
    [ApiController]
    [Route("api/autores")]
    public class AutoresController : ControllerBase
    {
        private readonly ApplicationDbContext context;
        public AutoresController(ApplicationDbContext context) 
        {
            this.context = context;
        }

         
        [HttpGet] //api/autores
        [HttpGet("listado")] //api/autores/listado
        [HttpGet("/listado")] //listado
        public async Task<List<Autor>>  Get()
        {
            return await context.Autores.Include(x => x.Libros).ToListAsync();
        }
         
        [HttpGet("{id:int}/{param2 = persona}")] //al poner el signo "?" hago ese parametro opcional /{param2?} o darle valor por defecto por ejemplo "persona"
        public async Task<ActionResult<Autor>> Get(int id, string param2)
        {
            var autor = await context.Autores.FirstOrDefaultAsync(x => x.Id == id);

            if(autor is null)
            {
                return NotFound();
            }

            return autor;
        }

        [HttpGet("{nombre}")]
        public async Task<ActionResult<Autor>> Get(string nombre)
        {
            var autor = await context.Autores.FirstOrDefaultAsync(x => x.Nombre.Contains(nombre));

            if (autor is null)
            {
                return NotFound();
            }

            return autor;
        }

        [HttpGet("primero")]
        public async Task<ActionResult<Autor>> Primerautor()
        {
            return await context.Autores.FirstOrDefaultAsync();
        }

        [HttpPost]
        public async Task<ActionResult> Post(Autor autor)
        {
            context.Add(autor);
            await context.SaveChangesAsync();
            return Ok();
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult> Put(Autor autor, int id)
        {
            if(autor.Id != id)
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

            if(!existe)
            {
                return NotFound();
            }
             
            context.Remove(new Autor() { Id = id });
            await context.SaveChangesAsync();
            return Ok();
        }
    }
}
