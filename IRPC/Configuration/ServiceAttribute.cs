using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenNGS.IRPC.Configuration
{
    /// <summary>
    /// Explicitly indicates that an interface represents a IRPC service
    /// </summary>
    [AttributeUsage(AttributeTargets.Interface, AllowMultiple = false, Inherited = false)]
    [ImmutableObject(true)]
    public sealed class ServiceAttribute : Attribute
    {
        /// <summary>
        /// The name of the service
        /// </summary>
        public string? Name { get; }
        /// <summary>
        /// Create a new instance of the attribute
        /// </summary>
        public ServiceAttribute(string? name = null)
            => Name = name;
    }



    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    [ImmutableObject(true)]
    public sealed class MethodAttribute : Attribute
    {
        /// <summary>
        /// The name of the service
        /// </summary>
        public string? Name { get; }
        /// <summary>
        /// Create a new instance of the attribute
        /// </summary>
        public MethodAttribute(string? name = null)
            => Name = name;
    }
}
