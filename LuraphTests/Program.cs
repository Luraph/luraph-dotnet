using Luraph;

var api = new LuraphAPI(Environment.GetEnvironmentVariable("LPH_API_KEY") ?? "");

var nodes = await api.GetNodes();

var nodeList = nodes.Nodes;
var nodeName = nodes.RecommendedId;
if (nodeName == null)
    nodeName = nodeList.Keys.First();
var node = nodeList[nodeName];

Console.WriteLine($"Recommended Node: {nodeName}");

Console.WriteLine($"- CPU Usage: {node.CpuUsage}");
Console.WriteLine("- Options:");
foreach(var entry in node.Options)
{
    var optionId = entry.Key;
    var optionInfo = entry.Value;

    Console.WriteLine($"  * {optionId} - {optionInfo.Name}:");
    Console.WriteLine($"  |- Description: {optionInfo.Description}");
    Console.WriteLine($"  |- Type: {optionInfo.Type}");
    Console.WriteLine($"  |- Tier: {optionInfo.Tier}");
    Console.WriteLine($"  |- Choices: [{string.Join(", ", optionInfo.Choices)}]");
    Console.WriteLine();
}

var options = new Dictionary<string, object>();
var jobId = (await api.CreateNewJob(nodeName, "print'Hello World!'", "hello-world.txt", options)).JobId;
Console.WriteLine($"Job ID: {jobId}");

var status = await api.GetJobStatus(jobId);
Console.WriteLine($"Job finished {(status.Success ? "successfully" : "unsuccessfully")}");

if(status.Success)
{
    var download = await api.DownloadResult(jobId);
    Console.WriteLine($"Result Filename: {download.FileName}");
    using (var stream = new StreamReader(download.Data))
    {
        Console.WriteLine($"First line of file: {stream.ReadLine()}");
    }
}
else
{
    Console.WriteLine($"Error: {status.Error}");
}