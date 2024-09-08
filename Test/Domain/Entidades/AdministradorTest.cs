using MinimalApi.Dominio.Entidades;

namespace Test.Domain.Entidades;

[TestClass]
public class AdministradorTest
{
    [TestMethod]
    public void TestarGetSetPropriedades()
    {
        // Arrange
        var adm = new Administrador();

        // Act
        adm.Id = 1;
        adm.Email = "joao@test.com";
        adm.Senha = "joaotest";
        adm.Perfil = "Adm";

        // Assert
        Assert.AreEqual(1, adm.Id);
        Assert.AreEqual("joao@test.com", adm.Email);
        Assert.AreEqual("joaotest", adm.Senha);
        Assert.AreEqual("Adm", adm.Perfil);
    }
}