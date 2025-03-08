dotnet tool install --global Microsoft.OpenApi.Kiota --version 1.23.0

kiota generate  --exclude-backward-compatible `
    -l csharp `
    -c WebApiClient `
    --ad False `
    --ebc `
    --serializer Microsoft.Kiota.Serialization.Json.JsonSerializationWriterFactory `
    --deserializer Microsoft.Kiota.Serialization.Json.JsonParseNodeFactory `
    --structured-mime-types application/json `
    --clean-output `
    -n MoneyGroup.WebApi `
    -d ..\WebApi\MoneyGroup.WebApi.json `
    -o Client