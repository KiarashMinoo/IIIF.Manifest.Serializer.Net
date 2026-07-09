namespace IIIF.Manifests.Serializer.Net.Cookbook;

class Program
{
    static void Main(string[] args)
    {
        try
        {
            var examples = new ICookbookExample[]
            {
                new Recipe0001SimplestImageExample(),
                new Recipe0002SimplestAudioExample()
            };

            foreach (var example in examples)
            {
                example.Run();
                Console.WriteLine();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
            Console.WriteLine($"Stack Trace: {ex.StackTrace}");
            if (ex.InnerException != null)
            {
                Console.WriteLine($"Inner Exception: {ex.InnerException.Message}");
                Console.WriteLine($"Inner Stack Trace: {ex.InnerException.StackTrace}");
            }
        }
    }
}