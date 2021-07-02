﻿using System.Collections.Generic;
using System.Reflection;

namespace BluebirdPS.Models.APIV2.Metrics
{
    public class BaseMetrics : TwitterObject
    {
        public override string ToString()
        {
            List<string> displayMetrics = new List<string>();
            foreach (PropertyInfo property in GetType().GetProperties())
            {
                var value = property.GetValue(this, null);
                if (value != null)
                {
                    displayMetrics.Add($"{property.Name}: {value}");
                }
            }
            return string.Join(", ", displayMetrics);
        }

    }

}
