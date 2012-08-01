using System;

namespace Qi
{
    /// <summary>
    /// 
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = true)]
    public sealed class EnumDescriptionAttribute : Attribute
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="name">show name of the Enum value, if set the <seealso cref="ResourceType"/>, it means the Key of resource. </param>
        public EnumDescriptionAttribute(string name)
        {
            Name = name;
        }

        /// <summary>
        /// Gets or sets the type of resource¡£
        /// </summary>
        public Type ResourceType { get; set; }

        /// <summary>
        /// Gets or sets the Name of this descirption.
        /// </summary>
        public string Name { get; set; }
    }
}