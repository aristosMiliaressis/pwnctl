using Microsoft.Extensions.Configuration;
using Pwntainer.Persistence;
using Pwntainer.Persistence.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;

namespace Pwntainer.AssetArchiver
{
    class Program
    {
        static void Main(string[] args)
        {
            var dbContext = new PwntainerDbContext();
            var assetService = new AssetService(dbContext);

            do
            {
                var assetLine = Console.ReadLine();
                assetService.AddAsset(assetLine);
            } while (true);
        }
    }
}
