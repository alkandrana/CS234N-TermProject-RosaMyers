using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

/*
 * dotnet tool install --global dotnet-ef
 * Use nuget package manager to install efcore efcore.analyzers, design, tools, and mysql.data.efcore
 * -- Install-Package Microsoft.EntityFrameworkCore.Tools could do from the console too
 * Get-Help about_EntityFrameworkCore frome the console
 * dotnet ef dbcontext scaffold "server=127.0.0.1;uid=root;pwd=YOURPASSWORDHERE;database=bits" Pomelo.EntityFrameworkCore.MySql -o Models -c MMABooksContext -p MMABooksEFClasses -s MMABooksEFClasses -f 
 */
namespace InventoryTracker;

public class ConfigDB
{
 public static string GetMySQLConnectionString()
 {
  string folder = System.AppContext.BaseDirectory;
  var builder = new ConfigurationBuilder()
   .SetBasePath(folder)
   .AddJsonFile("mySqlSettings.json", optional: true, reloadOnChange: true);
  string connectionString = builder.Build().GetConnectionString("mySql");
  return connectionString;
 }
}