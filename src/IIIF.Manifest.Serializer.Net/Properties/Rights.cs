using IIIF.Manifests.Serializer.Attributes;
using IIIF.Manifests.Serializer.Shared.ValuableItem;
using Newtonsoft.Json;

namespace IIIF.Manifests.Serializer.Properties
{
    /// <summary>
    /// Standard rights statement URLs (Creative Commons, RightsStatements.org).
    /// Can be used directly with static properties or by creating custom values.
    /// </summary>
    [PresentationAPI("2.0", Notes = "In 2.x use 'license', in 3.0 use 'rights'. Values are the same.")]
    [JsonConverter(typeof(ValuableItemJsonConverter<Rights>))]
    public class Rights : ValuableItem<Rights>
    {
        public Rights(string value) : base(value)
        {
        }

        // Creative Commons 4.0
        public static Rights CcBy => new Rights("http://creativecommons.org/licenses/by/4.0/");
        public static Rights CcBySa => new Rights("http://creativecommons.org/licenses/by-sa/4.0/");
        public static Rights CcByNd => new Rights("http://creativecommons.org/licenses/by-nd/4.0/");
        public static Rights CcByNc => new Rights("http://creativecommons.org/licenses/by-nc/4.0/");
        public static Rights CcByNcSa => new Rights("http://creativecommons.org/licenses/by-nc-sa/4.0/");
        public static Rights CcByNcNd => new Rights("http://creativecommons.org/licenses/by-nc-nd/4.0/");
        public static Rights Cc0 => new Rights("http://creativecommons.org/publicdomain/zero/1.0/");
        public static Rights PublicDomain => new Rights("http://creativecommons.org/publicdomain/mark/1.0/");

        // RightsStatements.org
        public static Rights InCopyright => new Rights("http://rightsstatements.org/vocab/InC/1.0/");
        public static Rights InCopyrightEuOrphanWork => new Rights("http://rightsstatements.org/vocab/InC-OW-EU/1.0/");
        public static Rights InCopyrightEducationalUse => new Rights("http://rightsstatements.org/vocab/InC-EDU/1.0/");
        public static Rights InCopyrightNonCommercialUse => new Rights("http://rightsstatements.org/vocab/InC-NC/1.0/");
        public static Rights InCopyrightRightsHolderUnlocatable => new Rights("http://rightsstatements.org/vocab/InC-RUU/1.0/");
        public static Rights NoKnownCopyright => new Rights("http://rightsstatements.org/vocab/NKC/1.0/");
        public static Rights NoCopyrightContractualRestrictions => new Rights("http://rightsstatements.org/vocab/NoC-CR/1.0/");
        public static Rights NoCopyrightNonCommercialUseOnly => new Rights("http://rightsstatements.org/vocab/NoC-NC/1.0/");
        public static Rights NoCopyrightOtherKnownLegalRestrictions => new Rights("http://rightsstatements.org/vocab/NoC-OKLR/1.0/");
        public static Rights NoCopyrightUnitedStates => new Rights("http://rightsstatements.org/vocab/NoC-US/1.0/");
        public static Rights CopyrightNotEvaluated => new Rights("http://rightsstatements.org/vocab/CNE/1.0/");
        public static Rights CopyrightUndetermined => new Rights("http://rightsstatements.org/vocab/UND/1.0/");
    }
}
