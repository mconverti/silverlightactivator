// <copyright file="ApplicationExitMethodAttribute.cs" company="open-source">
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

    /// <summary>
    /// Identifies activation methods that run while the application is exiting. 
    /// </summary>
    [AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true)]
    public sealed class ApplicationExitMethodAttribute : BaseActivationMethodAttribute
    {
        /// <summary>
        /// Initializes a new instance of the ApplicationExitMethodAttribute class with the specified parameters.
        /// </summary>
        /// <param name="activationType">The activation type.</param>
        /// <param name="methodName">The activation method name.</param>
        public ApplicationExitMethodAttribute(Type activationType, string methodName)
            : base(activationType, methodName)
        {
        }
    }
}
