using QRCoder;
using System.Reflection;
using ITMSInstallerServer.IOSApplicationArchive;
using ITMSInstallerServer.Components;
using Microsoft.Extensions.FileProviders;

namespace ITMSInstallerServer {
    class WebServer {
        private WebApplication app;
        private Arguments arguments;
        private EphemeralECCCertificate certificate;

        public WebServer(Arguments arguments, EphemeralECCCertificate certificate) {
            this.arguments = arguments;
            this.certificate = certificate;

            var builder = WebApplication.CreateBuilder();
            ConfigureHost(builder);

            app = builder.Build();
            ConfigureApp();
        }

        private void ConfigureHost(WebApplicationBuilder builder) {
            builder.WebHost.ConfigureKestrel((context, serverOptions) => {
                serverOptions.ListenAnyIP(arguments.WebPort, listenOptions => {
                    listenOptions.UseHttps(httpsOptions => {
                        httpsOptions.ServerCertificate = certificate.Certificate;
                    });
                });
                serverOptions.ListenAnyIP(arguments.DataPort);
            });

            var assembly = Assembly.GetExecutingAssembly();
            builder.Services.AddSingleton(assembly);
            builder.Services.AddSingleton(arguments);
            builder.Services.AddSingleton(certificate);
            builder.Services.AddScoped<QRCodeGenerator>();
            builder.Services.AddScoped(_ => new IPACollection(arguments.IPADirectory));
            builder.Services.AddHttpContextAccessor();
            builder.Services.AddControllers();
            builder.Services
                .AddRazorComponents()
                .AddInteractiveServerComponents();
        }

        private void ConfigureApp() {
            app.UseStaticFiles(
                new StaticFileOptions
                {
                    FileProvider = new PhysicalFileProvider(new IPACollection(arguments.IPADirectory).FileDirectory),
                    RequestPath = new PathString("/packages"),
                    ServeUnknownFileTypes = true,
                    DefaultContentType = "application/octet-stream",
                }
            );
            app.UseAntiforgery();
            app.MapControllers();
            app.MapRazorComponents<App>().AddInteractiveServerRenderMode();
            app.MapFallback(async (context) => {
                context.Response.StatusCode = 404;
                await context.Response.WriteAsync("404 Not Found");
            });
        }

        public void Run() {
            app.Run();
        }
    }
}
