using Booking.BuildingBlocks.Domain.Enums;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.





builder.Services.AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = "Booking",
            ValidAudience = "Booking",
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("my_awesome_secret_key_the_best_in_world"))
        };
    });



builder.Services.AddAuthorization(options =>
{
    AddPolicy(options, "ChangeUserInfo", ["Host"], Permission.ChangeUserInfo.ToString());
    AddPolicy(options, "HostAccommodationOperations", ["Host"], Permission.HostAccommodationOperations.ToString());
    AddPolicy(options, "HostReservationOperations", ["Host"], Permission.HostReservationOperations.ToString());
    AddPolicy(options, "GuestReservationOperations", ["Guest"], Permission.GuestReservationOperations.ToString());
    AddPolicy(options, "GetGuestReservations", ["Guest"], Permission.GetGuestReservations.ToString());
    AddPolicy(options, "GetHostReservations", ["Host"], Permission.GetHostReservations.ToString());
    AddPolicy(options, "SubscriptionOperations", ["Host"], Permission.SubscriptionOperations.ToString());
    AddPolicy(options, "GuestReservationPayment", ["Guest"], Permission.GuestReservationPayment.ToString());
    AddPolicy(options, "HostReservationPayment", ["Host"], Permission.HostReservationPayment.ToString());
    AddPolicy(options, "ReservationOperations", ["Host"], Permission.ReservationOperations.ToString());
    AddPolicy(options, "InvoicesOperations", ["Host", "Guest"], Permission.InvoicesOperations.ToString());
    AddPolicy(options, "CreateReview", ["Guest"], Permission.CreateReview.ToString());
});

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();




var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors(policy =>
     policy.AllowAnyOrigin()
         .AllowAnyMethod()
         .AllowAnyHeader());

app.UseHttpsRedirection();


app.UseAuthentication();
app.UseAuthorization();


app.MapReverseProxy();

app.Run();

void AddPolicy(AuthorizationOptions options, string policyName, string[] role, string permission)
{
    options.AddPolicy(
        policyName,
        policy => policy
            .RequireAuthenticatedUser()
            .RequireRole(role)
            .RequireClaim("permission", permission));
}
