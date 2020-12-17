using System;

namespace Helpo.Common
{
    public static class IdHelper
    {
        public static string GenerateNewId()
        {
            return Guid.NewGuid().ToString("D");
        }
    }
}
