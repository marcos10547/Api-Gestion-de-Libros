var builder = WebApplication.CreateBuilder(args);


var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");


builder.Services.AddScoped<IAutorRepository, AutorRepository>(provider =>
    new AutorRepository(connectionString)
);
builder.Services.AddScoped<IAutorService, AutorService>();


builder.Services.AddScoped<IClienteRepository, ClienteRepository>(provider =>
    new ClienteRepository(connectionString)
);
builder.Services.AddScoped<IClienteService, ClienteService>();

builder.Services.AddScoped<ILibroRepository, LibroRepository>(provider =>
    new LibroRepository(connectionString)
);
builder.Services.AddScoped<ILibroService, LibroService>();

builder.Services.AddScoped<IGeneroRepository, GeneroRepository>(provider => 
    new GeneroRepository(connectionString)
);
builder.Services.AddScoped<IGeneroService, GeneroService>();



builder.Services.AddCors(options =>
{
    options.AddPolicy("PermitirTodo", app => 
    {
        app.AllowAnyOrigin()
           .AllowAnyHeader()
           .AllowAnyMethod();
    });
});


// Servicios base de la API
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


var app = builder.Build();


if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("PermitirTodo");

app.UseAuthorization();

app.MapControllers();

app.Run();