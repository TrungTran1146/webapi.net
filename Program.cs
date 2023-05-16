using WebAPI.Model;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using WebAPI.Redis;

var builder = WebApplication.CreateBuilder(args);

Microsoft.Extensions.Configuration.ConfigurationManager configuration = builder.Configuration;

//1.tao postgres
builder.Services.AddDbContext<DataContext>(o =>
o.UseNpgsql(builder.Configuration.GetConnectionString("Postgres_Db")));
// Add services to the container.


builder.Services.AddScoped<IRedisCacheService, RedisCacheService>();


/////////////////////////////////////////////////////////////////////////////
// For Identity
//builder.Services.AddIdentity<IdentityUser, IdentityRole>()
//    .AddEntityFrameworkStores<DataContext>()
//    .AddDefaultTokenProviders();

// Adding Authentication
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
})
// Adding Jwt Bearer
.AddJwtBearer(options =>
{
    options.SaveToken = true;
    options.RequireHttpsMetadata = false;
    options.TokenValidationParameters = new TokenValidationParameters()
    {
        //tự cấp token
        ValidateIssuer = false,
        ValidateAudience = false,

        // ValidAudience = configuration["JWT:ValidAudience"],
        // ValidIssuer = configuration["JWT:ValidIssuer"],
        //ký vào token
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(configuration["JWT:Secret"])),
        ClockSkew = TimeSpan.Zero
    };
});
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("admin", policy => policy.RequireRole("admin"));

    options.AddPolicy(
        "admin1",
        policyBuilder => policyBuilder.RequireAssertion(
            context => context.User.HasClaim(claim => claim.Type == "admin")
       //|| context.User.HasClaim(claim => claim.Type == "IT")
       // || context.User.IsInRole("CEO")

       ));
});

///////////////////////////////////////////////////////////////////////////////////
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
// 5. Swagger authentication
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Car API", Version = "v1" });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = @"JWT Authorization header using the Bearer scheme. \r\n\r\n 
                      Enter 'Bearer' [space] and then your token in the text input below.
                      \r\n\r\nExample: 'Bearer 12345abcdef'",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement()
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                },
                Scheme = "oauth2",
                Name = "Bearer",
                In = ParameterLocation.Header,
            },
            new List<string>()
        }
    });
});
builder.Services.AddSwaggerGen();
// 6. Add CORS policy
//builder.Services.AddCors(options =>
//{
//    options.AddPolicy("AllowAngularDevClient",
//        b =>
//        {
//            b
//                .WithOrigins("http://localhost:3000")
//                .AllowAnyHeader()
//                .AllowAnyMethod();
//        });
//});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
//7. Use CORS
//app.UseCors("AllowAngularDevClient");
app.UseCors(builder =>
        builder.WithOrigins("http://localhost:3001")
            .AllowAnyHeader()
            .AllowAnyMethod());

app.UseHttpsRedirection();

//xác thực
app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
