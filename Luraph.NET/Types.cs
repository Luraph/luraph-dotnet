using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json.Serialization;

namespace Luraph.APITypes
{
    public readonly struct LuraphOption
    {
        [JsonPropertyName("name")]
        public readonly string Name;

        [JsonPropertyName("description")]
        public readonly string Description;

        [JsonPropertyName("tier")]
        public readonly string Tier;

        [JsonPropertyName("type")]
        public readonly string Type;

        [JsonPropertyName("choices")]
        public readonly string[] Choices;

        [JsonConstructor]
        public LuraphOption(string name, string description, string tier, string type, string[] choices) => (Name, Description, Tier, Type, Choices) = (name, description, tier, type, choices);
    }

    public readonly struct LuraphNode
    {
        [JsonPropertyName("cpuUsage")]
        public readonly double CpuUsage;

        [JsonPropertyName("options")]
        public readonly Dictionary<string, LuraphOption> Options;

        [JsonConstructor]
        public LuraphNode(double cpuUsage, Dictionary<string, LuraphOption> options) => (CpuUsage, Options) = (cpuUsage, options);
    }

    public readonly struct LuraphError
    {
        [JsonPropertyName("param")]
        public readonly string? Param;

        [JsonPropertyName("message")]
        public readonly string Message;

        [JsonConstructor]
        public LuraphError(string? param, string message) => (Param, Message) = (param, message);
    }

    public readonly struct NodesResponse
    {
        [JsonPropertyName("recommendedId")]
        public readonly string? RecommendedId;

        [JsonPropertyName("nodes")]
        public readonly Dictionary<string, LuraphNode> Nodes;

        [JsonConstructor]
        public NodesResponse(string recommendedId, Dictionary<string, LuraphNode> nodes) => (RecommendedId, Nodes) = (recommendedId, nodes);
    }

    public readonly struct CreateJobResponse
    {
        [JsonPropertyName("jobId")]
        public readonly string JobId;

        [JsonConstructor]
        public CreateJobResponse(string jobId) => JobId = jobId;
    }

    public readonly struct ObfuscateStatusResponse
    {
        [JsonIgnore]
        public readonly bool Success { get => Error == null; }

        [JsonPropertyName("error")]
        public readonly string? Error;

        [JsonConstructor]
        public ObfuscateStatusResponse(string? error) => Error = error;
    }

    public readonly struct ObfuscateDownloadResponse
    {
        public readonly string FileName;

        public readonly Stream Data;

        public ObfuscateDownloadResponse(string fileName, Stream data) => (FileName, Data) = (fileName, data);
    }

    public readonly struct LuraphWarnings
    {
        [JsonPropertyName("warnings")]
        public readonly string[] Warnings;

        [JsonConstructor]
        public LuraphWarnings(string[] warnings) => Warnings = warnings;
    }

    public class LuraphException : Exception
    {
        public readonly bool IsHttpError;

        [JsonPropertyName("errors")]
        public readonly LuraphError[] Errors;

        internal LuraphException(int statusCode) : this(new LuraphError[]
        {
                new LuraphError(null, $"API returned status code ${statusCode}")
        })
        {
            IsHttpError = true;
        }

        internal LuraphException(Exception cause) : this(new LuraphError[] {
                    new LuraphError(null, "Internal exception while sending request")
            }, cause)
        {
            IsHttpError = true;
        }

        internal LuraphException(
            LuraphError[] errors, Exception? cause = null) : base(GenerateErrorMessage(errors), cause) => Errors = errors;

        [JsonConstructor]
        public LuraphException(LuraphError[] errors) : this(errors, null) { }

        private static string GenerateErrorMessage(LuraphError[] errors)
        {
            return string.Join(" | ", errors.Select(error =>
                (error.Param != null ? $"({error.Param})" : "") + error.Message));
        }
    }
}