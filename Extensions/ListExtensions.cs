﻿using System.Collections.Generic;
using System.Linq;

namespace ContestSystem.Extensions
{
    public static class ListExtensions
    {
        public static bool IsEmpty<T>(this IEnumerable<T> list)
        {
            return !list.Any();
        }
    }
}