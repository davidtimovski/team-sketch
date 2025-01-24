using System.Collections.Generic;
using System.Text.Json.Serialization;
using TeamSketch.Common.ApiModels;

namespace TeamSketch.Serialization;

[JsonSourceGenerationOptions(PropertyNameCaseInsensitive = true)]
[JsonSerializable(typeof(JoinRoomValidationResult))]
[JsonSerializable(typeof(List<string>))]
internal partial class SourceGenerationContext : JsonSerializerContext
{
}
