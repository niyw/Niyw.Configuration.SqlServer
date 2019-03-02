# Niyw.Configuration.SqlServer
����΢��SQL Server��Asp.NET Core����ϵͳ�ṩ����

# ���ʹ��
��Programs.cs�а���������´��룺
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
