using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(opt => opt.UseInMemoryDatabase("SimpleDb"));
builder.Services.AddEndpointsApiExplorer();

var app = builder.Build();

#region 1. Таблица User (Пользователи) - CRUD

app.MapGet("/users", async (AppDbContext db) => 
    await db.Users.Include(u => u.Profile).ToListAsync());

app.MapGet("/users/{id:int}", async (int id, AppDbContext db) =>
    await db.Users.Include(u => u.Profile).FirstOrDefaultAsync(u => u.Id == id) is User user 
        ? Results.Ok(user) 
        : Results.NotFound());

app.MapPost("/users", async (User user, AppDbContext db) =>
{
    db.Users.Add(user);
    await db.SaveChangesAsync();
    return Results.Created($"/users/{user.Id}", user);
});

app.MapPut("/users/{id:int}", async (int id, User inputUser, AppDbContext db) =>
{
    var user = await db.Users.FindAsync(id);
    if (user is null) return Results.NotFound();

    user.Username = inputUser.Username;
    user.Email = inputUser.Email;

    await db.SaveChangesAsync();
    return Results.NoContent();
});

app.MapDelete("/users/{id:int}", async (int id, AppDbContext db) =>
{
    if (await db.Users.FindAsync(id) is User user)
    {
        db.Users.Remove(user);
        await db.SaveChangesAsync();
        return Results.Ok(user);
    }
    return Results.NotFound();
});

#endregion

#region 2. Таблица UserProfile (Профили, связь 1:1 с User) - CRUD

app.MapGet("/profiles", async (AppDbContext db) => 
    await db.UserProfiles.ToListAsync());

app.MapGet("/profiles/{id:int}", async (int id, AppDbContext db) =>
    await db.UserProfiles.FindAsync(id) is UserProfile profile 
        ? Results.Ok(profile) 
        : Results.NotFound());

app.MapPost("/profiles", async (UserProfile profile, AppDbContext db) =>
{
    var userExists = await db.Users.AnyAsync(u => u.Id == profile.UserId);
    if (!userExists) return Results.BadRequest("User with this Id does not exist.");

    var profileExists = await db.UserProfiles.AnyAsync(p => p.UserId == profile.UserId);
    if (profileExists) return Results.BadRequest("Profile for this user already exists.");

    db.UserProfiles.Add(profile);
    await db.SaveChangesAsync();
    return Results.Created($"/profiles/{profile.Id}", profile);
});

app.MapPut("/profiles/{id:int}", async (int id, UserProfile inputProfile, AppDbContext db) =>
{
    var profile = await db.UserProfiles.FindAsync(id);
    if (profile is null) return Results.NotFound();

    profile.FullName = inputProfile.FullName;
    profile.Bio = inputProfile.Bio;

    await db.SaveChangesAsync();
    return Results.NoContent();
});

app.MapDelete("/profiles/{id:int}", async (int id, AppDbContext db) =>
{
    if (await db.UserProfiles.FindAsync(id) is UserProfile profile)
    {
        db.UserProfiles.Remove(profile);
        await db.SaveChangesAsync();
        return Results.Ok(profile);
    }
    return Results.NotFound();
});

#endregion

#region 3. Таблица SystemConfig (Настройки, изолированная) - CRUD

app.MapGet("/configs", async (AppDbContext db) => 
    await db.SystemConfigs.ToListAsync());

app.MapGet("/configs/{id:int}", async (int id, AppDbContext db) =>
    await db.SystemConfigs.FindAsync(id) is SystemConfig config 
        ? Results.Ok(config) 
        : Results.NotFound());

app.MapPost("/configs", async (SystemConfig config, AppDbContext db) =>
{
    db.SystemConfigs.Add(config);
    await db.SaveChangesAsync();
    return Results.Created($"/configs/{config.Id}", config);
});

app.MapPut("/configs/{id:int}", async (int id, SystemConfig inputConfig, AppDbContext db) =>
{
    var config = await db.SystemConfigs.FindAsync(id);
    if (config is null) return Results.NotFound();

    config.Key = inputConfig.Key;
    config.Value = inputConfig.Value;

    await db.SaveChangesAsync();
    return Results.NoContent();
});

app.MapDelete("/configs/{id:int}", async (int id, AppDbContext db) =>
{
    if (await db.SystemConfigs.FindAsync(id) is SystemConfig config)
    {
        db.SystemConfigs.Remove(config);
        await db.SaveChangesAsync();
        return Results.Ok(config);
    }
    return Results.NotFound();
});

#endregion

app.Run();

#region Описание таблиц и контекста БД

public class User
{
    public int Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public UserProfile? Profile { get; set; }
}

public class UserProfile
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Bio { get; set; } = string.Empty;
}

public class SystemConfig
{
    public int Id { get; set; }
    public string Key { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
}

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<User> Users => Set<User>();
    public DbSet<UserProfile> UserProfiles => Set<UserProfile>();
    public DbSet<SystemConfig> SystemConfigs => Set<SystemConfig>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>()
            .HasOne(u => u.Profile)
            .WithOne()
            .HasForeignKey<UserProfile>(p => p.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

#endregion
