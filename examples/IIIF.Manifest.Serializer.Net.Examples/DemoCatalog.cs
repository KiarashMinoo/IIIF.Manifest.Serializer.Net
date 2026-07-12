using System;
using System.Collections.Generic;
using IIIF.Manifests.Serializer.Extensions;
using IIIF.Manifests.Serializer.Nodes;
using IIIF.Manifests.Serializer.Nodes.Contents.Annotation;
using IIIF.Manifests.Serializer.Nodes.Contents.Image.Resource;
using IIIF.Manifests.Serializer.Nodes.Contents.Segment.Resource;
using IIIF.Manifests.Serializer.Properties;
using IIIF.Manifests.Serializer.Properties.Services;
using IIIF.Manifests.Serializer.Properties.Services.Auth2;
using IIIF.Manifests.Serializer.Shared.Content.Resources;

namespace IIIF.Manifests.Serializer.Examples;

public sealed record ExampleDefinition(string Title, Func<object> Build);

public static class DemoCatalog
{
    public static IReadOnlyList<ExampleDefinition> GetAll()
    {
        return
        [
            new ExampleDefinition("Wellcome-style search and access manifest", CreateSearchManifest),
            new ExampleDefinition("Nationalmuseum-style deep zoom manifest", CreateDeepZoomManifest),
            new ExampleDefinition("Stanford-style paged book manifest", CreatePagedBookManifest),
            new ExampleDefinition("Biblissima-style collection browsing", CreateCollectionExample),
            new ExampleDefinition("Harvard-style canvas annotation example", CreateAnnotationExample),
            new ExampleDefinition("Map-based place example", CreateMapExample)
        ];
    }

    private static Manifest CreateSearchManifest()
    {
        var manifest = new Manifest("https://example.org/demo/search/manifest", new Label("Search Demo"));
        var canvas = new Canvas("https://example.org/demo/search/canvas/1", new Label("Page 1"), 1200, 900);
        canvas.AddAnnotation(new Annotation(
            "https://example.org/demo/search/annotation/1",
            new ImageResource("https://example.org/demo/search/image.jpg", "image/jpeg").SetHeight(1200).SetWidth(900),
            canvas.Id));
        manifest.AddItem(canvas);
        manifest.SetSequenceId("https://example.org/demo/search/sequence/normal");
        manifest.AddService(new SearchService("http://iiif.io/api/search/2/context.json", "https://example.org/demo/search/service", "http://iiif.io/api/search/0/search").AddService(new AutoCompleteService("http://iiif.io/api/search/2/context.json",
            "https://example.org/demo/search/autocomplete", "http://iiif.io/api/search/0/autocomplete")));
        manifest.AddService(new ContentStateService("http://iiif.io/api/content-state/1/context.json", "https://example.org/demo/content-state", "http://iiif.io/api/content-state/v1/state"));
        var accessTokenService = new AuthAccessTokenService2("https://example.org/demo/auth/token");
        var accessService = new AuthAccessService2("https://example.org/demo/auth/login", "active", accessTokenService)
            .SetLabel("Login").SetHeading("Sign in").SetNote("Protected content").SetConfirmLabel("Continue")
            .SetLogoutService(new AuthLogoutService2("https://example.org/demo/auth/logout", "Logout"));
        manifest.AddService(new AuthProbeService2("https://example.org/demo/auth/probe", accessService));
        return manifest;
    }

    private static Manifest CreateDeepZoomManifest()
    {
        var manifest = new Manifest("https://example.org/demo/deep-zoom/manifest", new Label("Deep Zoom Demo"));
        manifest.AddHomepage(new Homepage("https://example.org/demo/object", "Object page"));
        manifest.SetThumbnail(new Thumbnail("https://example.org/demo/deep-zoom/thumb.jpg"));
        manifest.AddProvider(new Provider("https://example.org/demo/institution", "Museum"));

        var canvas = new Canvas("https://example.org/demo/deep-zoom/canvas/1", new Label("Image"), 4000, 3000);
        var imageResource = new ImageResource("https://example.org/demo/deep-zoom/image/full/full/0/default.jpg", "image/jpeg").SetHeight(4000).SetWidth(3000);
        imageResource.AddService(new Service("http://iiif.io/api/image/2/context.json", "https://example.org/demo/image-service", "http://iiif.io/api/image/2/level2.json").SetHeight(4000).SetWidth(3000));
        canvas.AddAnnotation(new Annotation("https://example.org/demo/deep-zoom/annotation/1", imageResource, canvas.Id));
        manifest.AddItem(canvas);
        manifest.SetSequenceId("https://example.org/demo/deep-zoom/sequence/normal");
        return manifest;
    }

    private static Manifest CreatePagedBookManifest()
    {
        var manifest = new Manifest("https://example.org/demo/book/manifest", new Label("Paged Book Demo"));
        manifest.SetViewingDirection(ViewingDirection.Ltr);
        manifest.AddBehavior(Behavior.Paged);
        manifest.SetStart("https://example.org/demo/book/canvas/2");
        manifest.SetSequenceId("https://example.org/demo/book/sequence/normal");

        for (var page = 1; page <= 4; page++)
        {
            var canvas = new Canvas($"https://example.org/demo/book/canvas/{page}", new Label($"Page {page}"), 2000, 1400);
            canvas.AddAnnotation(new Annotation($"https://example.org/demo/book/annotation/{page}", new ImageResource($"https://example.org/demo/book/page-{page}.jpg", "image/jpeg").SetHeight(2000).SetWidth(1400), canvas.Id));
            manifest.AddItem(canvas);
        }

        return manifest;
    }

    private static Collection CreateCollectionExample()
    {
        var collection = new Collection("https://example.org/demo/collection", new Label("Collection Demo"));
        collection.AddBehavior(Behavior.Individuals);
        collection.AddManifestReference("https://example.org/demo/book/manifest");
        collection.AddManifestReference("https://example.org/demo/search/manifest");
        collection.AddItem(new Collection("https://example.org/demo/collection/sub", new Label("Sub collection")));
        return collection;
    }

    private static AnnotationList CreateAnnotationExample()
    {
        var annotationList = new AnnotationList("https://example.org/demo/annotations/list", new Label("Annotations"));
        annotationList.AddResource(new SegmentResource("https://example.org/demo/annotations/1", ResourceType.Image).SetFull(new BaseResource("https://example.org/demo/annotations/1/full", ResourceType.Image)));
        return annotationList;
    }

    private static Manifest CreateMapExample()
    {
        var manifest = new Manifest("https://example.org/demo/map/manifest", new Label("Map Demo"));
        manifest.SetNavPlace(new NavPlace("https://example.org/demo/map/navplace").AddFeature(new Feature("https://example.org/demo/map/feature/1").SetGeometry(new Geometry(GeometryType.Point).AddCoordinate(new CoordinateItem(-73.9857, 40.7484)))
            .SetProperties(new FeatureProperties().AddLabel(new Label("New York")))));
        manifest.AddItem(new Canvas("https://example.org/demo/map/canvas/1", new Label("Map"), 1000, 1000));
        manifest.SetSequenceId("https://example.org/demo/map/sequence/normal");
        return manifest;
    }
}