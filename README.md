# Niyw.Configuration.SqlServer
基于微软SQL Server的Asp.NET Core配置系统提供程序

# 如何使用
在Programs.cs中按照下面更新代码：
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
