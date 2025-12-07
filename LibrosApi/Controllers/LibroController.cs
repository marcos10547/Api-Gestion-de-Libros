using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/libros")]
public class LibroController : ControllerBase
{
    private readonly ILibroService _libroService;

    public LibroController(ILibroService libroService)
    {
        _libroService = libroService;
    }

    [HttpGet]
    [ProducesResponseType(200, Type = typeof(IEnumerable<LibroResponseDTO>))]
    public IActionResult GetLibros([FromQuery] LibroQueryParams parametros)
    {
        // Si no hay parámetros, devuelve todos. Si hay, filtra.
        // El repositorio maneja la lógica si los campos del QueryParams son nulos.
        var libros = _libroService.GetLibros(parametros);
        
        if (libros == null || !libros.Any())
        {
            return NotFound();
        }
        return Ok(libros);
    }

    [HttpGet("{isbn}")]
    [ProducesResponseType(200, Type = typeof(LibroResponseDTO))]
    [ProducesResponseType(404)]
    public IActionResult GetLibro(string isbn)
    {
        var libro = _libroService.GetLibroByISBN(isbn);
        if (libro == null)
        {
            return NotFound();
        }
        return Ok(libro);
    }

    [HttpPost]
    [ProducesResponseType(201, Type = typeof(LibroResponseDTO))]
    [ProducesResponseType(400)]
    public IActionResult CreateLibro([FromBody] LibroCreateDTO dto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var creado = _libroService.CreateLibro(dto);
        return CreatedAtAction(nameof(GetLibro), new { isbn = creado.ISBN }, creado);
    }

    [HttpPut("{isbn}")]
    [ProducesResponseType(204)]
    [ProducesResponseType(404)]
    [ProducesResponseType(400)]
    public IActionResult UpdateLibro(string isbn, [FromBody] LibroCreateDTO dto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var resultado = _libroService.UpdateLibro(isbn, dto);
        if (!resultado)
        {
            return NotFound();
        }
        return NoContent();
    }

    [HttpDelete("{isbn}")]
    [ProducesResponseType(204)]
    [ProducesResponseType(404)]
    public IActionResult DeleteLibro(string isbn)
    {
        var resultado = _libroService.DeleteLibro(isbn);
        if (!resultado)
        {
            return NotFound();
        }
        return NoContent();
    }
}