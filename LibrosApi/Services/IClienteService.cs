using System.Collections.Generic;

public interface IClienteService
{
    ClienteResponseDTO GetClienteById(int id);
    IEnumerable<ClienteResponseDTO> GetAllClientes();
    ClienteResponseDTO CreateCliente(ClienteCreateDTO dto);
    bool UpdateCliente(int id, ClienteCreateDTO dto);
    bool DeleteCliente(int id);
}