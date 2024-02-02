using Luraph.APITypes;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Luraph
{
    public class LuraphAPI : IDisposable
    {
        private readonly HttpClient http = new HttpClient();
        private static readonly JsonSerializerOptions options = new JsonSerializerOptions
        {
            IncludeFields = true
        };

        public LuraphAPI(string apiKey)
        {
            http.BaseAddress = new Uri("https://api.lura.ph/v1/");
            http.DefaultRequestHeaders.Add("Luraph-API-Key", apiKey);
        }

        private static async Task<T> HandleResponse<T>(HttpResponseMessage resp)
        {
            if (!resp.IsSuccessStatusCode)
            {
                var ex = (await resp.Content.ReadFromJsonAsync<LuraphException>(options))!;
                if (ex.Errors != null)
                {
                    throw ex;
                }
                else
                {
                    throw new LuraphException((int)resp.StatusCode); //http status code wrapper
                }
            }

            var contentLength = resp.Content.Headers.ContentLength;
            if (contentLength == 0)
                return default!;

            if(typeof(T).IsAssignableFrom(typeof(HttpContent))){
                return (T)(object) resp.Content;
            }

            var content = await resp.Content.ReadAsStringAsync();
            if(content.Length == 0) 
                return default!;

            var warnings = JsonSerializer.Deserialize<LuraphWarnings>(content, options);
            if(warnings.Warnings != null)
            {
                foreach(var warning in warnings.Warnings)
                {
                    Console.Error.WriteLine($"Luraph API warning: {warning}");
                }
            }

            return JsonSerializer.Deserialize<T>(content, options)!;
        }

        private async Task<T> GetAsync<T>(string requestUri)
        {
            try
            {
                return await HandleResponse<T>(await http.GetAsync(requestUri));
            }
            catch (Exception ex)
            {
                if (ex is LuraphException)
                    throw;
                else
                    throw new LuraphException(ex); //internal exception wrapper
            }
        }

        private async Task<T> PostAsync<T>(string requestUri, object body)
        {
            try
            {
                return await HandleResponse<T>(await http.PostAsJsonAsync(requestUri, body));
            }
            catch (Exception ex)
            {
                throw new LuraphException(ex); //internal exception wrapper
            }
        }

        public Task<NodesResponse> GetNodes()
        {
            return GetAsync<NodesResponse>("obfuscate/nodes");
        }

        public Task<CreateJobResponse> CreateNewJob(string node, string script, string fileName, Dictionary<string, object> options, bool useTokens = false, bool enforceSettings = false)
        {
            return PostAsync<CreateJobResponse>("obfuscate/new", new
            {
                node,
                script = Convert.ToBase64String(Encoding.UTF8.GetBytes(script)),
                fileName,
                options,
                useTokens,
                enforceSettings
            });
        }

        public Task<ObfuscateStatusResponse> GetJobStatus(string jobId)
        {
            return GetAsync<ObfuscateStatusResponse>($"obfuscate/status/{jobId}");
        }

        public async Task<ObfuscateDownloadResponse> DownloadResult(string jobId)
        {
            var content = await GetAsync<HttpContent>($"obfuscate/download/{jobId}");
            return new ObfuscateDownloadResponse(content.Headers.ContentDisposition?.FileName?.Trim('"') ?? "script-obfuscated.lua", await content.ReadAsStreamAsync());
        }

        protected virtual void Dispose(bool disposing)
        {
            http.Dispose();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
