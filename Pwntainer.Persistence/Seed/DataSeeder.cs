using DnsClient;
using Newtonsoft.Json;
using Pwntainer.Application.Entities;
using Pwntainer.Core;
using Pwntainer.Persistence.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Pwntainer.Persistence.Seed
{
    public class DataSeeder
    {
        private readonly AssetService _service;
        private string AssemblyPath
        {
            get
            {
                string codeBase = Assembly.GetExecutingAssembly().CodeBase;
                UriBuilder uri = new UriBuilder(codeBase);
                string path = Uri.UnescapeDataString(uri.Path);
                return Path.GetDirectoryName(path);
            }
        }

        public DataSeeder(AssetService service)
        {
            _service = service;
        }

        public void SeedAll()
        {
            SeedDomains(AssemblyPath + "/Seed/Data/domains.txt");
            SeedDomains(AssemblyPath + "/Seed/Data/wildcards.txt");

            SeedBugBountyData(AssemblyPath + "/Seed/Data/hackerone_data.json");
            SeedBugBountyData(AssemblyPath + "/Seed/Data/bugcrowd_data.json");
            SeedBugBountyData(AssemblyPath + "/Seed/Data/federacy_data.json");
            SeedBugBountyData(AssemblyPath + "/Seed/Data/hackenproof_data.json");
            SeedBugBountyData(AssemblyPath + "/Seed/Data/intigriti_data.json");
            SeedBugBountyData(AssemblyPath + "/Seed/Data/yeswehack_data.json");
        }

        private void SeedDomains(string filename)
        {
            string[] domains = File.ReadAllLines(filename);

            foreach (var domain in domains)
            {
                _service.AddAsset(domain);
            }
        }

        private void SeedBugBountyData(string filename)
        {
            string bcData = File.ReadAllText(filename);

            var bcPrograms = JsonConvert.DeserializeObject<List<BugBountyDataProgram>>(bcData);
            foreach (var program in bcPrograms)
            {
                foreach (var asset in program.targets.in_scope)
                {
                    if (asset.type.Contains("api", StringComparison.OrdinalIgnoreCase) || asset.type.Contains("web", StringComparison.OrdinalIgnoreCase) || asset.type == "Protocol")
                    {
                        _service.AddAsset(asset.target);
                    }
                    else if (asset.type == "url")
                    {
                        _service.AddAsset(asset.endpoint);
                    }
                    else if (asset.type == "iprange")
                    {
                        _service.AddAsset(asset.endpoint);
                    }
                    else if (asset.asset_type == "URL")
                    {
                        _service.AddAsset(asset.asset_identifier);
                    }
                    else if (asset.asset_type == "CIDR")
                    {
                        _service.AddAsset(asset.asset_identifier);
                    }
                    else if (asset.asset_type == "SOURCE_CODE")
                    {
                        _service.AddAsset(asset.asset_identifier);
                    }
                    else if (asset.type == "ip-address")
                    {
                        _service.AddAsset(asset.target);
                    }
                }
            }
        }

        internal class BugBountyDataInScope
        {
            public string type { get; set; }
            public string target { get; set; }
            public string endpoint { get; set; }
            public string asset_identifier { get; set; }
            public string asset_type { get; set; }
        }

        internal class BugBountyDataTargets
        {
            public List<BugBountyDataInScope> in_scope { get; set; }
            public List<object> out_of_scope { get; set; }
        }

        internal class BugBountyDataProgram
        {
            public string name { get; set; }
            public string url { get; set; }
            public bool allows_disclosure { get; set; }
            public bool managed_by_bugcrowd { get; set; }
            public string safe_harbor { get; set; }
            public int max_payout { get; set; }
            public BugBountyDataTargets targets { get; set; }
        }
    }
}
