﻿using System.Text.Json;

public class KebabCaseNamingPolicy : JsonNamingPolicy
{
    public override string ConvertName(string name)
    {
        return string.Concat(
            name.Select((c, i) =>
                i > 0 && char.IsUpper(c)
                    ? "-" + char.ToLower(c)
                    : char.ToLower(c).ToString()
            )
        );
    }
}
