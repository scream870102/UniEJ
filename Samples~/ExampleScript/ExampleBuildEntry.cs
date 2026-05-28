using System;
using UnityEditor;
using UnityEngine;

namespace Scream.UniEJ.Editor
{
    public static class Builder
    {
        public static void BuildEmpty()
        {
            try
            {
                string[] cmdArgs = Environment.GetCommandLineArgs();
                var args = new Arguments(cmdArgs);
                var builder = new EmptyBuilder<Arguments, ExampleBuildHelper>();
                var buildHelper = new ExampleBuildHelper();
                builder.Build(args, buildHelper);

                if (Application.isBatchMode)
                {
                    EditorApplication.Exit(0);
                }
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                if (Application.isBatchMode)
                {
                    EditorApplication.Exit(1);
                    return;
                }

                throw;
            }
        }
    }
}
