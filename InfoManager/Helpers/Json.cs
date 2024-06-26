﻿using Newtonsoft.Json;

namespace InfoManager.Helpers;

public static class Json
{
    public static async Task<T?> ToObjectAsync<T>(string value) =>
        await Task.Run(() => JsonConvert.DeserializeObject<T>(value));

    public static async Task<string> StringifyAsync(object value) =>
        await Task.Run(() => JsonConvert.SerializeObject(value));
}