using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Net;
using System.Linq;

[ApiController]
[Route("api/clientes")]
public class ClienteController : ControllerBase
{
    private readonly IClienteService _clienteService;

    public ClienteController(IClienteService clienteService)
    {
        _clienteService = clienteService;
    }

    [HttpGet]
    [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(IEnumerable<ClienteResponseDTO>))]
    public IActionResult GetAllClientes()
    {
        // En un escenario real, esta ruta solo sería accesible por administradores
        var clientes = _clienteService.GetAllClientes();
        
        if (clientes == null || !clientes.Any())
        {
            return NotFound();
        }
        
        return Ok(clientes);
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(ClienteResponseDTO))]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    public IActionResult GetCliente(int id)
    {
        var cliente = _clienteService.GetClienteById(id);

        if (cliente == null)
        {
            return NotFound();
        }

        return Ok(cliente);
    }

    [HttpPost]
    [ProducesResponseType((int)HttpStatusCode.Created, Type = typeof(ClienteResponseDTO))]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    public IActionResult CreateCliente([FromBody] ClienteCreateDTO dto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState); 
        }

        var nuevoCliente = _clienteService.CreateCliente(dto);

        if (nuevoCliente == null)
        {
             return BadRequest();
        }
        
        return CreatedAtAction(nameof(GetCliente), new { id = nuevoCliente.ClienteId }, nuevoCliente);
    }
    
    [HttpPut("{id:int}")]
    [ProducesResponseType((int)HttpStatusCode.NoContent)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    public IActionResult UpdateCliente(int id, [FromBody] ClienteCreateDTO dto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var resultado = _clienteService.UpdateCliente(id, dto);

        if (!resultado)
        {
            return NotFound();
        }

        return NoContent();
    }

    [HttpDelete("{id:int}")]
    [ProducesResponseType((int)HttpStatusCode.NoContent)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    public IActionResult DeleteCliente(int id)
    {
        var resultado = _clienteService.DeleteCliente(id);

        if (!resultado)
        {
            return NotFound();
        }

        return NoContent();
    }
}