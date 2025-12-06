using System.Collections.Generic;
using System.Linq;
using System;

public class ClienteService : IClienteService
{
    private readonly IClienteRepository _clienteRepository;

    public ClienteService(IClienteRepository clienteRepository)
    {
        _clienteRepository = clienteRepository;
    }
    
    private ClienteResponseDTO MapToResponseDTO(Cliente cliente)
    {
        if (cliente == null) return null;
        
        return new ClienteResponseDTO
        {
            ClienteId = cliente.ClienteId,
            Email = cliente.Email,
            NombreUsuario = cliente.NombreUsuario,
            GastoTotal = cliente.GastoTotal,
            FechaRegistro = cliente.FechaRegistro,
            EsPremium = cliente.EsPremium,
            Nivel = cliente.Nivel
        };
    }

    public IEnumerable<ClienteResponseDTO> GetAllClientes()
    {
        var clientes = _clienteRepository.GetAll();
        
        return clientes.Select(MapToResponseDTO);
    }

    public ClienteResponseDTO GetClienteById(int id)
    {
        var cliente = _clienteRepository.GetById(id);
        
        return MapToResponseDTO(cliente);
    }

    public ClienteResponseDTO CreateCliente(ClienteCreateDTO dto)
    {
        var nuevoCliente = new Cliente
        {
            Email = dto.Email,
            NombreUsuario = dto.NombreUsuario,
            EsPremium = dto.EsPremium,
            
            // Valores predeterminados en el servicio
            GastoTotal = 0.00m,
            FechaRegistro = DateTime.UtcNow,
            Nivel = 1 
        };

        var clienteGuardado = _clienteRepository.Create(nuevoCliente);
        
        return MapToResponseDTO(clienteGuardado);
    }
    
    public bool UpdateCliente(int id, ClienteCreateDTO dto)
    {
        var clienteExistente = _clienteRepository.GetById(id);
        if (clienteExistente == null) return false;

        clienteExistente.Email = dto.Email;
        clienteExistente.NombreUsuario = dto.NombreUsuario;
        clienteExistente.EsPremium = dto.EsPremium;
        
        // No actualizamos GastoTotal ni FechaRegistro desde el DTO de creación
        
        return _clienteRepository.Update(clienteExistente);
    }

    public bool DeleteCliente(int id)
    {
        // Nota: Implementar lógica de negocio para borrar recursos asociados (Reseñas, ListasDeseos)
        return _clienteRepository.Delete(id);
    }
}