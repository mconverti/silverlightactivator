// <copyright file="ActivationManager.cs" company="open-source">
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
    using System.Linq;
    using System.Windows;

    /// <summary>
    /// Represents the activation entry point.
    /// </summary>
    public class ActivationManager
    {
        private static bool initialized = false;

        /// <summary>
        /// Initializes a new instance of the SilverlightActivator.ActivationManager class, containing the activation mechanisms.
        /// </summary>
        public ActivationManager()
        {
            if (!initialized)
            {
                Init();
                initialized = true;
            }
        }

        /// <summary>
        /// Runs all the available ApplicationStartup methods.
        /// </summary>
        public static void RunApplicationStartupMethods()
        {
            RunActivationMethods<ApplicationStartupMethodAttribute>();
        }

        /// <summary>
        /// Runs all the available ApplicationExit methods.
        /// </summary>
        public static void RunApplicationExitMethods()
        {
            RunActivationMethods<ApplicationExitMethodAttribute>();
        }

        private static void Init()
        {
            Application.Current.Startup += (s, e) => RunApplicationStartupMethods();
            Application.Current.Exit += (s, e) => RunApplicationExitMethods();
        }

        private static void RunActivationMethods<T>() where T : BaseActivationMethodAttribute
        {
            var deploymentParts = Deployment.Current.Parts.Cast<AssemblyPart>();
            var activationAssemblies = AppDomain.CurrentDomain.GetAssemblies()
                .Where(a => deploymentParts.Any(p => p.Source.Equals(a.ManifestModule.Name, StringComparison.OrdinalIgnoreCase))
                            && (a != typeof(ActivationManager).Assembly));

            foreach (var assembly in activationAssemblies)
            {
                // The activation methods are executed according to the specified order
                var activationAttributes = assembly.GetActivationAttributes<T>().OrderBy(at => at.Order);
                foreach (var attribute in activationAttributes)
                {
                    attribute.InvokeMethod();
                }
            }
        }
    }
}
