using System;
using IIIF.Manifests.Serializer.Helpers;
using IIIF.Manifests.Serializer.Shared.Exceptions;
using IIIF.Manifests.Serializer.Shared.Service;
using IIIF.Manifests.Serializer.Shared.Trackable;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace IIIF.Manifests.Serializer.Shared.BaseItem
{
    public class BaseItemJsonConverter<TBaseItem> : TrackableObjectJsonConverter<TBaseItem>
        where TBaseItem : BaseItem<TBaseItem>
    {
        protected bool DisableTypeChecking { get; set; }

        private TBaseItem SetType(JToken element, TBaseItem baseItem)
        {
            var jType = element.TryGetToken(BaseItem<TBaseItem>.TypeJName);

            if (jType != null)
                baseItem.SetType(jType.ToString());
            else if (!DisableTypeChecking)
                throw new JsonNodeRequiredException<TBaseItem>(BaseItem<TBaseItem>.TypeJName);

            return baseItem;
        }

        private TBaseItem SetService(JToken element, TBaseItem baseItem)
        {
            var jService = element.TryGetToken(BaseItem<TBaseItem>.ServiceJName);
            if (jService != null)
            {
                IBaseService? service = null;

                if (jService is JArray serviceArray)
                {
                    // Handle array of services
                    foreach (var serviceToken in serviceArray)
                    {
                        var detectedService = DetectAndDeserializeService(serviceToken);
                        if (detectedService != null)
                        {
                            // For now, set the first service. In future, we might want to support multiple services
                            service = detectedService;
                            break;
                        }
                    }
                }
                else if (jService is JObject serviceObject)
                {
                    service = DetectAndDeserializeService(serviceObject);
                }

                if (service != null)
                {
                    baseItem.SetService(service);
                }
            }

            return baseItem;
        }

        private IBaseService? DetectAndDeserializeService(JToken serviceToken)
        {
            // Check the @type field to determine service type
            var jType = serviceToken.TryGetToken(BaseItem<TBaseItem>.TypeJName);
            if (jType != null)
            {
                var typeValue = jType.ToString();
                switch (typeValue)
                {
                    case "ImageService2":
                    case "ImageService3":
                        return serviceToken.ToObject<Properties.ServiceProperty.Service>();
                    case "AuthCookieService1":
                    case "AuthTokenService1":
                    case "AuthLogoutService1":
                        return serviceToken.ToObject<Properties.ServiceProperty.AuthService1>();
                    case "AuthProbeService2":
                    case "AuthAccessService2":
                    case "AuthAccessTokenService2":
                    case "AuthLogoutService2":
                        return serviceToken.ToObject<Properties.ServiceProperty.AuthService2>();
                    case "SearchService2":
                        return serviceToken.ToObject<Properties.ServiceProperty.SearchService>();
                    case "AutoCompleteService2":
                        return serviceToken.ToObject<Properties.ServiceProperty.AutoCompleteService>();
                    case "OrderedCollection":
                        return serviceToken.ToObject<Properties.ServiceProperty.DiscoveryService>();
                    case "ContentStateService":
                        return serviceToken.ToObject<Properties.ServiceProperty.ContentStateService>();
                    default:
                        // Try to detect by profile or other means
                        var jProfile = serviceToken.TryGetToken("profile");
                        if (jProfile != null)
                        {
                            var profileValue = jProfile.ToString();
                            if (profileValue.Contains("auth"))
                            {
                                // Try Auth services
                                try
                                {
                                    return serviceToken.ToObject<Properties.ServiceProperty.AuthService1>();
                                }
                                catch
                                {
                                    return serviceToken.ToObject<Properties.ServiceProperty.AuthService2>();
                                }
                            }
                            else if (profileValue.Contains("search"))
                            {
                                return serviceToken.ToObject<Properties.ServiceProperty.SearchService>();
                            }
                            else if (profileValue.Contains("discovery"))
                            {
                                return serviceToken.ToObject<Properties.ServiceProperty.DiscoveryService>();
                            }
                            else if (profileValue.Contains("content-state"))
                            {
                                return serviceToken.ToObject<Properties.ServiceProperty.ContentStateService>();
                            }
                            else if (profileValue.Contains("image"))
                            {
                                return serviceToken.ToObject<Properties.ServiceProperty.Service>();
                            }
                        }

                        break;
                }
            }

            // Fallback: try to deserialize as the most common service type
            try
            {
                return serviceToken.ToObject<Properties.ServiceProperty.Service>();
            }
            catch
            {
                // If that fails, return null
                return null;
            }
        }

        protected override TBaseItem CreateInstance(JToken element, Type objectType, TBaseItem? existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            if (element is JObject)
            {
                var jId = element.TryGetToken(BaseItem<TBaseItem>.IdJName);
                if (jId is null)
                    throw new JsonNodeRequiredException<TBaseItem>(BaseItem<TBaseItem>.IdJName);

                return (TBaseItem)Activator.CreateInstance(typeof(TBaseItem), jId.ToString());
            }
            else
                return (TBaseItem)Activator.CreateInstance(typeof(TBaseItem), element.Value<string>());
        }

        protected override TBaseItem EnrichReadJson(TBaseItem item, JToken element, Type objectType, TBaseItem? existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            item = SetType(element, item);
            item = SetService(element, item);
            return item;
        }

        protected virtual void EnrichMoreWriteJson(JsonWriter writer, TBaseItem value, JsonSerializer serializer)
        {
        }

        protected sealed override void EnrichWriteJson(JsonWriter writer, TBaseItem value, JsonSerializer serializer)
        {
            if (!string.IsNullOrEmpty(value.Context))
            {
                writer.WritePropertyName(BaseItem<TBaseItem>.ContextJName);
                writer.WriteValue(value.Context);
            }

            if (!string.IsNullOrEmpty(value.Id))
            {
                writer.WritePropertyName(BaseItem<TBaseItem>.IdJName);
                writer.WriteValue(value.Id);
            }

            if (!string.IsNullOrEmpty(value.Type))
            {
                writer.WritePropertyName(BaseItem<TBaseItem>.TypeJName);
                writer.WriteValue(value.Type);
            }

            if (value.Service != null)
            {
                writer.WritePropertyName(BaseItem<TBaseItem>.ServiceJName);
                serializer.Serialize(writer, value.Service);
            }

            EnrichMoreWriteJson(writer, value, serializer);
        }
    }
}