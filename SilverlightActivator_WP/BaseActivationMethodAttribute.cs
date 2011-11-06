// <copyright file="BaseActivationMethodAttribute.cs" company="open-source">
//  No rights reserved. Copyright (c) 2011 by Mariano Converti, Damian Martinez, and Nico Bello
//   
//  Redistribution and use in source and binary forms, with or without modification, are permitted.
//
//  The names of its contributors may not be used to endorse or promote products derived from this software without specific prior written permission.
//
//  THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
// </copyright>

namespace SilverlightActivator
{
    using System;
    using System.Globalization;
    using System.Reflection;

    /// <summary>
    /// Base class of all activation attributes.
    /// </summary>
    [AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true)]
    public abstract class BaseActivationMethodAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the BaseActivationMethodAttribute class with the specified parameters.
        /// </summary>
        /// <param name="activationType">The activation type.</param>
        /// <param name="methodName">The activation method name (must be static).</param>
        /// <exception cref="System.ArgumentNullException" />
        /// <exception cref="System.ArgumentException" />
        protected BaseActivationMethodAttribute(Type activationType, string methodName)
        {
            if (activationType == null)
                throw new ArgumentNullException("activationType", "The activation type cannot be null");

            if (string.IsNullOrWhiteSpace(methodName))
                throw new ArgumentException("The method name cannot be null, empty or whitespace", "methodName");

            this.ActivationType = activationType;
            this.MethodName = methodName;

            // If no order is specified, is the last by default
            this.Order = int.MaxValue;
        }

        /// <summary>
        /// Gets the type that contains the activation method.
        /// </summary>
        public Type ActivationType { get; private set; }

        /// <summary>
        /// Gets the method name that will be called.
        /// </summary>
        public string MethodName { get; private set; }

        /// <summary>
        /// Gets or sets the execution order.
        /// </summary>
        public int Order { get; set; }

        /// <summary>
        /// Invokes the static activation method from the activation type.
        /// </summary>
        /// <exception cref="System.ArgumentException" />
        public void InvokeMethod()
        {
            var method = this.ActivationType.GetMethod(this.MethodName, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static);
            if (method == null)
                throw new ArgumentException(string.Format(
                    CultureInfo.InvariantCulture,
                    "The type {0} does not have a static method named {1}",
                    this.ActivationType,
                    this.MethodName));

            method.Invoke(null, null);
        }
    }
}
