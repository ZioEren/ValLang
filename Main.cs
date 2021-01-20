using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Runtime;
using System.Runtime.InteropServices;

class Program
{
    [DllImport("psapi.dll")]
    static extern int EmptyWorkingSet(IntPtr hwProc);

    [DllImport("kernel32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool SetProcessWorkingSetSize(IntPtr process, UIntPtr minimumWorkingSetSize, UIntPtr maximumWorkingSetSize);

    public static bool debugErrors, noClose;

    static void Main(string[] args)
    {
        Process.GetCurrentProcess().PriorityClass = System.Diagnostics.ProcessPriorityClass.RealTime;
        new Thread(clearRam).Start();
        EmptyWorkingSet(Process.GetCurrentProcess().Handle);
        SetProcessWorkingSetSize(Process.GetCurrentProcess().Handle, (UIntPtr)0xFFFFFFFF, (UIntPtr)0xFFFFFFFF);
        GCSettings.LargeObjectHeapCompactionMode = GCLargeObjectHeapCompactionMode.CompactOnce;

        string text = "", allString = "";

        foreach (string arg in args)
        {
            if (allString == "")
            {
                allString = arg;
            }
            else
            {
                allString += " " + arg;
            }
        }

        if (allString.StartsWith("--noClose "))
        {
            allString = allString.Substring(10, allString.Length - 10);
            noClose = true;
        }

        if (allString.StartsWith("--noClose"))
        {
            allString = allString.Substring(9, allString.Length - 9);
            noClose = true;
        }

        if (allString.EndsWith("--noClose"))
        {
            allString = allString.Substring(0, allString.Length - 9);
            noClose = true;
        }

        if (System.IO.File.Exists(allString))
        {
            completeRun(System.IO.Path.GetFileNameWithoutExtension(allString), System.IO.File.ReadAllText(allString));
        }
        else if (System.IO.File.Exists(allString + ".v"))
        {
            completeRun(System.IO.Path.GetFileNameWithoutExtension(allString + ".v"), System.IO.File.ReadAllText(allString + ".v"));
        }
        else
        {
            Console.WriteLine(@"

 __      __     _        _               _   _  _____ _    _         _____ ______ 
 \ \    / /\   | |      | |        /\   | \ | |/ ____| |  | |  /\   / ____|  ____|
  \ \  / /  \  | |      | |       /  \  |  \| | |  __| |  | | /  \ | |  __| |__   
   \ \/ / /\ \ | |      | |      / /\ \ | . ` | | |_ | |  | |/ /\ \| | |_ |  __|  
    \  / ____ \| |____  | |____ / ____ \| |\  | |__| | |__| / ____ \ |__| | |____ 
     \/_/    \_\______| |______/_/    \_\_| \_|\_____|\____/_/    \_\_____|______|
                                                                                  
                                                                                  

");

            Console.WriteLine("[*] Welcome to Val Language Interpreter! A free and open source interpreted programming language.");
            Console.WriteLine("[*] Made, developed and constructed by: ZioEren (ZioEren#1337, https://www.github.com/ZioEren/)");
            Console.WriteLine("\r\n[*] Here is the list of all commands that you can use:");
            Console.WriteLine("[+] --help - Get the list of all commands");
            Console.WriteLine("[+] --clear - Clear all the console lines.");
            Console.WriteLine("[+] --run <file> - Load and run a Val Language source code file from disk.");
            Console.WriteLine("[+] --debugErrors - Enable/disable interpreter error debugging.\r\n");

            while (true)
            {
                text = Console.ReadLine();

                if (text == "--help")
                {
                    Console.WriteLine("\r\n[*] Here is the list of all commands that you can use:");
                    Console.WriteLine("[+] --help - Get the list of all commands");
                    Console.WriteLine("[+] --clear - Clear all the console lines.");
                    Console.WriteLine("[+] --run <file> - Load and run a Val Language source code file from disk.");
                    Console.WriteLine("[+] --debugErrors - Enable/disable interpreter error debugging.\r\n");
                }
                else if (text.StartsWith("--run "))
                {
                    string thePath = text.Substring(6, text.Length - 6);

                    if (System.IO.File.Exists(thePath))
                    {
                        completeRun(System.IO.Path.GetFileNameWithoutExtension(thePath), System.IO.File.ReadAllText(thePath));
                    }
                    else if (System.IO.File.Exists(thePath + ".v"))
                    {
                        completeRun(System.IO.Path.GetFileNameWithoutExtension(thePath + ".v"), System.IO.File.ReadAllText(thePath + ".v"));
                    }
                    else
                    {
                        Console.WriteLine("[-] The specified file does not exist!");
                    }
                }
                else if (text == "--clear")
                {
                    Console.Clear();
                }
                else if (text == "--debugErrors")
                {
                    debugErrors = !debugErrors;

                    if (debugErrors)
                    {
                        Console.WriteLine("[+] Succesfully enabled the debug errors option.");
                    }
                    else
                    {
                        Console.WriteLine("[+] Succesfully disabled the debug errors option.");
                    }
                }
                else
                {
                    Tuple<object, Error> totalResult = run("program", text);
                    object result = totalResult.Item1;
                    Error error = totalResult.Item2;

                    if (error != null)
                    {
                        Console.WriteLine("[-] Invalid command. Type --help for the list of all commands.");
                    }
                    else
                    {
                        Console.WriteLine(result.GetType().GetMethod("as_string").Invoke(result, new object[] { }));
                    }
                }
            }
        }

        if (noClose)
        {
            while (true)
            {
                Console.ReadLine();
            }
        }

        Process.GetCurrentProcess().Kill();
    }

    public static void completeRun(string fn, string text)
    {
        if (text.Replace(" ", "").Trim(' ').Trim(';').Trim('\t').Trim('\r').Trim('\n').Replace(";", "").Replace("\r\n", "").Replace("\n", "").Replace("\t", "").Replace(" ", "") == "")
        {
            return;
        }

        Tuple<object, Error> totalResult = run(fn, System.Text.RegularExpressions.Regex.Replace(text, @"\r\n?|\n", ";"));
        object result = totalResult.Item1;
        Error error = totalResult.Item2;

        if (error != null)
        {
            Console.WriteLine(error.GetType() == typeof(RuntimeError) ? ((RuntimeError)error).get_rt_error() : error.as_string());
        }
    }

    public static Tuple<object, Error> run(string fn, string text)
    {
        try
        {
            Tuple<List<Token>, Error> tokensError = new Lexer(fn, text).make_tokens();

            List<Token> tokens = tokensError.Item1;
            Error error = tokensError.Item2;

            if (error != null)
            {
                return new Tuple<object, Error>(null, error);
            }

            ParseResult ast = new Parser(tokens).parse();

            if (ast.error != null)
            {
                return new Tuple<object, Error>(null, ast.error);
            }

            Interpreter interpreter = new Interpreter();
            Context context = new Context(fn);
            SymbolTable global_symbol_table = new SymbolTable();
            Importer.add(global_symbol_table, 0);
            context.symbol_table = global_symbol_table;
            RuntimeResult result = interpreter.visit(ast.node, context);

            return new Tuple<object, Error>(result.value, result.error);
        }
        catch (Exception ex)
        {
            if (debugErrors)
            {
                Console.WriteLine("Error trace:\r\n\r\n" + ex.Message.ToString() + "\r\n\r\n" + ex.StackTrace.ToString() + "\r\n\r\n");
            }
        }

        return new Tuple<object, Error>(null, new GenericError(new Position(0, 0, 0, fn, text), new Position(0, 0, 0, fn, text), "Failed to progress the runtime"));
    }

    public static void clearRam()
    {
        while (true)
        {
            Thread.Sleep(3000);
            GC.Collect(GC.MaxGeneration);
            GC.WaitForPendingFinalizers();
        }
    }

    public static Tuple<object, Error> import(string fn, string text, Context context)
    {
        try
        {
            Tuple<List<Token>, Error> tokensError = new Lexer(fn, System.Text.RegularExpressions.Regex.Replace(text, @"\r\n?|\n", ";")).make_tokens();

            List<Token> tokens = tokensError.Item1;
            Error error = tokensError.Item2;

            if (error != null)
            {
                return new Tuple<object, Error>(null, error);
            }

            ParseResult ast = new Parser(tokens).parse();

            if (ast.error != null)
            {
                return new Tuple<object, Error>(null, ast.error);
            }

            Interpreter interpreter = new Interpreter();
            RuntimeResult result = interpreter.visit(ast.node, context);

            return new Tuple<object, Error>(result.value, result.error);
        }
        catch (Exception ex)
        {
            if (debugErrors)
            {
                Console.WriteLine("Error trace:\r\n\r\n" + ex.Message.ToString() + "\r\n\r\n" + ex.StackTrace.ToString() + "\r\n\r\n");
            }
        }

        return new Tuple<object, Error>(null, new GenericError(new Position(0, 0, 0, fn, text), new Position(0, 0, 0, fn, text), "Failed to progress the runtime"));
    }
}