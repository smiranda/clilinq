using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;
using Microsoft.CodeAnalysis.Scripting.Hosting;
using System.CommandLine;
using System.CommandLine.Invocation;

namespace clilinq
{
  class Program
  {
    static void Main(string script, string format = "lines")
    {
      List<string> seq = null;
      List<JToken> seqj = null;
      JToken jtok = null;
      using (var reader = new StreamReader(Console.OpenStandardInput()))
      {
        if (format == "lines")
        {
          seq = new List<string>();
          string s;
          while ((s = reader.ReadLine()) != null)
          {
            seq.Add(s);
          }
        }
        else if (format == "jsonl")
        {
          seqj = new List<JToken>();
          string s;
          while ((s = reader.ReadLine()) != null)
          {
            seqj.Add(JToken.Parse(s));
          }
        }
        else if (format == "json")
        {
          jtok = JToken.Parse(reader.ReadToEnd());
        }
        else
        {
          throw new NotImplementedException("Known formats: lines, jsonl, json");
        }
      }

      var scriptOptions = ScriptOptions.Default;
      var mscorlib = typeof(object).GetTypeInfo().Assembly;
      var systemCore = typeof(System.Linq.Enumerable).GetTypeInfo().Assembly;
      var newton = typeof(Newtonsoft.Json.Linq.JObject).GetTypeInfo().Assembly;

      var references = new[] { mscorlib, systemCore, newton };
      scriptOptions = scriptOptions.AddReferences(references);

      using (var interactiveLoader = new InteractiveAssemblyLoader())
      {
        foreach (var reference in references)
        {
          interactiveLoader.RegisterDependency(reference);
        }

        scriptOptions = scriptOptions.AddImports("System");
        scriptOptions = scriptOptions.AddImports("System.Linq");
        scriptOptions = scriptOptions.AddImports("System.Collections.Generic");
        scriptOptions = scriptOptions.AddImports("Newtonsoft.Json");
        scriptOptions = scriptOptions.AddImports("Newtonsoft.Json.Linq");

        Script cscript = null;
        ScriptState state = null;
        if (format == "lines")
        {
          cscript = CSharpScript.Create(@"", scriptOptions, typeof(InputData<List<string>>), interactiveLoader);
          state = cscript.RunAsync(new InputData<List<string>> { Data = seq }).Result;
        }
        else if (format == "jsonl")
        {
          cscript = CSharpScript.Create(@"", scriptOptions, typeof(InputData<List<JToken>>), interactiveLoader);
          state = cscript.RunAsync(new InputData<List<JToken>> { Data = seqj }).Result;
        }
        else if (format == "json")
        {
          cscript = CSharpScript.Create(@"", scriptOptions, typeof(InputData<JToken>), interactiveLoader);
          state = cscript.RunAsync(new InputData<JToken> { Data = jtok }).Result;
        }

        state = state.ContinueWithAsync(script).Result;

        var output = state.ReturnValue;
        Console.WriteLine(output);
      }
    }
  }
}
