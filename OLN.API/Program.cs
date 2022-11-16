using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using OLN.API.Authorization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddAuthentication(options =>
{
  options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
  options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
  options.Authority = "https://dev-qdpsr1z8.us.auth0.com/";
  options.Audience = "https://oln.com.ar/api/";
});

//Configuracion para Validar Autorización. 
builder.Services.AddAuthorization(options =>
    {
      options.AddPolicy("read:deliverymans", policy => policy.Requirements.Add(new HasScopeRequirement("read:deliverymans", "https://dev-qdpsr1z8.us.auth0.com/")));
      options.AddPolicy("write:deliverymans", policy => policy.Requirements.Add(new HasScopeRequirement("write:deliverymans", "https://dev-qdpsr1z8.us.auth0.com/")));
      options.AddPolicy("delete:deliverymans", policy => policy.Requirements.Add(new HasScopeRequirement("delete:deliverymans", "https://dev-qdpsr1z8.us.auth0.com/")));
      options.AddPolicy("write:shipmentorder", policy => policy.Requirements.Add(new HasScopeRequirement("write:shipmentorder", "https://dev-qdpsr1z8.us.auth0.com/")));
      options.AddPolicy("read:shipmentorder", policy => policy.Requirements.Add(new HasScopeRequirement("read:shipmentorder", "https://dev-qdpsr1z8.us.auth0.com/")));
      options.AddPolicy("write:shipmentstate", policy => policy.Requirements.Add(new HasScopeRequirement("write:shipmentstate", "https://dev-qdpsr1z8.us.auth0.com/")));
    });

builder.Services.AddSingleton<IAuthorizationHandler, HasScopeHandler>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
  app.UseSwagger();
  app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
