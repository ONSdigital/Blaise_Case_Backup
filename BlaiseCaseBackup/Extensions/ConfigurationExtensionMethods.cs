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
        public static string ThrowExceptionIfNotNull(this string enviromentVariable)
        {
            throw new ConfigurationErrorsException($"No value found for enviroment variable '{enviromentVariable}'");
        }
    }
}
