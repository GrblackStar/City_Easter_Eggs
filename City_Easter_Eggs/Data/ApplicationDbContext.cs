#region Using

using City_Easter_Eggs.Models;
using Microsoft.EntityFrameworkCore;

#endregion

namespace City_Easter_Eggs.Data
{
	public class ApplicationDbContext : DbContext
	{
		public DbSet<User> Users { get; set; } = default!;
		public DbSet<PointOfInterest> POIs { get; set; } = default!;

		public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
		{
		}

		protected override void OnModelCreating(ModelBuilder builder)
		{
			base.OnModelCreating(builder);

			builder.Entity<User>().HasMany(u => u.PlacedPoints);
			builder.Entity<PointOfInterest>().HasOne(p => p.Creator);
		}

		public static void SetupDatabase(WebApplicationBuilder builder)
		{
			string? connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
			if (connectionString == null)
				throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

			void OptionsConfigCallback(DbContextOptionsBuilder options)
			{
				options.UseSqlServer(connectionString);
			}

			builder.Services.AddDbContext<ApplicationDbContext>(OptionsConfigCallback);

			// Open a second connection to apply migrations.
			{
				var dbBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
				OptionsConfigCallback(dbBuilder);
				var dbContext = new ApplicationDbContext(dbBuilder.Options);
				dbContext.Database.Migrate();

				// Initial debug data
				/*if (!dbContext.POIs.Any())
				{
					var systemUser = new User("SystemUser");
					dbContext.Users.Add(systemUser);

					for (var x = 0; x < 10; x++)
					{
						for (var y = 0; y < 10; y++)
						{
							dbContext.POIs.Add(new PointOfInterest
							{
								PointId = $"test_{x}x{y}",
								Name = $"Debug Point {x} {y}",
								Longitude = 20 + x,
								Latitude = 40 + y,
								Creator = systemUser
							});
						}
					}
				}*/

				dbContext.SaveChanges();
				dbContext.Dispose();
			}
		}
	}
}