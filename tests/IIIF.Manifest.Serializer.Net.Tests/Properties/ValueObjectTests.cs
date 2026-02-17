namespace IIIF.Manifests.Serializer.Tests.Properties
{
    public class ValueObjectTests
    {
        [Fact]
        public void ImageFormat_StaticProperties_ShouldHaveCorrectValues()
        {
            ImageFormat.Jpg.Value.Should().Be("jpg");
            ImageFormat.Png.Value.Should().Be("png");
            ImageFormat.Webp.Value.Should().Be("webp");
            ImageFormat.Gif.Value.Should().Be("gif");
            ImageFormat.Tif.Value.Should().Be("tif");
            ImageFormat.Jp2.Value.Should().Be("jp2");
            ImageFormat.Pdf.Value.Should().Be("pdf");
            ImageFormat.Avif.Value.Should().Be("avif");
            ImageFormat.Heic.Value.Should().Be("heic");
        }

        [Fact]
        public void ImageFormat_ShouldAllowCustomValues()
        {
            var customFormat = new ImageFormat("custom-format");
            customFormat.Value.Should().Be("custom-format");
        }

        [Fact]
        public void ImageQuality_StaticProperties_ShouldHaveCorrectValues()
        {
            ImageQuality.Default.Value.Should().Be("default");
            ImageQuality.Color.Value.Should().Be("color");
            ImageQuality.Gray.Value.Should().Be("gray");
            ImageQuality.Bitonal.Value.Should().Be("bitonal");
        }

        [Fact]
        public void ImageFeature_StaticProperties_ShouldHaveCorrectValues()
        {
            ImageFeature.RegionByPx.Value.Should().Be("regionByPx");
            ImageFeature.RegionByPct.Value.Should().Be("regionByPct");
            ImageFeature.RegionSquare.Value.Should().Be("regionSquare");
            ImageFeature.SizeByH.Value.Should().Be("sizeByH");
            ImageFeature.SizeByW.Value.Should().Be("sizeByW");
            ImageFeature.SizeByWh.Value.Should().Be("sizeByWh");
            ImageFeature.RotationBy90s.Value.Should().Be("rotationBy90s");
            ImageFeature.RotationArbitrary.Value.Should().Be("rotationArbitrary");
            ImageFeature.Mirroring.Value.Should().Be("mirroring");
            ImageFeature.Cors.Value.Should().Be("cors");
        }

        [Fact]
        public void Behavior_StaticProperties_ShouldHaveCorrectValues()
        {
            Behavior.Paged.Value.Should().Be("paged");
            Behavior.Continuous.Value.Should().Be("continuous");
            Behavior.Individuals.Value.Should().Be("individuals");
            Behavior.FacingPages.Value.Should().Be("facing-pages");
            Behavior.NonPaged.Value.Should().Be("non-paged");
            Behavior.AutoAdvance.Value.Should().Be("auto-advance");
            Behavior.Hidden.Value.Should().Be("hidden");
            Behavior.MultiPart.Value.Should().Be("multi-part");
        }

        [Fact]
        public void Motivation_StaticProperties_ShouldHaveCorrectValues()
        {
            Motivation.Painting.Value.Should().Be("painting");
            Motivation.Supplementing.Value.Should().Be("supplementing");
            Motivation.Commenting.Value.Should().Be("commenting");
            Motivation.Describing.Value.Should().Be("describing");
            Motivation.Tagging.Value.Should().Be("tagging");
            Motivation.ScPainting.Value.Should().Be("sc:painting");
        }

        [Fact]
        public void ResourceType_StaticProperties_ShouldHaveCorrectValues()
        {
            ResourceType.Collection.Value.Should().Be("Collection");
            ResourceType.Manifest.Value.Should().Be("Manifest");
            ResourceType.Canvas.Value.Should().Be("Canvas");
            ResourceType.Range.Value.Should().Be("Range");
            ResourceType.Annotation.Value.Should().Be("Annotation");
            ResourceType.Image.Value.Should().Be("Image");
            ResourceType.Video.Value.Should().Be("Video");
            ResourceType.Sound.Value.Should().Be("Sound");
            ResourceType.ImageService3.Value.Should().Be("ImageService3");
        }

        [Fact]
        public void Profile_StaticProperties_ShouldHaveCorrectValues()
        {
            Profile.Level0.Value.Should().Be("level0");
            Profile.Level1.Value.Should().Be("level1");
            Profile.Level2.Value.Should().Be("level2");
            Profile.ImageApi2Level2.Value.Should().Be("http://iiif.io/api/image/2/level2.json");
        }

        [Fact]
        public void Context_StaticProperties_ShouldHaveCorrectValues()
        {
            Context.Presentation2.Value.Should().Be("http://iiif.io/api/presentation/2/context.json");
            Context.Presentation3.Value.Should().Be("http://iiif.io/api/presentation/3/context.json");
            Context.Image2.Value.Should().Be("http://iiif.io/api/image/2/context.json");
            Context.Image3.Value.Should().Be("http://iiif.io/api/image/3/context.json");
            Context.Auth2.Value.Should().Be("http://iiif.io/api/auth/2/context.json");
            Context.Search2.Value.Should().Be("http://iiif.io/api/search/2/context.json");
        }

        [Fact]
        public void Rights_StaticProperties_ShouldHaveCorrectValues()
        {
            Rights.CcBy.Value.Should().Be("http://creativecommons.org/licenses/by/4.0/");
            Rights.CcBySa.Value.Should().Be("http://creativecommons.org/licenses/by-sa/4.0/");
            Rights.Cc0.Value.Should().Be("http://creativecommons.org/publicdomain/zero/1.0/");
            Rights.PublicDomain.Value.Should().Be("http://creativecommons.org/publicdomain/mark/1.0/");
            Rights.InCopyright.Value.Should().Be("http://rightsstatements.org/vocab/InC/1.0/");
        }

        [Fact]
        public void Language_StaticProperties_ShouldHaveCorrectValues()
        {
            Language.English.Value.Should().Be("en");
            Language.EnglishUs.Value.Should().Be("en-US");
            Language.French.Value.Should().Be("fr");
            Language.German.Value.Should().Be("de");
            Language.Japanese.Value.Should().Be("ja");
            Language.None.Value.Should().Be("none");
        }

        [Fact]
        public void TimeMode_StaticProperties_ShouldHaveCorrectValues()
        {
            TimeMode.Trim.Value.Should().Be("trim");
            TimeMode.Scale.Value.Should().Be("scale");
            TimeMode.Loop.Value.Should().Be("loop");
        }

        [Fact]
        public void ViewingDirection_StaticProperties_ShouldHaveCorrectValues()
        {
            ViewingDirection.Ltr.Value.Should().Be("left-to-right");
            ViewingDirection.Rtl.Value.Should().Be("right-to-left");
            ViewingDirection.Ttb.Value.Should().Be("top-to-bottom");
            ViewingDirection.Btt.Value.Should().Be("bottom-to-top");
        }

        [Fact]
        public void ViewingHint_StaticProperties_ShouldHaveCorrectValues()
        {
            ViewingHint.Paged.Value.Should().Be("paged");
            ViewingHint.Continuous.Value.Should().Be("continuous");
            ViewingHint.Individuals.Value.Should().Be("individuals");
            ViewingHint.FacingPages.Value.Should().Be("facing-pages");
            ViewingHint.NonPaged.Value.Should().Be("non-paged");
            ViewingHint.Top.Value.Should().Be("top");
            ViewingHint.MultiPart.Value.Should().Be("multi-part");
        }

        [Fact]
        public void ValueObjects_ShouldSerializeCorrectly()
        {
            var format = ImageFormat.Webp;
            var json = JsonConvert.SerializeObject(format);
            json.Should().Be("\"webp\"");
        }

        [Fact]
        public void ValueObjects_ShouldDeserializeCorrectly()
        {
            var json = "\"jpg\"";
            var format = JsonConvert.DeserializeObject<ImageFormat>(json);
            format.Value.Should().Be("jpg");
        }
    }
}

