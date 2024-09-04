using MinimalApi.DTOs;
using MinimalApi.Dominio.Entidades;

namespace MinimalApi.Dominio.Interfaces;

public interface IVeiculoServico
{
    List<Veiculo>? Todos(int pagina = 1, string? nome = null, string? marca = null);
    void Apagar(Veiculo veiculo);
    void Atualizar(Veiculo veiculo);
    Veiculo? BuscaPorId(int id);
    void Incluir(Veiculo veiculo);

}