using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Net;
using System.Linq;

[ApiController]
[Route("api/autores")]
public class AutorController : ControllerBase
{
    private readonly IAutorService _autorService;

    public AutorController(IAutorService autorService)
    {
        _autorService = autorService;
    }

    [HttpGet]
    [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(IEnumerable<AutorResponseDTO>))]
    public IActionResult GetAllAutores()
    {
        var autores = _autorService.GetAllAutores();
        
        if (autores == null || !autores.Any())
        {
            return NotFound();
        }
        
        return Ok(autores);
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(AutorResponseDTO))]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    public IActionResult GetAutor(int id)
    {
        var autor = _autorService.GetAutorById(id);

        if (autor == null)
        {
            return NotFound();
        }

        return Ok(autor);
    }

    [HttpPost]
    [ProducesResponseType((int)HttpStatusCode.Created, Type = typeof(AutorResponseDTO))]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    public IActionResult CreateAutor([FromBody] AutorCreateDTO dto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState); 
        }

        var nuevoAutor = _autorService.CreateAutor(dto);

        if (nuevoAutor == null)
        {
             return BadRequest();
        }
        
        return CreatedAtAction(nameof(GetAutor), new { id = nuevoAutor.AutorId }, nuevoAutor);
    }
    
    [HttpPut("{id:int}")]
    [ProducesResponseType((int)HttpStatusCode.NoContent)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    public IActionResult UpdateAutor(int id, [FromBody] AutorCreateDTO dto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var resultado = _autorService.UpdateAutor(id, dto);

        if (!resultado)
        {
            return NotFound();
        }

        return NoContent();
    }

    [HttpDelete("{id:int}")]
    [ProducesResponseType((int)HttpStatusCode.NoContent)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    public IActionResult DeleteAutor(int id)
    {
        var resultado = _autorService.DeleteAutor(id);

        if (!resultado)
        {
            return NotFound();
        }

        return NoContent();
    }
}