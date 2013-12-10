using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security;
using System.Security.Permissions;
using System.Security.Policy;
using ElevatorCompetition.Core;

namespace ElevatorCompetition
{
    class Program
    {
        //The Sandboxer class needs to derive from MarshalByRefObject so that we can create it in another 
        // AppDomain and refer to it from the default AppDomain.
        class Sandboxer : MarshalByRefObject
        {
            static void Main()
            {
                //Setting the AppDomainSetup. It is very important to set the ApplicationBase to a folder 
                //other than the one in which the sandboxer resides.
                var adSetup = new AppDomainSetup();
                adSetup.ApplicationBase = Path.GetFullPath(@".");

                //Setting the permissions for the AppDomain. We give the permission to execute and to 
                //read/discover the location where the untrusted code is loaded.
                var permSet = new PermissionSet(PermissionState.None);
                permSet.AddPermission(new SecurityPermission(SecurityPermissionFlag.Execution));
                permSet.AddPermission(new UIPermission(PermissionState.Unrestricted));

                //We want the sandboxer assembly's strong name, so that we can add it to the full trust list.
                var fullTrustAssembly = typeof(Sandboxer).Assembly.Evidence.GetHostEvidence<StrongName>();

                //Now we have everything we need to create the AppDomain, so let's create it.
                var newDomain = AppDomain.CreateDomain("Sandbox", null, adSetup, permSet, fullTrustAssembly);

                //Use CreateInstanceFrom to load an instance of the Sandboxer class into the
                //new AppDomain. 
                var handle = Activator.CreateInstanceFrom(
                    newDomain, typeof(Sandboxer).Assembly.ManifestModule.FullyQualifiedName,
                    typeof(Sandboxer).FullName);
                //Unwrap the new domain instance into a reference in this domain and use it to execute the 
                //untrusted code.
                var newDomainInstance = (Sandboxer)handle.Unwrap();
                newDomainInstance.StartSimulation();
            }

            private void StartSimulation()
            {
                try
                {
                    (new PermissionSet(PermissionState.Unrestricted)).Assert();
                    var controls = GetElevatorControlImplementations();
                    var simulation = new Simulation();
                    simulation.Start(CreateInstances(controls));
                    Console.WriteLine();
                    Console.WriteLine();
                    Console.WriteLine("Press any key to exit.");
                    Console.ReadKey(true);
                    CodeAccessPermission.RevertAssert();
                }
                catch (Exception ex)
                {
                    Console.WriteLine("SecurityException caught:\n{0}", ex);
                    Console.ReadLine();
                }
            }

            private static List<ElevatorControl> CreateInstances(IEnumerable<Type> controls)
            {
                return controls.Select(control => (ElevatorControl) Activator.CreateInstance(control)).ToList();
            }

            private static IEnumerable<Type> GetElevatorControlImplementations()
            {
                var files =
                    Directory.GetFiles(Environment.CurrentDirectory)
                             .Where(p => p.EndsWith(".dll") && !p.EndsWith("ElevatorCompetition.Core.dll"))
                             .ToList();
                var result = new List<Type>();

                var elevatorControlType = typeof (ElevatorControl);
                foreach (var file in files)
                {
                    var elevatorControls =
                        Assembly.LoadFile(file).GetTypes().Where(elevatorControlType.IsAssignableFrom);
                    result.AddRange(elevatorControls);
                }

                return result;
            }
        }
    }
}
