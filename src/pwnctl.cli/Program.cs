using Cocona;
using pwnctl.infra.Persistence;
using pwnctl.infra.Repositories;
using pwnctl.app.Importers;
using pwnctl.app.Utilities;
using pwnctl.app;
using System;
using System.Collections.Generic;

PwnctlAppFacade.Setup();
var app = CoconaApp.Create();

app.AddCommand("query", () => 
    { 
        var queryRunner = new QueryRunner(PwnctlDbContext.ConnectionString);
        var input = new List<string>();

        string line;
        while (!string.IsNullOrEmpty(line = Console.ReadLine()))
        {
            input.Add(line);
        }

        queryRunner.Run(string.Join("\n", input));
    }
).WithDescription("Query mode (reads SQL from stdin executes and prints output to stdout)");

app.AddCommand("process", async () => 
    {
        var processor = new AssetProcessor();

        string line;
        while (!string.IsNullOrEmpty(line = Console.ReadLine()))
        {
            await processor.TryProccessAsync(line);
        }
    }
).WithDescription("Asset processing mode (reads assets from stdin)");

app.AddSubCommand("import", x =>
    {  
        x.AddCommand("csv-burp", async ( [Argument(Description = "path to burp csv file.")] string file ) 
        => {
            await BurpSuiteImporter.ImportAsync(file);
        }).WithDescription("BurpSuite CSV Import mode");
    }
).WithDescription("Import mode");

// app.AddSubCommand("list", x => 
// {
//     x.AddCommand("ips", () => );
//     x.AddCommand("urls", () => );
//     x.AddCommand("domains", () => );
//     x.AddCommand("ports", () => );
// }
// ).WithDescription("Import mode");

app.Run();
