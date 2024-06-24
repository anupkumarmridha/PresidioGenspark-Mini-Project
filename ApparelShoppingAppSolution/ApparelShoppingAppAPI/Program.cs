using ApparelShoppingAppAPI.Contexts;
using ApparelShoppingAppAPI.Repositories.Classes;
using ApparelShoppingAppAPI.Repositories.Interfaces;
using ApparelShoppingAppAPI.Services.Classes;
using ApparelShoppingAppAPI.Services.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

namespace ApparelShoppingAppAPI
{
    public class Program
    {
        private static void AddDbContext(IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<ShoppingAppDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("defaultConnection")));
        }

        #region RegisterRepositories
        /// <summary>
        /// Registering Repositories
        /// </summary>
        /// <param name="services"></param>
        private static void RegisterRepositories(IServiceCollection services)
        {
            services.AddScoped<ICustomerRepository, CustomerRepository>();
            services.AddScoped<ISellerRepository, SellerRepository>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IUserRegisterRepository, UserRegisterRepository>();
            services.AddScoped<IProductRepository, ProductRepository>();
            services.AddScoped<ICartRepository, CartRepository>();
            services.AddScoped<IAddressRepository, AddressRepository>();
            services.AddScoped<IOrderRepository, OrderRepository>();
            services.AddScoped<IPaymentRepository, PaymentRepository>();
            services.AddScoped<IReviewRepository, ReviewRepository>();
        }
        #endregion RegisterRepositories

        #region RegisterServices
        /// <summary>
        /// Registering Services
        /// </summary>
        /// <param name="services"></param>
        private static void RegisterServices(IServiceCollection services)
        {
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<ITokenService, TokenService>();
            services.AddScoped<IProductService, ProductService>();
            services.AddScoped<ICartService, CartService>();
            services.AddScoped<IAddressService, AddressService>();
            services.AddScoped<IOrderService, OrderService>();
            services.AddScoped<IPaymentService, PaymentService>();
            services.AddScoped<IReviewService, ReviewService>();

        }
        #endregion RegisterServices

        #region AddJWTTokenSwaggerGen
        /// <summary>
        /// Adding JWT Token to Swagger
        /// </summary>
        /// <param name="services"></param>
        private static void AddJWTTokenSwaggerGen(IServiceCollection services)
        {
            services.AddSwaggerGen(option =>
            {
                option.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "JWT Authorization header using the Bearer scheme. \r\n\r\n Enter 'Bearer' [space] and then your token in the text input below.\r\n\r\nExample: \"Bearer 1safsfsdfdfd\"",
                });
                option.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        new string[] { }
                    }
                });
            });
        }
        #endregion AddJWTTokenSwaggerGen

        #region ValidateToken
        /// <summary>
        /// Validate Token
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        private static void ValidateToken(IServiceCollection services, IConfiguration configuration)
        {
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters()
                    {
                        ValidateIssuer = false,
                        ValidateAudience = false,
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["TokenKey:JWT"]))
                    };

                });
        }
        #endregion ValidateToken

        #region ConfigureServices
        /// <summary>
        /// Registering Services
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        private static void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            services.AddControllers();
            //.AddJsonOptions(options =>
            //{
            //    options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.Preserve;
            //    options.JsonSerializerOptions.DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull;
            //});
            #region CORS
            services.AddCors(opts =>
            {
                opts.AddPolicy("MyCors", options =>
                {
                    options.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin();
                });
            });
            #endregion
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen();

            //Token operations
            AddJWTTokenSwaggerGen(services);
            ValidateToken(services, configuration);

            AddDbContext(services, configuration);

            RegisterRepositories(services);
            RegisterServices(services);
        }
        #endregion ConfigureServices

        #region Main
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            ConfigureServices(builder.Services, builder.Configuration);


            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseCors("MyCors");
            app.MapControllers();

            app.Run();
        }
        #endregion Main
    }
}
