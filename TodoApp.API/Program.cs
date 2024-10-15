using TodoApp.BL;
using TodoApp.DL;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddScoped<IBaseDL, BaseDL>();
builder.Services.AddScoped<IGroupDL, GroupDL>();
builder.Services.AddScoped<ITodoDL, TodoDL>();
builder.Services.AddScoped<IUserDL, UserDL>();

builder.Services.AddScoped<IBaseBL, BaseBL>();
builder.Services.AddScoped<IGroupBL, GroupBL>();
builder.Services.AddScoped<ITodoBL, TodoBL>();
builder.Services.AddScoped<IUserBL, UserBL>();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

DatabaseConstant.ConnectionString = builder.Configuration.GetConnectionString("MySql");

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();

app.Run();
