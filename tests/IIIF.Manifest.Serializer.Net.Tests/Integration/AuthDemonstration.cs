using System;
using IIIF.Manifests.Serializer.Properties;
using IIIF.Manifests.Serializer.Properties.Service;
using Newtonsoft.Json;

namespace IIIF.Manifest.Serializer.Net.Tests.Integration
{
    /// <summary>
    /// Simple demonstration of Auth API usage - not a unit test.
    /// Shows Auth 1.0 and Auth 2.0 patterns with full JSON output.
    /// </summary>
    public class AuthDemonstration
    {
        public static void Main()
        {
            Console.WriteLine("=== IIIF Authentication API Demonstration ===\n");

            DemonstrateAuth1();
            Console.WriteLine("\n" + new string('═', 80) + "\n");
            DemonstrateAuth2();
        }

        private static void DemonstrateAuth1()
        {
            Console.WriteLine("AUTH API 1.0 - Login Pattern with Token & Logout Services");
            Console.WriteLine(new string('-', 80));

            // Create token service (provides access token after login)
            var tokenService = new AuthService1(
                "https://auth.example.org/token",
                Profile.AuthToken.Value
            );

            // Create logout service
            var logoutService = new AuthService1(
                "https://auth.example.org/logout",
                Profile.AuthLogout.Value
            )
            .SetLabel("Logout from Example Institution");

            // Create login service with nested token and logout
            var loginService = new AuthService1(
                "https://auth.example.org/login",
                Profile.AuthLogin.Value
            )
            .SetLabel("Login to View Content")
            .SetHeader("Authentication Required")
            .SetDescription("Please log in with your institutional credentials.")
            .SetConfirmLabel("Login")
            .SetFailureHeader("Login Failed")
            .SetFailureDescription("Authentication was unsuccessful. Please try again.")
            .AddService(tokenService)
            .AddService(logoutService);

            var json = JsonConvert.SerializeObject(loginService, Formatting.Indented);
            Console.WriteLine(json);
            Console.WriteLine("\nKey features:");
            Console.WriteLine("✓ Login service with user-facing labels");
            Console.WriteLine("✓ Token service for authorization");
            Console.WriteLine("✓ Logout service for session management");
            Console.WriteLine("✓ Nested service structure: login → [token, logout]");
        }

        private static void DemonstrateAuth2()
        {
            Console.WriteLine("AUTH API 2.0 - Probe/Access/Token Pattern");
            Console.WriteLine(new string('-', 80));

            // Create logout service
            var logoutService = new AuthService2("https://auth.example.org/auth2/logout");

            // Create access token service
            var tokenService = new AuthService2("https://auth.example.org/auth2/token")
                .AddService(logoutService);

            // Create access service (active pattern for interactive auth)
            var accessService = new AuthService2(
                "https://auth.example.org/auth2/access",
                "active" // profile for active authentication
            )
            .SetLabel("Login Required")
            .SetHeading("Secure Content Access")
            .SetNote("This content requires authentication. Please log in to continue.")
            .SetConfirmLabel("Login")
            .AddService(tokenService);

            // Create probe service (entry point for checking access)
            var probeService = new AuthService2("https://auth.example.org/auth2/probe")
                .AddService(accessService);

            var json = JsonConvert.SerializeObject(probeService, Formatting.Indented);
            Console.WriteLine(json);
            Console.WriteLine("\nKey features:");
            Console.WriteLine("✓ Probe service for access checking");
            Console.WriteLine("✓ Access service with 'active' profile");
            Console.WriteLine("✓ Token service for authorization");
            Console.WriteLine("✓ Logout service for session termination");
            Console.WriteLine("✓ Nested service structure: probe → access → token → logout");
        }
    }
}
