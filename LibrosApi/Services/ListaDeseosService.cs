using System.Collections.Generic;
using System.Linq;
using System;

public class ListaDeseosService : IListaDeseosService
{
    private readonly IListaDeseosRepository _repository;

    public ListaDeseosService(IListaDeseosRepository repository)
    {
        _repository = repository;
    }

    private ListaDeseosResponseDTO MapToDTO(ListaDeseos lista)
    {
        if (lista == null) return null!;
        
        var librosEntities = _repository.GetLibrosInLista(lista.ListaId);

        var librosDTO = librosEntities.Select(l => new LibroResponseDTO
        {
            ISBN = l.ISBN,
            Titulo = l.Titulo,
            PrecioVenta = l.PrecioVenta,
            NumPaginas = l.NumPaginas,
            FechaPublicacion = l.FechaPublicacion,
            EsBestSeller = l.EsBestSeller,
            AutorId = l.AutorId,
            GeneroId = l.GeneroId
        }).ToList();

        return new ListaDeseosResponseDTO
        {
            ListaId = lista.ListaId,
            Nombre = lista.Nombre,
            Descripcion = lista.Descripcion,
            FechaCreacion = lista.FechaCreacion,
            CostoEstimadoTotal = lista.CostoEstimadoTotal,
            EsPublica = lista.EsPublica,
            NumLibros = lista.NumLibros,
            ClienteId = lista.ClienteId,
            Libros = librosDTO 
        };
    }

    public ListaDeseosResponseDTO GetById(int id)
    {
        var lista = _repository.GetById(id);
        return MapToDTO(lista);
    }

    public IEnumerable<ListaDeseosResponseDTO> GetByCliente(int clienteId)
    {
        var listas = _repository.GetByClienteId(clienteId);
        return listas.Select(MapToDTO);
    }

    public ListaDeseosResponseDTO Create(ListaDeseosCreateDTO dto)
    {
        var nuevaLista = new ListaDeseos
        {
            Nombre = dto.Nombre,
            Descripcion = dto.Descripcion,
            EsPublica = dto.EsPublica,
            ClienteId = dto.ClienteId,
            
            FechaCreacion = DateTime.UtcNow,
            CostoEstimadoTotal = 0,
            NumLibros = 0
        };

        var creada = _repository.Create(nuevaLista);
        return MapToDTO(creada);
    }

    public bool Delete(int id)
    {
        return _repository.Delete(id);
    }

    public bool AddLibroToLista(int listaId, string isbn)
    {
        var exito = _repository.AddLibro(listaId, isbn);
        
        if (exito)
        {
            _repository.RecalcularTotales(listaId);
        }
        return exito;
    }

    public bool RemoveLibroFromLista(int listaId, string isbn)
    {
        var exito = _repository.RemoveLibro(listaId, isbn);
        if (exito)
        {
            _repository.RecalcularTotales(listaId);
        }
        return exito;
    }
}