using MinimalApi.Dominio.Entidades;
using MinimalApi.Dominio.Servicos;
using MinimalApi.Infraestrutura.Db;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Reflection;

namespace Test.Domain.Servicos;

[TestClass]
public class AdministradorServicoTest
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
    public void TestandoSalvarAdministrador()
    {
        // Arrange
        var context = CriarContextoDeTeste();
        context.Database.ExecuteSqlRaw("TRUNCATE TABLE Administradores");

        var adm = new Administrador();
        adm.Email = "joao@test.com";
        adm.Senha = "joaotest";
        adm.Perfil = "Adm";

        var administradorServico = new AdministradorServico(context);

        // Act
        administradorServico.Incluir(adm);

        // Assert
        Assert.AreEqual(1, administradorServico.Todos(1).Count());
    }

    [TestMethod]
    public void TestandoBuscaPorId()
    {
        // Arrange
        var context = CriarContextoDeTeste();
        context.Database.ExecuteSqlRaw("TRUNCATE TABLE Administradores");

        var adm = new Administrador();
        adm.Email = "joao@test.com";
        adm.Senha = "joaotest";
        adm.Perfil = "Adm";

        var administradorServico = new AdministradorServico(context);

        // Act
        administradorServico.Incluir(adm);
        var admDoBanco = administradorServico.BuscaPorId(adm.Id);

        // Assert
        Assert.AreEqual(1, admDoBanco?.Id);
    }

    [TestMethod]
    public void TestandoBuscaTodosAdministradores()
    {
        // Arrange
        var context = CriarContextoDeTeste();
        context.Database.ExecuteSqlRaw("TRUNCATE TABLE Administradores");

        var administradores = new List<Administrador>()
        {
            new Administrador { Email = "joao@test.com", Senha = "joaotest", Perfil = "Adm" },
            new Administrador { Email = "maria@test.com", Senha = "mariatest", Perfil = "Editor" },
            new Administrador { Email = "pedro@test.com", Senha = "pedrotest", Perfil = "Editor" },
        };

        administradores.ForEach(adm => context.Administradores.Add(adm));
        context.SaveChanges();

        var administradorServico = new AdministradorServico(context);

        // Act
        var listaAdministradores = administradorServico.Todos(null); 

        // Assert
        Assert.AreEqual(3, listaAdministradores.Count); 
    }

    [TestMethod]
    public void TestandoBuscaTodosAdministradoresPaginado()
    {
        // Arrange
        var context = CriarContextoDeTeste();
        context.Database.ExecuteSqlRaw("TRUNCATE TABLE Administradores");

        var administradores = new List<Administrador>()
        {
            new Administrador { Email = "joao@test.com", Senha = "joaotest", Perfil = "Adm" },
            new Administrador { Email = "maria@test.com", Senha = "mariatest", Perfil = "Editor" },
            new Administrador { Email = "pedro@test.com", Senha = "pedrotest", Perfil = "Editor" },
            new Administrador { Email = "ana@test.com", Senha = "anatest", Perfil = "Adm" },
            new Administrador { Email = "carlos@test.com", Senha = "carlostest", Perfil = "Editor" },
        };

        administradores.ForEach(adm => context.Administradores.Add(adm));
        context.SaveChanges();

        var administradorServico = new AdministradorServico(context);

        // Act
        var pagina1 = administradorServico.Todos(1); 
        var pagina2 = administradorServico.Todos(2);

        int itemsPorPagina = 3;
        // Assert
        Assert.AreEqual(3, pagina1.Count); 
        Assert.AreEqual(2, pagina2.Count);
        Assert.AreEqual(administradores[0].Email, pagina1[0].Email);
        Assert.AreEqual(administradores[1].Email, pagina1[1].Email);
        Assert.AreEqual(administradores[2].Email, pagina1[2].Email);
        Assert.AreEqual(administradores[3].Email, pagina2[0].Email);
        Assert.AreEqual(administradores[4].Email, pagina2[1].Email);
    }
}
