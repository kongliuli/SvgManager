using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using System.Text.Json;
using SvgColorNormalizer.Core.Models;

namespace SvgColorNormalizer.Core.DataSources
{
    public class ConfigurableApiSvgSource : ConfigurableDataSourceBase
    {
        public override string SourceType => "API接口";

        public override async Task<IEnumerable<SvgData>> LoadAsync()
        {
            if (!ValidateConfiguration())
                throw new System.Exception("配置无效");

            var apiUrl = Configuration["ApiUrl"];
            var results = new List<SvgData>();

            using var httpClient = new HttpClient();
            if (Configuration.ContainsKey("ApiKey"))
            {
                httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {Configuration["ApiKey"]}");
            }

            var response = await httpClient.GetAsync(apiUrl);
            response.EnsureSuccessStatusCode();

            var jsonContent = await response.Content.ReadAsStringAsync();
            var apiResponse = JsonSerializer.Deserialize<ApiSvgResponse>(jsonContent);

            if (apiResponse != null && apiResponse.Svgs != null)
            {
                foreach (var svgItem in apiResponse.Svgs)
                {
                    results.Add(new SvgData
                    {
                        Source = svgItem.Name,
                        Content = svgItem.Content,
                        Metadata = new Dictionary<string, object>
                        {
                            ["apiUrl"] = apiUrl,
                            ["svgId"] = svgItem.Id
                        }
                    });
                }
            }

            return results;
        }

        public override bool ValidateConfiguration()
        {
            return Configuration.ContainsKey("ApiUrl");
        }

        public override Task InitializeAsync()
        {
            // API数据源不需要初始化
            return Task.CompletedTask;
        }

        private class ApiSvgResponse
        {
            public List<ApiSvgItem> Svgs { get; set; }
        }

        private class ApiSvgItem
        {
            public string Id { get; set; }
            public string Name { get; set; }
            public string Content { get; set; }
        }
    }
}
