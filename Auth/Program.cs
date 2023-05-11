using Auth.Extensions;
using Auth.Features;
using Auth.Services.EmailService;
using DAL.Auth.Repository;
using DAL.Auth.Repository.Interfaces;

var builder = WebApplication.CreateBuilder(args);

builder.Services.ConfigureCors();
builder.Services.ConfigureSqlContext(builder.Configuration);
builder.Services.ConfigureReviewRepository();
builder.Services.AddScoped<ILikeRepository, LikeRepository>();
builder.Services.AddScoped<IRatingRepository, RatingRepository>();
builder.Services.AddAutoMapper(typeof(Program));
builder.Services.ConfigureIdentity();

builder.Services.AddScoped<JwtHandler>();

var emailConfig = builder.Configuration
    .GetSection("EmailConfiguration")
    .Get<EmailConfiguration>();

builder.Services.AddSingleton(emailConfig);
builder.Services.AddScoped<IEmailSender, EmailSender>();

builder.Services.ConfigureAuth(builder.Configuration);

builder.Services.AddControllers();

builder.Services.AddAWSLambdaHosting(LambdaEventSource.HttpApi);

var app = builder.Build();

app.UseRouting();
app.UseCors();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();