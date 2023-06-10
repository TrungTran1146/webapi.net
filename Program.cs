using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Minio.AspNetCore;
using System.Text;
using WebAPI.Logstash;
using WebAPI.Model;
using WebAPI.Redis;

var builder = WebApplication.CreateBuilder(args);

Microsoft.Extensions.Configuration.ConfigurationManager configuration = builder.Configuration;

//1.tao postgres
builder.Services.AddDbContext<DataContext>(o =>
o.UseNpgsql(builder.Configuration.GetConnectionString("Postgres_Db")));
// Add services to the container.


//MINIO
builder.Services.AddMinio(options =>
{
    builder.Configuration.GetSection("Minio").Bind(options);
    var port = builder.Configuration.GetValue<int>("Minio:Port");
    options.ConfigureClient(client =>
    {
        client.WithEndpoint(options.Endpoint, port);
    });
});
///*builder.Services.AddSingleton(new MinioClient(
//       builder.Configuration.GetValue<string>("Minio:Endpoint"),
//       builder.Configuration.GetValue<string>("Minio:AccessKey"),
//        builder.Configuration.GetValue<string>("Minio:SecretKey"),
//         builder.Configuration.GetValue<string>("Minio:BucketName")));*/


//redis
builder.Services.AddScoped<IRedisCacheService, RedisCacheService>();

////kafka
//builder.Services.AddSingleton(new ProducerBuilder<Null, string>(new ProducerConfig { BootstrapServers = "localhost:9092" }));


//// Đăng ký ProductConsumer
//builder.Services.AddSingleton<CartConsumer>();



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


//Logstash
builder.Services.AddElasticsearch(builder.Configuration);

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
        builder.WithOrigins("http://localhost:3000")
            .AllowAnyHeader()
            .AllowAnyMethod());

app.UseHttpsRedirection();

//xác thực
app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
