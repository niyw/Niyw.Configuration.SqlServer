# Niyw.Configuration.SqlServer
asp.net core configuration provider basing microsoft sql server

# Getting start
Edit Program.cs following the below simple code:
``` csharp
            .ConfigureAppConfiguration((hostingContext, config) => {
                var builtConfig = config.Build();
                var dbConnStr = builtConfig.GetConnectionString("AppConfigDB");              
                var migrationsAssembly = typeof(Program).GetTypeInfo().Assembly.GetName().Name;
                config.SetBasePath(Directory.GetCurrentDirectory());
                config.AddSqlServerConfiguration((options) => {
                    options.UseSqlServer(dbConnStr, sql => sql.MigrationsAssembly(migrationsAssembly));
                });
                var srclist = config;
            })
            .Configure(appBuilder=> {
                appBuilder.UseSqlServerConfiguration();
            })
```