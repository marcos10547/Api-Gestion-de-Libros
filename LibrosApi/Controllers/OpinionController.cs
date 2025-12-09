using Microsoft.AspNetCore.Mvc;


[ApiController]
[Route("api/opiniones")] 
public class OpinionController : ControllerBase
{
    private readonly IOpinionService _reseñaService;

    public OpinionController(IOpinionService reseñaService)
    {
        _reseñaService = reseñaService;
    }

    [HttpGet]
    public IActionResult GetAll()
    {
        return Ok(_reseñaService.GetAllReseñas());
    }

    [HttpPost]
    public IActionResult Create([FromBody] OpinionCreateDTO dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        var creada = _reseñaService.CreateReseña(dto);
        return CreatedAtAction(nameof(GetById), new { id = creada.ReseñaId }, creada);
    }

    [HttpGet("{id:int}")]
    public IActionResult GetById(int id)
    {
        var r = _reseñaService.GetReseñaById(id);
        return r != null ? Ok(r) : NotFound();
    }

    [HttpPut("{id:int}")]
    public IActionResult Update(int id, [FromBody] OpinionCreateDTO dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        return _reseñaService.UpdateReseña(id, dto) ? NoContent() : NotFound();
    }
    [HttpDelete("{id:int}")]
    public IActionResult Delete(int id)
    {
        return _reseñaService.DeleteReseña(id) ? NoContent() : NotFound();
    }


    [HttpGet("/api/libros/{isbn}/estadisticas")]
    public IActionResult GetEstadisticas(string isbn)
    {
        var stats = _reseñaService.GetEstadisticas(isbn);
        return Ok(stats);
    }

    [HttpGet("/api/libros/{isbn}/opiniones")]
    public IActionResult GetByLibroFiltradas(
        string isbn,  
        [FromQuery] DateTime? minFecha
        )
    {
        var resenas = _reseñaService.GetReseñasFiltradas(isbn, minFecha);
        return Ok(resenas);
    }
}