using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/no";
        options.AccessDeniedPath = "/Forbidden/";
    });
builder.Services.AddAuthorization(options => {
});

var app = builder.Build();
app.UseAuthentication();
app.UseAuthorization();

app.MapGet("/", () => "Hello World!");
app.MapGet("/no", () => "No!");
app.MapGet("/me", [Authorize] () => "Me!");
app.MapGet("/html", () => Results.Text("<h1>Header!</h1>", "text/html"));
app.MapGet("/context", (HttpContext context) => context.Response.WriteAsync("Context!"));
app.MapGet("/login_form2", (HttpContext context) => { 
    var form_text = @"<form action='/login3' method='POST'>
        Username: <input type='text' name='username'/><br/>
        Password: <input type='text' name='password'/><br/>
        <input type='submit'/>
    </form>";
    context.Response.ContentType = "text/html; charset=UTF-8";
    context.Response.WriteAsync(form_text);
});
app.MapPost("/login2", (HttpContext context) =>  {
    return $"{context.Request.Form["username"]} {context.Request.Form["password"]}";
});
app.MapGet("/login_form3", (HttpContext context) => { 
    var form_text = @"<form action='/login3' method='GET'>
        Username: <input type='text' name='username'/><br/>
        Password: <input type='text' name='password'/><br/>
        <input type='submit'/>
    </form>";
    context.Response.ContentType = "text/html; charset=UTF-8";
    context.Response.WriteAsync(form_text);
});
app.MapGet("/login3", (string username, string password) => {
    return $"{username} {password}";
});
app.MapGet("/login_form4", (HttpContext context) => { 
    var form_text = @"<form action='/login4' method='POST'>
        Username: <input type='text' name='username'/><br/>
        Password: <input type='text' name='password'/><br/>
        <input type='submit'/>
    </form>";
    context.Response.ContentType = "text/html; charset=UTF-8";
    context.Response.WriteAsync(form_text);
});
// Binding from form values is not supported in .NET 6.
app.MapPost("/login4", ([FromForm] string username,[FromForm] string password) => {
    return $"{username} {password}";
});
app.MapGet("/login_form5", (HttpContext context) => { 
    var form_text = @"<form action='/login5' method='GET'>
        Username: <input type='text' name='username'/><br/>
        Password: <input type='text' name='password'/><br/>
        <input type='submit'/>
    </form>";
    context.Response.ContentType = "text/html; charset=UTF-8";
    context.Response.WriteAsync(form_text);
});
app.MapGet("/login5", ([FromBody] User u) => {
    return $"{u.username} {u.password}";
});
app.MapGet("/login", (HttpContext context) => 
{
    var claims = new List<Claim>
    {
        new Claim(ClaimTypes.Name, "korovaikon@gmail.com"),
        new Claim("FullName", "Nikolay Korovaiko"),
        new Claim(ClaimTypes.Role, "Administrator"),
    };

    var claimsIdentity = new ClaimsIdentity(
        claims, CookieAuthenticationDefaults.AuthenticationScheme);

    var authProperties = new AuthenticationProperties
    {
    };

    context.SignInAsync(
        CookieAuthenticationDefaults.AuthenticationScheme, 
        new ClaimsPrincipal(claimsIdentity), 
            authProperties);
    context.Response.WriteAsync("You Are Logged In!");
}
);
app.Run();


record User(string username, string password);