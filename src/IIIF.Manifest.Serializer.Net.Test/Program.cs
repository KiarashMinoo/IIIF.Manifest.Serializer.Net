using IIIF.Manifests.Serializer.Nodes;
using JsonDiffPatchDotNet;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Diagnostics;

var manifestStringLocation = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "USA-congress-manifest.json");
var manifestString = File.ReadAllText(manifestStringLocation);

var benhmark = new Stopwatch();
benhmark.Start();
var manifest = JsonConvert.DeserializeObject<Manifest>(manifestString);
benhmark.Stop();
Console.WriteLine(benhmark.Elapsed);

benhmark.Restart();
var manifestStr = JsonConvert.SerializeObject(manifest);
benhmark.Stop();
Console.WriteLine(benhmark.Elapsed);

var sr = JToken.Parse(manifestString);
var ds = JToken.Parse(manifestStr);
var isEqual = JToken.DeepEquals(sr, ds);
Console.WriteLine(isEqual);

if (!isEqual)
{
    var jdp = new JsonDiffPatch();
    JToken patch = jdp.Diff(sr, ds);
    Console.WriteLine(patch.ToString());
}

Console.ReadLine();