// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.
//
// https://github.com/dotnet/runtime/blob/master/src/libraries/Microsoft.Extensions.Configuration.Json/src/JsonConfigurationFileParser.cs

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text.Json;
using AmCreations.Configuration.EncryptedJson.Crypters;
using Microsoft.Extensions.Configuration;

namespace AmCreations.Configuration.EncryptedJson
{
    internal class EncryptedJsonConfigurationFileParser
    {
        private readonly ICrypter _crypter;
        
        private EncryptedJsonConfigurationFileParser(ICrypter crypter)
        {
            _crypter = crypter;
        }

        public static IDictionary<string, string> Parse(Stream input, ICrypter crypter)
            => new EncryptedJsonConfigurationFileParser(crypter).ParseStream(input);

        public static IDictionary<string, string> Parse(string path, ICrypter crypter)
            => new EncryptedJsonConfigurationFileParser(crypter).ParseStream(path);

        private readonly IDictionary<string, string> _data = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        private readonly Stack<string> _paths = new Stack<string>();

        private IDictionary<string, string> ParseStream(string path)
        {
            var jsonDocumentOptions = new JsonDocumentOptions
            {
                CommentHandling = JsonCommentHandling.Skip,
                AllowTrailingCommas = true,
            };

            using (var reader = new StreamReader(path))
            using (var doc = JsonDocument.Parse(reader.ReadToEnd(), jsonDocumentOptions))
            {
                if (doc.RootElement.ValueKind != JsonValueKind.Object)
                {
                    throw new FormatException($"Unsupported JSON token '{doc.RootElement.ValueKind}' was found");
                }
                VisitElement(doc.RootElement);
            }

            return _data;
        }

        private IDictionary<string, string> ParseStream(Stream stream)
        {
            _data.Clear();

            var jsonDocumentOptions = new JsonDocumentOptions
            {
                CommentHandling = JsonCommentHandling.Skip,
                AllowTrailingCommas = true,
            };

            using (var reader = new StreamReader(stream))
            using (var doc = JsonDocument.Parse(reader.ReadToEnd(), jsonDocumentOptions))
            {
                if (doc.RootElement.ValueKind != JsonValueKind.Object)
                {
                    throw new FormatException($"Unsupported JSON token '{doc.RootElement.ValueKind}' was found");
                }
                VisitElement(doc.RootElement);
            }

            return _data;
        }

        private void VisitElement(JsonElement element)
        {
            var isEmpty = true;

            foreach (var property in element.EnumerateObject())
            {
                isEmpty = false;
                EnterContext(property.Name);
                VisitValue(property.Value);
                ExitContext();
            }

            if (isEmpty && _paths.Count > 0)
            {
                _data[_paths.Peek()] = null;
            }
        }

        private void VisitValue(JsonElement value)
        {
            Debug.Assert(_paths.Count > 0);

            switch (value.ValueKind)
            {
                case JsonValueKind.Object:
                    VisitElement(value);
                    break;

                case JsonValueKind.Array:
                    var index = 0;
                    foreach (var arrayElement in value.EnumerateArray())
                    {
                        EnterContext(index.ToString());
                        VisitValue(arrayElement);
                        ExitContext();
                        index++;
                    }
                    break;

                case JsonValueKind.String:
                    var key = _paths.Peek();
                    if (_data.ContainsKey(key))
                    {
                        throw new FormatException($"A duplicate key '{key}' was found.");
                    }
                    
                    _data[key] = _crypter.DecryptString(value.ToString());
                    break;

                default:
                    throw new FormatException($"Unsupported JSON token '{value.ValueKind}' was found. Only string values can be decrypted.");
            }
        }

        private void EnterContext(string context) =>
            _paths.Push(_paths.Count > 0 ?
                _paths.Peek() + ConfigurationPath.KeyDelimiter + context :
                context);

        private void ExitContext() => _paths.Pop();
    }
}