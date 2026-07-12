using System.Linq;
using System.Reflection;
using IIIF.Manifests.Serializer.Attributes;
using IIIF.Manifests.Serializer.Nodes;
using IIIF.Manifests.Serializer.Nodes.Contents.Annotation;
using IIIF.Manifests.Serializer.Nodes.Contents.Audio;
using IIIF.Manifests.Serializer.Nodes.Contents.Audio.Resource;
using IIIF.Manifests.Serializer.Nodes.Contents.Image;
using IIIF.Manifests.Serializer.Nodes.Contents.Image.Resource;
using IIIF.Manifests.Serializer.Nodes.Contents.OtherContent;
using IIIF.Manifests.Serializer.Nodes.Contents.Video;
using IIIF.Manifests.Serializer.Nodes.Contents.Video.Resource;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace IIIF.Manifests.Serializer.Tests;

/// <summary>
/// Milestone 1 (SDK_VERSIONING_GUIDE.md): Canvas reshaped around 3.0-native Items
/// (AnnotationPage/Annotation), with Images/Audios/Videos/OtherContents as computed
/// legacy views and obsolete-error legacy mutators.
/// </summary>
public class CanvasReshapeTests
{
    private static Canvas CreateCanvasWithLegacyImage(out ImageResource resource)
    {
        var canvas = new Canvas("https://example.org/canvas/1", new Label("Page 1"), 1000, 800);
        resource = new ImageResource("https://example.org/image.png", "image/png").SetHeight(1000).SetWidth(800);
        canvas.AddAnnotation(new Annotation("https://example.org/annotation/1", resource, canvas.Id));
        return canvas;
    }

    [Fact]
    public void AddAnnotation_Should_PopulateItemsAsThePrimary3_0Storage()
    {
        var canvas = CreateCanvasWithLegacyImage(out _);

        canvas.Items.Should().ContainSingle();
        var page = canvas.Items.OfType<AnnotationPage>().Single();
        page.Id.Should().Be($"{canvas.Id}/page");
        page.Items.OfType<Annotation>().Should().ContainSingle();
    }

    [Fact]
    public void Images_Should_BeAComputedLegacyView_ReflectingItems()
    {
        var canvas = CreateCanvasWithLegacyImage(out var resource);

        canvas.Images.Should().ContainSingle();
        var image = canvas.Images.First();
        image.Id.Should().Be("https://example.org/annotation/1");
        image.On.Should().Be(canvas.Id);
        image.Resource.Id.Should().Be(resource.Id);
        image.Motivation.Should().Be("sc:painting");
    }

    [Fact]
    public void ImagesAudiosVideos_Should_CoexistOnTheSameCanvas_WithoutClobberingEachOther()
    {
        var canvas = new Canvas("https://example.org/canvas/1", new Label("Mixed"), 1000, 800);

        var image = new Annotation("https://example.org/annotation/image", new ImageResource("https://example.org/image.png", "image/png"), canvas.Id);
        var audio = new Annotation("https://example.org/annotation/audio", new AudioResource("https://example.org/audio.mp4", "audio/mp4"), canvas.Id);
        var video = new Annotation("https://example.org/annotation/video", new VideoResource("https://example.org/video.mp4", "video/mp4"), canvas.Id);

        canvas.AddAnnotation(image);
        canvas.AddAnnotation(audio);
        canvas.AddAnnotation(video);

        canvas.Images.Should().ContainSingle(x => x.Id == "https://example.org/annotation/image");
        canvas.Audios.Should().ContainSingle(x => x.Id == "https://example.org/annotation/audio");
        canvas.Videos.Should().ContainSingle(x => x.Id == "https://example.org/annotation/video");

        var page = canvas.Items.OfType<AnnotationPage>().Single();
        page.Items.Should().HaveCount(3);
    }

    [Fact]
    public void LegacyImageJson_Should_DeserializeDirectlyIntoItems()
    {
        const string json = """
                            {
                              "@id": "https://example.org/canvas/1",
                              "@type": "sc:Canvas",
                              "label": "Page 1",
                              "height": 1000,
                              "width": 800,
                              "images": [
                                {
                                  "@id": "https://example.org/annotation/1",
                                  "@type": "oa:Annotation",
                                  "motivation": "sc:painting",
                                  "on": "https://example.org/canvas/1",
                                  "resource": {
                                    "@id": "https://example.org/image.png",
                                    "@type": "dctypes:Image",
                                    "format": "image/png",
                                    "height": 1000,
                                    "width": 800
                                  }
                                }
                              ]
                            }
                            """;

        var canvas = JsonConvert.DeserializeObject<Canvas>(json)!;

        // The legacy view getter reflects the same data...
        canvas.Images.Should().ContainSingle();
        canvas.Images.First().Resource.Format.Should().Be("image/png");

        // ...because deserializing the legacy document wrote directly into 3.0-native Items.
        canvas.Items.OfType<AnnotationPage>().Single().Items.OfType<Annotation>().Should().ContainSingle();
    }

    [Fact]
    public void Canvas_Should_NotLeak3_0ItemsIntoLegacyV2Json()
    {
        var canvas = CreateCanvasWithLegacyImage(out _);

        var json = JsonConvert.SerializeObject(canvas);
        var obj = JObject.Parse(json);

        obj["images"].Should().NotBeNull();
        obj["items"].Should().BeNull("Items is 3.0-native storage and must never leak into legacy JSON via reflection-based serialization");
    }

