var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapGet("/", () => "Hello World!");

app.MapPost("/login", (MinimalApi.DTOs.LoginDTO loginDTO) => {
    // Login logic here
    if(loginDTO.Email == "admin@email.com" && loginDTO.Senha == "123456")
        return Results.Ok("Login realizado com sucesso");
    else
        return Results.Unauthorized();
});

app.Run();
