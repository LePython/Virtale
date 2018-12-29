using IronPython;
using IronPython.Modules;
using IronPython.Hosting;
using Microsoft.Scripting.Hosting;
using Microsoft.Scripting;
using System.Collections.Generic;

namespace RunPythonScript
{
    class LoadIronPythonMLModel
    {
        public static void ScriptTest()
        {

        }

        /// <summary>
        /// Get the last assigned parameter in python script
        /// </summary>
        /// <param name="path"></param>
        /// <param name="parameter"></param>
        /// <param name="serviceid"></param>
        /// <returns></returns>
        public string PatchParameter(string path, string parameter)
        {
            //var engine = Python.CreateEngine(); // Extract Python language engine from their grasp
            //var scope = engine.CreateScope(); // Introduce Python namespace (scope)
            //var d = new Dictionary<string, object>
            //{
            //    { "serviceid", serviceid},
            //    { "parameter", parameter}
            //}; // Add some sample parameters. Notice that there is no need in specifically setting the object type, interpreter will do that part for us in the script properly with high probability

            //scope.SetVariable("params", d); // This will be the name of the dictionary in python script, initialized with previously created .NET Dictionary
            //ScriptSource source = engine.CreateScriptSourceFromFile(path); // Load the script
            //object result = source.Execute(scope);
            //parameter = scope.GetVariable<string>("parameter"); // To get the finally set variable 'parameter' from the python script
            //return parameter;

            // create the engine    
            ScriptEngine scriptEngine = Python.CreateEngine();
            ScriptSource source = scriptEngine.CreateScriptSourceFromFile(path);

            // and the scope (ie, the python namespace)    

            ScriptScope scriptScope = scriptEngine.CreateScope();

            // execute a string in the interpreter and grab the variable

            string inputValue = parameter;
            //scriptEngine.GetSysModule().SetVariable("inputValue", inputValue);

            object result = source.Execute(scriptScope);

            parameter = scriptScope.GetVariable<string>("parameter");

            return parameter;
        }
    }
}
