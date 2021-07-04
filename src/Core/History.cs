using BluebirdPS.Models;
using System.Collections.Generic;
namespace BluebirdPS.Core
{
    internal class History
    {
        private static List<ResponseData> history;
        public static List<ResponseData> GetOrCreateInstance() => history ??= Create();
        private static List<ResponseData> Create()
        {
            history = new List<ResponseData>();
            return history;
        }
    }
}
