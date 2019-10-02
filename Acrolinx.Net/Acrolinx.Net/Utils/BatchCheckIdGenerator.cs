using System;

namespace Acrolinx.Net.Utils
{
    public static class BatchCheckIdGenerator
    {
        public static string GenerateId(string integrationShortName)
        {
            var uuid = Guid.NewGuid().ToString("D");
            return $"gen.{integrationShortName}.{uuid}";
        }
    }
}
