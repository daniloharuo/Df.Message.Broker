using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Df.Sample.Model
{
    public class Test
    {
        public Test()
        {
            Version = Environment.Version.ToString();
            ProjectName = Assembly.GetCallingAssembly().GetName().Name;
        }

        public string Version { get; private set; }
        public string ProjectName { get; private set; }
    }
}
