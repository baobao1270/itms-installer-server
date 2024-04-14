using QRCoder;
using System.Drawing;
using System.Reflection;
using Microsoft.AspNetCore.Mvc;
using ITMSInstallerServer.IOSApplicationArchive;

namespace ITMSInstallerServer{
    [ApiController]
    public class Controller : ControllerBase {
        private readonly Assembly assembly;
        private readonly Arguments arguments;
        private readonly IPACollection ipaCollection;
        private readonly QRCodeGenerator qrCodeGenerator;
        private readonly Endpoint endpoint;
        

        public Controller(Assembly assembly, Arguments arguments, IPACollection ipaCollection,
                QRCodeGenerator qrCodeGenerator, IHttpContextAccessor hca) {
            this.assembly = assembly;
            this.arguments = arguments;
            this.ipaCollection = ipaCollection;
            this.qrCodeGenerator = qrCodeGenerator;
            endpoint = new Endpoint(hca.HttpContext.Request.Host);
        }

        [HttpGet("/version")]
        public ActionResult<string> GetVersion() {
            return Ok(Program.Version);
        }

        [HttpGet("/qrcode")]
        public ActionResult<byte[]> GetQRCode() {
            var qr = qrCodeGenerator.CreateQrCode(endpoint.GetWebAddress(), QRCodeGenerator.ECCLevel.Q);
            var image = (new PngByteQRCode(qr))
                .GetGraphic(20, new byte[] { 0, 0, 0, 255 }, new byte[] { 0, 0, 0, 0 });
            return File(image, "image/png");
        }

        [HttpGet("/ipas")]
        public ActionResult<IEnumerable<IPAManifest>> GetIPAManifests() {
            var ipaManifest = ipaCollection
                .GetFiles()
                .Select(x => x.ReadManifest());
            return Ok(ipaManifest);
        }

        [HttpGet("/manifest/{fileName}")]
        public ActionResult<string> GetManifest(string fileName) {
            var stream = assembly.GetManifestResourceStream("ITMSInstallerServer.Resources.manifest.plist");
            if (stream == null) throw new NullReferenceException("manifest.plist not found in resources");
            using (var reader = new StreamReader(stream)) {
                var template = reader.ReadToEnd();
                var ipaFile = ipaCollection.GetFiles().FirstOrDefault(x => x.FileName == fileName);
                if (ipaFile == null) return NotFound("The IPA file does not exist");
                var ipaManifest = ipaFile.ReadManifest();
                return template
                    .Replace("%URL%", endpoint.GetIPAFileAddress(fileName))
                    .Replace("%BUNDLE_NAME%", ipaManifest.BundleName)
                    .Replace("%BUNDLE_VERSION%", ipaManifest.BundleVersion)
                    .Replace("%BUNDLE_ID%", ipaManifest.BundleId);
            }
        }
    }
}
