using pwnctl;
using pwnctl.infra.Persistence;
using pwnctl.core.Entities.Assets;
using pwnctl.app.Utilities;
using System;
using System.Linq;
using System.IO;
using System.Threading.Tasks;

//ID,Time,Tool,Method,Protocol,Host,Port,URL,IP,Path,Query,Param count,Param names,Status,Length,
//MIME type,Extension,Page title,Start response timer,End response timer,Comment,Request,Response

namespace pwnctl.app.Importers
{
    public static class BurpSuiteImporter
    {
        public static Task ImportAsync(string file)
        {
            var csv = File.ReadLines(file).Skip(1);
            foreach (var line in csv)
            {
                var parts = line.Split(",");
                var method = parts[3];
                var protocol = parts[4];
                var host = parts[5];
                var port = parts[6];
                var url = parts[7];
                var ip = parts[8];
                var path = parts[9];
            }

            throw new NotImplementedException();
        }
    }
}