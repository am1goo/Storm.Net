﻿using Ston.Serializers.Loaders;
using System;
using System.Collections.Generic;

namespace Ston.Serializers
{
    public class UrlStonConverter : IStonConverter
    {
        public static readonly UrlStonConverter instance = new UrlStonConverter();

        private readonly Dictionary<string, IUrlStonLoader> _loaders = new Dictionary<string, IUrlStonLoader>
        {
            { "file", FileUrlStonLoader.instance },
        };

        public bool CanConvert(string type)
        {
            return type == "url";
        }

        public IStonValue Deserialize(string type, string text, StonContext ctx)
        {
            if (!Uri.TryCreate(text, UriKind.RelativeOrAbsolute, out var uri))
                return StonValue.nil;

            if (!_loaders.TryGetValue(uri.Scheme, out var loader))
                return StonValue.nil;

            return loader.Deserialize(uri, ctx);
        }
    }
}
