# ITMS Installer Server

此项目用于解决安装 iTunes 下载的 IPK 文件需要 HTTPS 的问题。此程序通过创建自签名证书，需要用户手动信任（是 HTTPS 证书，不是开发者证书），然后通过 HTTPS 服务提供 IPK 文件下载。

更加详细的使用方法请参见 [通过 iTunes 官方途径下载 iOS 旧版本应用](https://josephcz.xyz/technology/ios/download-ios-old-version-apps-with-itunes/)。

## 下载与使用
 1. 前往 [GitHub Releases](https://github.com/baobao1270/itms-installer-server/releases) 下载与您的系统对应的版本
 2. 解压下载的压缩包
 3. 将 IPA 文件放进 `./IPA` 文件夹
 4. 运行 `./itmserv`
 5. 打开程序输出的 HTTPS URL，用 iOS 设备扫描网页上的二维码

## 构建

构建此程序需要 .NET 8。您可以在 [此处下载](https://dotnet.microsoft.com/download) 下载对应的版本。

```bash
git clone https://github.com/baobao1270/itms-installer-server.git
cd itms-installer-server
dotnet restore
dotnet build
```

要发布构建，请运行：

```bash
dotnet publish -c Release -r <archtechture>
# 例如
dotnet publish -c Release -r osx-arm64
```

## 许可
此项目使用 MIT 许可证。详细信息请参见 [LICENSE](./LICENSE)。
