using System;
using System.Collections.Generic;
using System.Text;

namespace WebJobs.Extensions.ResourceDeployment
{
    /// <summary>
    /// Helper class for proper JSON object for ARM template parameters
    /// This is for string attributes
    /// </summary>
    public class StringParameter
    {
        public string value { get; set; }
        public StringParameter(string value)
        {
            this.value = value;
        }
    }
    /// <summary>
    /// Helper class for proper JSON object for ARM template parameters
    /// This is for int attributes
    /// </summary>
    public class IntParameter
    {
        public int value { get; set; }
        public IntParameter(int value)
        {
            this.value = value;
        }
    }
}
