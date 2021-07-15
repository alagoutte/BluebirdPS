using System.Collections.Generic;
using BluebirdPS.Models;
namespace BluebirdPS.Core
{
    internal class History
    {
        private static List<ResponseData> history;

        private static List<ResponseData> Create()
        {
            return new List<ResponseData>();
        }

        public static List<ResponseData> GetOrCreateInstance() => history ??= Create();
    }
}
