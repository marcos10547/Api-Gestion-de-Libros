using Microsoft.AspNetCore.Mvc;
using System.Net;

[ApiController]
[Route("api/generos")]
public class GeneroController : ControllerBase
{
    private readonly IGeneroService _generoService;

    public GeneroController(IGeneroService generoService)
    {
        _generoService = generoService;
    }

    [HttpGet]
    [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(IEnumerable<GeneroResponseDTO>))]
    public IActionResult GetAllGeneros()
    {
        var generos = _generoService.GetAllGeneros();
        
        if (generos == null || !generos.Any())
        {
            return NotFound();
        }
        
        return Ok(generos);
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(GeneroResponseDTO))]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    public IActionResult GetGenero(int id)
    {
        var genero = _generoService.GetGeneroById(id);

        if (genero == null)
        {
            return NotFound();
        }

        return Ok(genero);
    }

    [HttpPost]
    [ProducesResponseType((int)HttpStatusCode.Created, Type = typeof(GeneroResponseDTO))]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    public IActionResult CreateGenero([FromBody] GeneroCreateDTO dto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState); 
        }

        var nuevoGenero = _generoService.CreateGenero(dto);
        
        return CreatedAtAction(nameof(GetGenero), new { id = nuevoGenero.GeneroId }, nuevoGenero);
    }
    
    [HttpPut("{id:int}")]
    [ProducesResponseType((int)HttpStatusCode.NoContent)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    public IActionResult UpdateGenero(int id, [FromBody] GeneroCreateDTO dto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var resultado = _generoService.UpdateGenero(id, dto);

        if (!resultado)
        {
            return NotFound();
        }

        return NoContent();
    }

    [HttpDelete("{id:int}")]
    [ProducesResponseType((int)HttpStatusCode.NoContent)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    public IActionResult DeleteGenero(int id)
    {
        var resultado = _generoService.DeleteGenero(id);

        if (!resultado)
        {
            return NotFound();
        }

        return NoContent();
    }
}