    [Fact]
    public void SettingImagesToEmpty_Should_NotLeaveADanglingEmptyAnnotationPage()
    {
        const string json = """
                            {
                              "@id": "https://example.org/canvas/1",
                              "@type": "sc:Canvas",
                              "label": "Page 1",
                              "height": 1000,
                              "width": 800,
                              "images": []
                            }
                            """;

        var canvas = JsonConvert.DeserializeObject<Canvas>(json)!;

        canvas.Images.Should().BeEmpty();
        canvas.Items.Should().BeEmpty();
    }

    [Fact]
    public void OtherContent_Should_MapToAnnotationsAndBackAsAnnotationPageReferences()
    {
        var canvas = new Canvas("https://example.org/canvas/1", new Label("Page 1"), 1000, 800);

        canvas.AddAnnotationPageReference(new AnnotationPage("https://example.org/annotation-list/transcript"));

        canvas.Annotations.Should().ContainSingle(x => x.Id == "https://example.org/annotation-list/transcript");
        canvas.OtherContents.Should().ContainSingle();
        canvas.OtherContents.First().Id.Should().Be("https://example.org/annotation-list/transcript");
    }

    [Fact]
    public void LegacyOtherContentJson_Should_DeserializeIntoAnnotations()
    {
        const string json = """
                            {
                              "@id": "https://example.org/canvas/1",
                              "@type": "sc:Canvas",
                              "label": "Page 1",
                              "height": 1000,
                              "width": 800,
                              "otherContent": [
                                { "@id": "https://example.org/annotation-list/transcript", "@type": "sc:AnnotationList" }
                              ]
                            }
                            """;

        var canvas = JsonConvert.DeserializeObject<Canvas>(json)!;

        canvas.OtherContents.Should().ContainSingle();
        canvas.Annotations.Should().ContainSingle(x => x.Id == "https://example.org/annotation-list/transcript");
    }

    [Theory]
    [InlineData(nameof(Canvas.AddImage))]
    [InlineData(nameof(Canvas.AddAudio))]
    [InlineData(nameof(Canvas.AddVideo))]
    [InlineData(nameof(Canvas.AddOtherContent))]
    public void LegacyMutators_Should_BeMarkedObsoleteAsWarnings(string methodName)
    {
        var method = typeof(Canvas).GetMethod(methodName, BindingFlags.Instance | BindingFlags.Public);

        method.Should().NotBeNull();
        var obsolete = method!.GetCustomAttribute<System.ObsoleteAttribute>();
        obsolete.Should().NotBeNull("legacy mutators must be tagged obsolete per the versioning guide");
        obsolete!.IsError.Should().BeFalse("legacy mutators remain callable - deprecated with a warning, not a compile-time error");
    }

    [Theory]
    [InlineData(nameof(Canvas.AddImage))]
    [InlineData(nameof(Canvas.AddAudio))]
    [InlineData(nameof(Canvas.AddVideo))]
    [InlineData(nameof(Canvas.AddOtherContent))]
    public void LegacyMutators_Should_CarryIIIFVersionAttribute_DescribingTheDeprecation(string methodName)
    {
        var method = typeof(Canvas).GetMethod(methodName, BindingFlags.Instance | BindingFlags.Public);

        method.Should().NotBeNull();
        var version = method!.GetCustomAttribute<IIIFVersionAttribute>();
        version.Should().NotBeNull("legacy mutators must document their deprecation via an IIIFVersionAttribute-derived attribute");
        version!.IsDeprecated.Should().BeTrue();
        version.DeprecatedInVersion.Should().Be("3.0");
        version.ReplacedBy.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public void AddAnnotation_Should_NotBeObsolete()
    {
        var method = typeof(Canvas).GetMethod(nameof(Canvas.AddAnnotation), BindingFlags.Instance | BindingFlags.Public);

        method.Should().NotBeNull();
        method!.GetCustomAttribute<System.ObsoleteAttribute>().Should().BeNull("the 3.0-preferred write API must remain warning-free");
    }

    [Fact]
    public void ImagesGetter_Should_NotBeObsolete()
    {
        var property = typeof(Canvas).GetProperty(nameof(Canvas.Images));

        property.Should().NotBeNull();
        property!.GetCustomAttribute<System.ObsoleteAttribute>().Should()
            .BeNull("legacy getters must stay warning-free so reading a parsed legacy document produces no compiler noise");
    }

    [Theory]
    [InlineData(nameof(Canvas.Images))]
    [InlineData(nameof(Canvas.Audios))]
    [InlineData(nameof(Canvas.Videos))]
    public void LegacyGetters_Should_CarryIIIFVersionAttribute_DescribingTheDeprecation(string propertyName)
    {
        var property = typeof(Canvas).GetProperty(propertyName);

        property.Should().NotBeNull();
        var version = property!.GetCustomAttribute<IIIFVersionAttribute>();
        version.Should().NotBeNull($"{propertyName} is a computed legacy view and must document its replacement");
        version!.IsDeprecated.Should().BeTrue();
        version.ReplacedBy.Should().Be("items");
    }
}
