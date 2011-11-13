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
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Windows;

    /// <summary>
    /// Represents the activation entry point.
    /// </summary>
    public class ActivationManager
    {
        private static bool initialized = false;
        private static bool loadAssemblyParts = false;
        private static bool assemblyPartsLoaded = false;

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
        /// Gets or Sets a flag indicating if all the assembly parts in the 
        /// Application Manifest should be loaded at startup to look for 
        /// activation attributes.
        /// </summary>
        public bool LoadAssemblyParts
        {
            get { return loadAssemblyParts; }
            set { loadAssemblyParts = value;  }
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

            if (loadAssemblyParts && !assemblyPartsLoaded)
            {
                Load(deploymentParts);
                assemblyPartsLoaded = true;
            }

            // Filter loaded assemblies to only get the deployment assembly parts.
            var activationAssemblies = AppDomain.CurrentDomain.GetAssemblies()
                .Where(a => deploymentParts.Any(p => p.Source.Equals(a.ManifestModule.Name, StringComparison.OrdinalIgnoreCase))
                            && (a != typeof(ActivationManager).Assembly))
                .ToList();
            var activationAttributes = new List<T>();

            // Iterate throughout all the loaded deployment assembly parts to look for activation attributes.
            activationAssemblies.ForEach(assembly => activationAttributes.AddRange(assembly.GetActivationAttributes<T>()));

            // Execute activation methods according to the order specified.
            foreach (var attribute in activationAttributes.OrderBy(at => at.Order))
            {
                attribute.InvokeMethod();
            }
        }

        private static void Load(IEnumerable<AssemblyPart> deploymentParts)
        {
            foreach (var assemblyPart in deploymentParts)
            {
                var assemblyString = assemblyPart.Source
                    .ToUpperInvariant()
                    .Replace(".DLL", string.Empty);

                try
                {
                    if (!string.IsNullOrWhiteSpace(assemblyString))
                        Assembly.Load(assemblyString);
                }
                catch (FileNotFoundException) { }
                catch (FileLoadException) { }
                catch (BadImageFormatException) { }
            }
        }
    }
}
