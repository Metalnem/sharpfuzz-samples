# SharpFuzz samples

[![Build Status][build-shield]][build-link]
[![License][license-shield]][license-link]

[build-shield]: https://github.com/metalnem/sharpfuzz-samples/actions/workflows/dotnet.yml/badge.svg
[build-link]: https://github.com/Metalnem/sharpfuzz-samples/actions/workflows/dotnet.yml
[license-shield]: https://img.shields.io/badge/license-MIT-blue.svg?style=flat
[license-link]: https://github.com/metalnem/sharpfuzz-samples/blob/master/LICENSE

Complete SharpFuzz fuzzing projects for various NuGet packages:

- [AngleSharp](https://github.com/Metalnem/sharpfuzz-samples/tree/master/src/AngleSharp)
- [Bond](https://github.com/Metalnem/sharpfuzz-samples/tree/master/src/Bond)
- [BouncyCastle](https://github.com/Metalnem/sharpfuzz-samples/tree/master/src/BouncyCastle)
- [Google.Protobuf](https://github.com/Metalnem/sharpfuzz-samples/tree/master/src/Google.Protobuf)
- [GraphQL-Parser](https://github.com/Metalnem/sharpfuzz-samples/tree/master/src/GraphQL.Parser)
- [HtmlAgilityPack](https://github.com/Metalnem/sharpfuzz-samples/tree/master/src/HtmlAgilityPack)
- [Markdig](https://github.com/Metalnem/sharpfuzz-samples/tree/master/src/Markdig)
- [MessagePack](https://github.com/Metalnem/sharpfuzz-samples/tree/master/src/MessagePack)
- [Newtonsoft.Json](https://github.com/Metalnem/sharpfuzz-samples/tree/master/src/Newtonsoft.Json)
- [protobuf-net](https://github.com/Metalnem/sharpfuzz-samples/tree/master/src/ProtobufNet)
- [YamlDotNet](https://github.com/Metalnem/sharpfuzz-samples/tree/master/src/YamlDotNet)

Example usage:

```powershell
scripts/fuzz.ps1 src/ProtobufNet/ProtobufNet.Fuzz.csproj `
    -i src/ProtobufNet/Testcases
```

Example usage with a dictionary file:

```powershell
scripts/fuzz.ps1 src/Markdig/Markdig.Fuzz.csproj `
    -i src/Markdig/Testcases `
    -x dictionaries/markdown.dict
```

The fuzzing script ([fuzz.ps1](https://github.com/Metalnem/sharpfuzz/raw/master/scripts/fuzz.ps1))
is located in the main [SharpFuzz](https://github.com/Metalnem/sharpfuzz) repo.
