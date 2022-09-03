using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc;


var builder = WebApplication.CreateBuilder(args);

var app = builder.Build();

List<User> UsersList = new List<User>();

app.MapGet("/", () => "Hello World");

app.MapPost("/users", (HttpContext ctx, [FromBody] NewUserRequest body) =>
{
    User user = new User
    {
        email = body.User.email,
        password = body.User.password,
        username = body.User.username,
        bio = "",
        image = "",
        token = Guid.NewGuid().ToString()
    };

    bool exists = UsersList.Exists((u) => u.email == user.email);
    if (exists)
    {
        ctx.Response.StatusCode = 422;
        return null;
    }

    UsersList.Add(user);

    var userResponse = new UserResponse { user = user };
    ctx.Response.StatusCode = StatusCodes.Status201Created; // `ctx.Response.StatusCode = 201` works as well
    return userResponse;
});

app.MapPost("/users/login", (HttpContext ctx, [FromBody] NewUserRequest body) =>
{
    User? user = UsersList.Find((u) => u.email == body.User.email && u.password == body.User.password);
    if (user == null)
    {
        ctx.Response.StatusCode = StatusCodes.Status401Unauthorized;
        return null;
    }

    UserResponse userResponse = new UserResponse { user = user };

    return userResponse;
});

app.MapGet("/user", (HttpContext ctx, [FromBody] LoginUserRequest body) =>
{
    User? user = UsersList.Find((u) =>
        u.email == body.user.email &&
        u.password == body.user.password
        );
    ctx.Response.StatusCode = StatusCodes.Status200OK;
    return new UserResponse { user = user };
});

app.MapPut("/user", (HttpContext ctx, [FromBody] UpdateUserRequest body) =>
{
    User user = new User
    {
        email = body.user.email,
        token = body.user.token,
    };

    bool exists = UsersList.Find(user); 
    if (exists){
        UsersList.Add(user);
    }

    var userResponse = new UserResponse { user = user };
    return userResponse;
});




app.Run();


public class NewUser
{
    public string username { get; set; }
    public string email { get; set; }
    public string password { get; set; }
}

public class NewUserRequest
{
    public NewUser User { get; set; }
}


public class User
{
    public string email { get; set; }
    [JsonIgnore]
    public string password { get; set; }
    public string username { get; set; }
    public string bio { get; set; }
    public string image { get; set; }
    public string token { get; set; }
}

public class UserResponse
{
    public User user { get; set; }
}

public class LoginUserRequest
{
    public LoginUser user { get; set; }

}

public class LoginUser
{
    public string email { get; set; }
    public string password { get; set; }
}


public class UpdateUserRequest
{
    public UpdateUser user { get; set; }

}

public class UpdateUser
{
    public string email { get; set; }
    public string token { get; set; }
    public string username { get; set; }
    public string bio { get; set; }
    public string image { get; set; }
}
