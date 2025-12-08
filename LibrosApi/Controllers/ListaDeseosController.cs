using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/listasdeseos")]
public class ListaDeseosController : ControllerBase
{
    private readonly IListaDeseosService _service;

    public ListaDeseosController(IListaDeseosService service)
    {
        _service = service;
    }

    // GET: /api/listasdeseos/5
    [HttpGet("{id:int}")]
    [ProducesResponseType(200, Type = typeof(ListaDeseosResponseDTO))]
    [ProducesResponseType(404)]
    public IActionResult GetById(int id)
    {
        var lista = _service.GetById(id);
        if (lista == null) return NotFound();
        return Ok(lista);
    }

    // POST: /api/listasdeseos
    [HttpPost]
    [ProducesResponseType(201, Type = typeof(ListaDeseosResponseDTO))]
    [ProducesResponseType(400)]
    public IActionResult Create([FromBody] ListaDeseosCreateDTO dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var creada = _service.Create(dto);
        return CreatedAtAction(nameof(GetById), new { id = creada.ListaId }, creada);
    }

    // DELETE: /api/listasdeseos/5
    [HttpDelete("{id:int}")]
    [ProducesResponseType(204)]
    [ProducesResponseType(404)]
    public IActionResult Delete(int id)
    {
        if (!_service.Delete(id)) return NotFound();
        return NoContent();
    }

    
    // GET: /api/clientes/{clienteId}/listasdeseos
    [HttpGet("/api/clientes/{clienteId}/listasdeseos")]
    [ProducesResponseType(200, Type = typeof(IEnumerable<ListaDeseosResponseDTO>))]
    public IActionResult GetByCliente(int clienteId)
    {
        var listas = _service.GetByCliente(clienteId);
        return Ok(listas);
    }


    // POST: /api/listasdeseos/1/libros
    // Body: { "libroISBN": "978-01" }
    [HttpPost("{id:int}/libros")]
    [ProducesResponseType(200)]
    [ProducesResponseType(400)]
    public IActionResult AddLibro(int id, [FromBody] ListaDeseosAddLibroDTO dto)
    {
        var resultado = _service.AddLibroToLista(id, dto.LibroISBN);
        
        if (!resultado)
        {
            return BadRequest("No se pudo añadir el libro. Verifique que el libro existe y no esté ya en la lista.");
        }
        return Ok("Libro añadido correctamente.");
    }

    // DELETE: /api/listasdeseos/1/libros/978-01
    [HttpDelete("{id:int}/libros/{isbn}")]
    [ProducesResponseType(204)]
    [ProducesResponseType(404)]
    public IActionResult RemoveLibro(int id, string isbn)
    {
        var resultado = _service.RemoveLibroFromLista(id, isbn);
        if (!resultado) return NotFound("El libro no estaba en la lista.");
        return NoContent();
    }
}