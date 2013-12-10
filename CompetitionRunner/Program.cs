using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Remoting;
using System.Security;
using System.Security.Permissions;
using System.Security.Policy;

namespace CompetitionRunner
{
    class Program
    {
        //The Sandboxer class needs to derive from MarshalByRefObject so that we can create it in another 
        // AppDomain and refer to it from the default AppDomain.
        class Sandboxer : MarshalByRefObject
        {
            //const string pathToUntrusted = @"..\..\..\UntrustedCode\bin\Debug";
            const string untrustedAssembly = "UntrustedCode";
            const string untrustedClass = "UntrustedCode.UntrustedClass";
            const string entryPoint = "IsFibonacci";
            private static Object[] parameters = { 45 };
            static void Main()
            {
                //Setting the AppDomainSetup. It is very important to set the ApplicationBase to a folder 
                //other than the one in which the sandboxer resides.
                AppDomainSetup adSetup = new AppDomainSetup();
                adSetup.ApplicationBase = Path.GetFullPath(@".");

                //Setting the permissions for the AppDomain. We give the permission to execute and to 
                //read/discover the location where the untrusted code is loaded.
                PermissionSet permSet = new PermissionSet(PermissionState.None);
                permSet.AddPermission(new SecurityPermission(SecurityPermissionFlag.Execution));

                //We want the sandboxer assembly's strong name, so that we can add it to the full trust list.
                StrongName fullTrustAssembly = typeof(Sandboxer).Assembly.Evidence.GetHostEvidence<StrongName>();

                //Now we have everything we need to create the AppDomain, so let's create it.
                AppDomain newDomain = AppDomain.CreateDomain("Sandbox", null, adSetup, permSet, fullTrustAssembly);

                //Use CreateInstanceFrom to load an instance of the Sandboxer class into the
                //new AppDomain. 
                ObjectHandle handle = Activator.CreateInstanceFrom(
                    newDomain, typeof(Sandboxer).Assembly.ManifestModule.FullyQualifiedName,
                    typeof(Sandboxer).FullName
                    );
                //Unwrap the new domain instance into a reference in this domain and use it to execute the 
                //untrusted code.
                Sandboxer newDomainInstance = (Sandboxer)handle.Unwrap();
                //newDomainInstance.ExecuteUntrustedCode(untrustedAssembly, untrustedClass, entryPoint, parameters);



                var controls = GetElevatorControlImplementations();

                newDomainInstance.StartSimulation(controls);

            }

            private void StartSimulation(List<Type> controls)
            {
                try
                {
                    var elevatorControls = CreateInstances(controls);

                    var simulation = new Simulation();

                    simulation.Start(new List<ElevatorControl>(elevatorControls));

                    Console.WriteLine();
                    Console.WriteLine();
                    Console.WriteLine("Press any key to exit.");
                    Console.ReadKey(true);
                }
                catch (Exception ex)
                {
                    // When we print informations from a SecurityException extra information can be printed if we are 
                    //calling it with a full-trust stack.
                    (new PermissionSet(PermissionState.Unrestricted)).Assert();
                    Console.WriteLine("SecurityException caught:\n{0}", ex);
                    CodeAccessPermission.RevertAssert();
                    Console.ReadLine();
                }


            }

            private List<ElevatorControl> CreateInstances(List<Type> controls)
            {
                var result = new List<ElevatorControl>();
                foreach (var control in controls)
                {
                    var instance = (ElevatorControl)Activator.CreateInstance(control);
                    result.Add(instance);
                }

                return result;
            }

            private static List<Type> GetElevatorControlImplementations()
            {
                var files = Directory.GetFiles(Environment.CurrentDirectory).Where(p => p.EndsWith(".dll")).ToList();
                var result = new List<Type>();

                foreach (var file in files)
                {
                    var elevatorControls =
                        Assembly.LoadFile(file).GetTypes().Where(p => typeof(ElevatorControl).IsAssignableFrom(p));
                    result.AddRange(elevatorControls);
                }

                return result;
            }
        }
    }
}
