using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options => {
    options.AddDefaultPolicy(policy =>
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader());
});
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton<IRepositorioVeiculos, RepositorioVeiculosMemoria>();

var app = builder.Build();

app.UseCors();
app.UseSwagger();
app.UseSwaggerUI();

app.MapPost("/veiculos", (Veiculo veiculo, IRepositorioVeiculos repositorio) => {
    if (string.IsNullOrWhiteSpace(veiculo.Marca) || string.IsNullOrWhiteSpace(veiculo.Modelo))
    {
        return Results.BadRequest("Marca e Modelo são obrigatórios.");
    }
    repositorio.Adicionar(veiculo);
    return Results.Ok(veiculo);
});

app.MapGet("/veiculos", (IRepositorioVeiculos repositorio) => Results.Ok(repositorio.ObterTodos()));

app.Run();

record Veiculo(string Marca, string Modelo);

interface IRepositorioVeiculos
{
    void Adicionar(Veiculo veiculo);
    IEnumerable<Veiculo> ObterTodos();
}

class RepositorioVeiculosMemoria : IRepositorioVeiculos
{
    private readonly List<Veiculo> _veiculos = new();

    public void Adicionar(Veiculo veiculo) => _veiculos.Add(veiculo);

    public IEnumerable<Veiculo> ObterTodos() => _veiculos;
}
