using DetailTECService.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<ILoginRepository, LoginRepo>();
builder.Services.AddScoped<IWorkerRepository, WorkerRepo>();
builder.Services.AddScoped<IOfficeRepository, OfficeRepo>();


//Middleware utilizado para habilitar politicas de CORS en los endpoints del REST API.
builder.Services.AddCors(options =>
{
    options.AddPolicy("Policy",
    policy =>
    {
        policy
        .AllowAnyOrigin()
        .AllowAnyHeader()
        .AllowAnyMethod();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


app.UseCors();

app.UseAuthorization();

app.MapControllers();

app.Run();
