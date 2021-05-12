using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace ContestSystem.Extensions
{
    public static class ControllerExtensions
    {
        public static object FormDataForJson<TModel>(this ControllerBase controller, TModel model, string className, List<string> properties)
                                                                                                                                    where TModel: new()
        {
            object result = new TModel();
            PropertyInfo[] propertyInfos = Type.GetType(className).GetProperties();
            for (int i = 0; i < properties.Count; i++)
            {
                properties[i] = properties[i].ToLower();
            }
            foreach (PropertyInfo prop in propertyInfos)
            {
                if (properties.Any(propName => propName == prop.ToString().ToLower()))
                {
                    prop.SetValue(result, prop.GetValue(model));
                }
            }
            return result;
        }
    }
}
