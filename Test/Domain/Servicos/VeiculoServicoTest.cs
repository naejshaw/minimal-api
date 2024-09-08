using MinimalApi.Dominio.Entidades;
using MinimalApi.Dominio.Servicos;
using MinimalApi.Infraestrutura.Db;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Reflection;

namespace Test.Domain.Servicos;

[TestClass]
public class VeiculoServicoTest
{
    private DbContexto CriarContextoDeTeste(){
        var assemblyPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        var path = Path.GetFullPath(Path.Combine(assemblyPath ?? "", "..", "..", ".."));

        var builder = new ConfigurationBuilder()
            .SetBasePath(path ?? Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddEnvironmentVariables();

        var configuration =builder.Build();

        return new DbContexto(configuration);
    }

    [TestMethod]
    public void TestandoSalvarVeiculo()
    {
        // Arrange
        var context = CriarContextoDeTeste();
        context.Database.ExecuteSqlRaw("TRUNCATE TABLE Veiculos");

        var veiculo = new Veiculo();
        veiculo.Nome = "Uno";
        veiculo.Marca = "Fiat";
        veiculo.Ano = 1999;

        var veiculoServico = new VeiculoServico(context);

        // Act
        veiculoServico.Incluir(veiculo);

        // Assert
        Assert.AreEqual(1, veiculoServico.Todos(1).Count());
    }

    [TestMethod]
    public void TestandoApagarVeiculo()
    {
        // Arrange
        var context = CriarContextoDeTeste();
        context.Database.ExecuteSqlRaw("TRUNCATE TABLE Veiculos");

        var veiculo = new Veiculo { Nome = "Uno", Marca = "Fiat", Ano = 1999 };
        context.Veiculos.Add(veiculo);
        context.SaveChanges();

        var veiculoServico = new VeiculoServico(context);

        // Act
        veiculoServico.Apagar(veiculo);

        // Assert
        var veiculoApagado = veiculoServico.BuscaPorId(veiculo.Id);
        Assert.IsNull(veiculoApagado);
    }

    [TestMethod]
    public void TestandoAtualizarVeiculo()
    {
        // Arrange
        var context = CriarContextoDeTeste();
        context.Database.ExecuteSqlRaw("TRUNCATE TABLE Veiculos");

        var veiculo = new Veiculo { Nome = "Uno", Marca = "Fiat", Ano = 1999 };
        context.Veiculos.Add(veiculo);
        context.SaveChanges();

        var veiculoServico = new VeiculoServico(context);

        // Act
        veiculo.Nome = "Palio";
        veiculoServico.Atualizar(veiculo);

        // Assert
        var veiculoAtualizado = veiculoServico.BuscaPorId(veiculo.Id);
        Assert.AreEqual("Palio", veiculoAtualizado?.Nome);
    }
    
    [TestMethod]
    public void TestandoBuscaPorId()
    {
        // Arrange
        var context = CriarContextoDeTeste();
        context.Database.ExecuteSqlRaw("TRUNCATE TABLE Veiculos");

        var veiculo = new Veiculo();
        veiculo.Nome = "Uno";
        veiculo.Marca = "Fiat";
        veiculo.Ano = 1999;

        var veiculoServico = new VeiculoServico(context);

        // Act
        veiculoServico.Incluir(veiculo);
        var veiculoDoBanco = veiculoServico.BuscaPorId(veiculo.Id);

        // Assert
        Assert.AreEqual(1, veiculoDoBanco?.Id);
    }

    [TestMethod]
    public void TestandoBuscaTodosVeiculos()
    {
        // Arrange
        var context = CriarContextoDeTeste();
        context.Database.ExecuteSqlRaw("TRUNCATE TABLE Veiculos");

        var veiculos = new List<Veiculo>()
        {
            new Veiculo { Nome = "Uno", Marca = "Fiat", Ano = 1999 },
            new Veiculo { Nome = "K", Marca = "Ford", Ano = 2010 },
            new Veiculo { Nome = "Clio", Marca = "Renault", Ano = 2008 },
        };

        veiculos.ForEach(veiculo => context.Veiculos.Add(veiculo));
        context.SaveChanges();

        var veiculoServico = new VeiculoServico(context);

        // Act
        var listaVeiculos = veiculoServico.Todos(null); 

        // Assert
        Assert.AreEqual(3, listaVeiculos.Count); 
    }

    [TestMethod]
    public void TestandoBuscaTodosVeiculosPaginado()
    {
        // Arrange
        var context = CriarContextoDeTeste();
        context.Database.ExecuteSqlRaw("TRUNCATE TABLE Veiculos");

        var veiculos = new List<Veiculo>()
        {
            new Veiculo { Nome = "Uno", Marca = "Fiat", Ano = 1999 },
            new Veiculo { Nome = "K", Marca = "Ford", Ano = 2010 },
            new Veiculo { Nome = "Clio", Marca = "Renault", Ano = 2008 },
            new Veiculo { Nome = "Sandero", Marca = "Reanult", Ano = 1996 },
            new Veiculo { Nome = "Focus", Marca = "Ford", Ano = 2020 },
        };

        veiculos.ForEach(veiculo => context.Veiculos.Add(veiculo));
        context.SaveChanges();

        var veiculoServico = new VeiculoServico(context);

        // Act
        var pagina1 = veiculoServico.Todos(1); 
        var pagina2 = veiculoServico.Todos(2);

        int itemsPorPagina = 3;
        // Assert
        Assert.AreEqual(3, pagina1.Count); 
        Assert.AreEqual(2, pagina2.Count);
        Assert.AreEqual(veiculos[0].Nome, pagina1[0].Nome);
        Assert.AreEqual(veiculos[1].Nome, pagina1[1].Nome);
        Assert.AreEqual(veiculos[2].Nome, pagina1[2].Nome);
        Assert.AreEqual(veiculos[3].Nome, pagina2[0].Nome);
        Assert.AreEqual(veiculos[4].Nome, pagina2[1].Nome);
    }
}
