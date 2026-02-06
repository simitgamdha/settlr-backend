using Settlr.Web.Extension;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSettlrControllers();
builder.Services.AddSettlrSwagger();
builder.Services.AddSettlrAuthentication(builder.Configuration);
builder.Services.AddSettlrDataAccess(builder.Configuration);
builder.Services.AddSettlrServices();

var app = builder.Build();

app.UseSettlrExceptionHandling();
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.UseSettlrSwagger();
app.MapControllers();

app.Run();
