using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/resenas")] 
public class ReseñaController : ControllerBase
{
    private readonly IReseñaService _reseñaService;

    public ReseñaController(IReseñaService reseñaService)
    {
        _reseñaService = reseñaService;
    }


    [HttpGet]
    [ProducesResponseType(200, Type = typeof(IEnumerable<ReseñaResponseDTO>))]
    public IActionResult GetAll()
    {
        var resenas = _reseñaService.GetAllReseñas();
        return Ok(resenas);
    }

    [HttpPost]
    [ProducesResponseType(201, Type = typeof(ReseñaResponseDTO))]
    [ProducesResponseType(400)]
    public IActionResult Create([FromBody] ReseñaCreateDTO dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        
        var creada = _reseñaService.CreateReseña(dto);
        return CreatedAtAction(nameof(GetById), new { id = creada.ReseñaId }, creada);
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(200, Type = typeof(ReseñaResponseDTO))]
    [ProducesResponseType(404)]
    public IActionResult GetById(int id)
    {
        var resena = _reseñaService.GetReseñaById(id);
        if (resena == null) return NotFound();
        return Ok(resena);
    }

    [HttpGet("/api/clientes/{clienteId}/resenas")]
    [ProducesResponseType(200, Type = typeof(IEnumerable<ReseñaResponseDTO>))]
    public IActionResult GetByCliente(int clienteId)
    {
        var resenas = _reseñaService.GetReseñasByCliente(clienteId);
        
        return Ok(resenas);
    }

    [HttpGet("/api/libros/{isbn}/resenas")]
    [ProducesResponseType(200, Type = typeof(IEnumerable<ReseñaResponseDTO>))]
    public IActionResult GetByLibro(string isbn)
    {
        var resenas = _reseñaService.GetReseñasByLibro(isbn);
        return Ok(resenas);
    }
}