var builder = WebApplication.CreateBuilder(args);


var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");


// --- A. INYECCIÓN DE REPOSITORIOS (ADO.NET) ---
// Registramos el Repositorio y le inyectamos el string de conexión.
builder.Services.AddScoped<IAutorRepository, AutorRepository>(provider =>
    new AutorRepository(connectionString)
);
builder.Services.AddScoped<IAutorService, AutorService>();



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


// Configurar el pipeline de HTTP.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// --- ACTIVAR CORS ---
app.UseCors("PermitirTodo");

app.UseAuthorization();

app.MapControllers();

app.Run();