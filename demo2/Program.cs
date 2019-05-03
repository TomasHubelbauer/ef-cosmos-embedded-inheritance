using System;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace demo2
{
  class Program
  {
    static async Task Main(string[] args)
    {
      string primaryKey;
      try
      {
        Console.WriteLine("Downloading the primary key from https://localhost:8081/_explorer/quickstart.html…");
        var html = await new HttpClient().GetStringAsync("https://localhost:8081/_explorer/quickstart.html");
        primaryKey = Regex.Match(html, "Primary Key</p>\\s+<input .* value=\"(?<primaryKey>.*)\"").Groups["primaryKey"].Value;
        Console.WriteLine("The primary key has been downloaded.");
      }
      catch
      {
        Console.WriteLine("Failed to download the primary key. Make sure to install and run the Cosmos emulator.");
        Console.WriteLine("The primary key gets downloaded from https://localhost:8081/_explorer/quickstart.html");
        return;
      }

      using (var appDbContext = new AppDbContext(primaryKey))
      {
        await appDbContext.Database.EnsureDeletedAsync();
        await appDbContext.Database.EnsureCreatedAsync();
        await appDbContext.Admins.AddAsync(new Admin
        {
          FirstName = "Tomas",
          LastName = "Hubelbauer",
          FavoriteLookup = new LookupByFirstName
          {
            FirstName = "Tomas",
          },
        });

        await appDbContext.SaveChangesAsync();
        var admin = await appDbContext.Admins.SingleAsync();
        // Place a breakpoint on this line
      }

      using (var appDbContext = new AppDbContext(primaryKey))
      {
        // TODO: Figure out why `FavoriteLookup` is empty here and crashes with `Include(a => a.FavoriteLookup)`
        var admin = await appDbContext.Admins.SingleAsync();
        // Place a breakpoint on this line
      }
    }
  }
  public class AppDbContext : DbContext
  {
    private string primaryKey;

    public AppDbContext(string primaryKey)
    {
      this.primaryKey = primaryKey;
    }

    public DbSet<Admin> Admins { get; set; }
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
      optionsBuilder.UseCosmos("https://localhost:8081", this.primaryKey, nameof(demo2));
    }
  }

  public sealed class Admin
  {
    public Guid Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public dynamic FavoriteLookup { get; set; }
  }
  public sealed class LookupByFirstName
  {
    public Guid Id { get; set; }
    public string FirstName { get; set; }
  }
  public sealed class LookupByLastName
  {
    public Guid Id { get; set; }
    public string LastName { get; set; }
  }
  public sealed class LookupByEmploymentInDateAndTimeRange
  {
    public Guid Id { get; set; }
    public DateTime From { get; set; }
    public DateTime To { get; set; }
  }
}
