using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlaiseCaseBackup.Extensions
{
    public static class ConfigurationExtensionMethods
    {
        public static void ThrowExceptionIfNull(this string environmentVariable, string variableName)
        {
            if (string.IsNullOrWhiteSpace(environmentVariable))
            {
                throw new ConfigurationErrorsException($"No value found for environment variable '{variableName}'");
            }
        }
    }
}
