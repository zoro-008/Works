using Machine;
using SML;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Script
{

    class Continue : Parser.Result
    { }
    
    class ContinueStatement : ParserFunction
    {
        protected override Parser.Result Evaluate(string data, ref int from)
        {
            return new Continue();
        }
    }
    
    class Break : Parser.Result
    { }
    
    class BreakStatement : ParserFunction
    {
        protected override Parser.Result Evaluate(string data, ref int from)
        {
            return new Break();
        }
    }
    
    
    class PiFunction : ParserFunction
    {
        protected override Parser.Result Evaluate(string data, ref int from)
        {
            return new Parser.Result(3.141592653589793);
        }
    }
    
    class ExpFunction : ParserFunction
    {
        protected override Parser.Result Evaluate(string data, ref int from)
        {
            Parser.Result result = Parser.LoadAndCalculate(data, ref from, Constants.END_ARG_ARRAY);
            result.Value = Math.Exp(result.Value);
            return result;
        }
    }
    
    class PowFunction : ParserFunction
    {
        protected override Parser.Result Evaluate(string data, ref int from)
        {
            Parser.Result arg1 = Parser.LoadAndCalculate(data, ref from, Constants.NEXT_ARG_ARRAY);
            from++; // eat separation
            Parser.Result arg2 = Parser.LoadAndCalculate(data, ref from, Constants.END_ARG_ARRAY);
    
            arg1.Value = Math.Pow(arg1.Value, arg2.Value);
            return arg1;
        }
    }
    
    class SinFunction : ParserFunction
    {
        protected override Parser.Result Evaluate(string data, ref int from)
        {
            Parser.Result arg = Parser.LoadAndCalculate(data, ref from, Constants.END_ARG_ARRAY);
            arg.Value = Math.Sin(arg.Value);
            return arg;
        }
    }
    
    class SqrtFunction : ParserFunction
    {
        protected override Parser.Result Evaluate(string data, ref int from)
        {
            Parser.Result arg = Parser.LoadAndCalculate(data, ref from, Constants.END_ARG_ARRAY);
            arg.Value = Math.Sqrt(arg.Value);
            return arg;
        }
    }
    
    class AbsFunction : ParserFunction
    {
        protected override Parser.Result Evaluate(string data, ref int from)
        {
            Parser.Result arg = Parser.LoadAndCalculate(data, ref from, Constants.END_ARG_ARRAY);
            arg.Value = Math.Abs(arg.Value);
            return arg;
        }
    }
    
    class PsInfoFunction : ParserFunction
    {
        protected override Parser.Result Evaluate(string data, ref int from)
        {
            string pattern = Utils.GetItem(data, ref from).String;
            if (string.IsNullOrWhiteSpace(pattern))
            {
                throw new ArgumentException("Couldn't extract process name");
            }
    
            int MAX_PROC_NAME = 26;
            Interpreter.Instance.AppendOutput(Utils.GetLine());
            Interpreter.Instance.AppendOutput(String.Format("{0} {1} {2} {3} {4}",
              "Process Id".PadRight(15), "Process Name".PadRight(MAX_PROC_NAME),
              "Working Set".PadRight(15), "Virt Mem".PadRight(15), "CPU Time".PadRight(25)));
    
            Process[] processes = Process.GetProcessesByName(pattern);
            List<Parser.Result> results = new List<Parser.Result>(processes.Length);
            for (int i = 0; i < processes.Length; i++)
            {
                Process pr = processes[i];
                int workingSet = (int)(((double)pr.WorkingSet64) / 1000000.0);
                int virtMemory = (int)(((double)pr.VirtualMemorySize64) / 1000000.0);
                string procTitle = pr.ProcessName + " " + pr.MainWindowTitle.Split(null)[0];
                if (procTitle.Length > MAX_PROC_NAME)
                {
                  procTitle = procTitle.Substring(0, MAX_PROC_NAME);
                }
                string procTime = string.Empty;
                try
                {
                  procTime = pr.TotalProcessorTime.ToString().Substring(0, 11);
                }
                catch (Exception) { }
    
                results.Add(new Parser.Result(Double.NaN,
                              string.Format("{0,15} {1," + MAX_PROC_NAME + "} {2,15} {3,15} {4,25}",
                              pr.Id, procTitle,
                              workingSet, virtMemory, procTime)));
                Interpreter.Instance.AppendOutput(results.Last().String);
            }
            Interpreter.Instance.AppendOutput(Utils.GetLine());
    
            if (data.Length > from && data[from] == Constants.NEXT_ARG)
            {
                from++; // eat end of statement semicolon
            }
    
            return new Parser.Result(Double.NaN, null, results);
        }
    }
    
    class KillFunction : ParserFunction
    {
        protected override Parser.Result Evaluate(string data, ref int from)
        {
            Parser.Result id = Utils.GetItem(data, ref from, true /* expectInt */);
    
            int processId = (int)id.Value;
            try
            {
                Process process = Process.GetProcessById(processId);
                process.Kill();
                Interpreter.Instance.AppendOutput("Process " + processId + " killed");
            }
            catch (Exception exc)
            {
                throw new ArgumentException("Couldn't kill process " + processId +
                                            " (" + exc.Message + ")");
            }
    
            return new Parser.Result();
        }
    }
    
    class RunFunction : ParserFunction
    {
        protected override Parser.Result Evaluate(string data, ref int from)
        {
            string processName = Utils.GetItem(data, ref from).String;
    
            if (string.IsNullOrWhiteSpace(processName))
            {
                throw new ArgumentException("Couldn't extract process name");
            }
    
            List<string> args = Utils.GetFunctionArgs(data, ref from);
            int processId = -1;
            try
            {
                Process pr = Process.Start(processName, string.Join("", args.ToArray()));
                processId = pr.Id;
            }
            catch (System.ComponentModel.Win32Exception exc)
            {
                throw new ArgumentException("Couldn't start [" + processName + "]: " + exc.Message);
            }
    
            Interpreter.Instance.AppendOutput("Process " + processName + " started, id: " + processId);
            return new Parser.Result(processId);
        }
    }
    
    class GetEnvFunction : ParserFunction
    {
        protected override Parser.Result Evaluate(string data, ref int from)
        {
            string varName = Utils.GetToken(data, ref from, Constants.END_ARG_ARRAY);
            string res = Environment.GetEnvironmentVariable(varName);
    
            return new Parser.Result(Double.NaN, res);
        }
    }
    
    class SetEnvFunction : ParserFunction
    {
        protected override Parser.Result Evaluate(string data, ref int from)
        {
            string varName = Utils.GetToken(data, ref from, Constants.NEXT_ARG_ARRAY);
            if (from >= data.Length)
            {
                throw new ArgumentException("Couldn't set env variable");
            }
    
            Parser.Result varValue = Utils.GetItem(data, ref from);
            string strValue = varValue.String != null ? varValue.String :
                                                        varValue.Value.ToString();
            Environment.SetEnvironmentVariable(varName, strValue);
    
            return new Parser.Result(Double.NaN, varName);
        }
    }
    
    class PrintFunction : ParserFunction
    {
        protected override Parser.Result Evaluate(string data, ref int from)
        {
            List<string> args = Utils.GetFunctionArgs(data, ref from);
            Interpreter.Instance.AppendOutput(string.Join("", args.ToArray()));
    
            return new Parser.Result();
        }
    }
    
    class IfStatement : ParserFunction
    {
        protected override Parser.Result Evaluate(string data, ref int from)
        {
            Interpreter.Instance.CurrentChar = from;
    
            Parser.Result result = Interpreter.Instance.ProcessIf();
    
            return result;
        }
    }
    
    class WhileStatement : ParserFunction
    {
        protected override Parser.Result Evaluate(string data, ref int from)
        {
            Interpreter.Instance.CurrentChar = from;
            Interpreter.Instance.ProcessWhile();
    
            return new Parser.Result();
        }
    }
    
    class ProcessorTimeFunction : ParserFunction
    {
        protected override Parser.Result Evaluate(string data, ref int from)
        {
            Process pr = Process.GetCurrentProcess();
            TimeSpan ts = pr.TotalProcessorTime;
    
            return new Parser.Result(ts.TotalMilliseconds);
        }
    }
    
    class PwdFunction : ParserFunction
    {
        protected override Parser.Result Evaluate(string data, ref int from)
        {
            string path = Directory.GetCurrentDirectory();
            Interpreter.Instance.AppendOutput(path);
    
            return new Parser.Result(Double.NaN, path);
        }
    }
    
    class Cd__Function : ParserFunction
    {
        protected override Parser.Result Evaluate(string data, ref int from)
        {
            string newDir = null;
    
            try
            {
                string pwd = Directory.GetCurrentDirectory();
                newDir = Directory.GetParent(pwd).FullName;
                Directory.SetCurrentDirectory(newDir);
            }
            catch (Exception exc)
            {
                throw new ArgumentException("Could not change directory: " + exc.Message);
            }
    
            return new Parser.Result(Double.NaN, newDir);
        }
    }
    
    class CdFunction : ParserFunction
    {
        protected override Parser.Result Evaluate(string data, ref int from)
        {
            string newDir = Utils.GetToken(data, ref from, Constants.END_ARG_ARRAY);
    
            try
            {
                Directory.SetCurrentDirectory(newDir);
            }
            catch (Exception exc)
            {
                throw new ArgumentException("Could not change directory: " + exc.Message);
            }
    
            return new Parser.Result(Double.NaN, newDir);
        }
    }
    
    class FindstrFunction : ParserFunction
    {
        protected override Parser.Result Evaluate(string data, ref int from)
        {
            string search = Utils.GetToken(data, ref from, Constants.QUOTE_ARRAY);
            List<string> patterns = Utils.GetFunctionArgs(data, ref from);
    
            bool ignoreCase = true;
            if (patterns.Count > 0 && patterns.Last().Equals("case"))
            {
                ignoreCase = false;
                patterns.RemoveAt(patterns.Count - 1);
            }
            if (patterns.Count == 0)
            {
                patterns.Add("*.*");
            }
    
            List<Parser.Result> results = null;
            try
            {
                string pwd = Directory.GetCurrentDirectory();
                List<string> files = Utils.GetStringInFiles(pwd, search, patterns.ToArray(), ignoreCase);
    
                Interpreter.Instance.AppendOutput("----- Files containing [" + search + "] with pattern [" +
                                            string.Join(", ", patterns.ToArray()) + "] -----");
                results = new List<Parser.Result>(files.Count);
                foreach (string filename in files)
                {
                  results.Add(new Parser.Result(Double.NaN, filename));
                  Interpreter.Instance.AppendOutput(filename);
                }
                Interpreter.Instance.AppendOutput(Utils.GetLine());
            }
            catch (Exception exc)
            {
                throw new ArgumentException("Couldn't find pattern: " + exc.Message);
            }
    
            return new Parser.Result(Double.NaN, null, results);
        }
    }
    
    class FindfilesFunction : ParserFunction
    {
        protected override Parser.Result Evaluate(string data, ref int from)
        {
            List<string> patterns = Utils.GetFunctionArgs(data, ref from);
            if (patterns.Count == 0)
            {
                patterns.Add("*.*");
            }
    
            List<Parser.Result> results = null;
            try
            {
                string pwd = Directory.GetCurrentDirectory();
                List<string> files = Utils.GetFiles(pwd, patterns.ToArray());
    
                //Interpreter.Instance..AppendOutput("----- Found files with pattern [" +
                //                           string.Join(", ", patterns.ToArray()) + "] -----");
                results = new List<Parser.Result>(files.Count);
                foreach (string filename in files)
                {
                  results.Add(new Parser.Result(Double.NaN, filename));
                  //Interpreter.Instance..AppendOutput(filename);
                }
    
                //Interpreter.Instance..AppendOutput(Utils.GetLine());
            }
            catch (Exception exc)
            {
                throw new ArgumentException("Couldn't list directory: " + exc.Message);
            }
    
            return new Parser.Result(Double.NaN, null, results);
        }
    }
    
    class DirFunction : ParserFunction
    {
        protected override Parser.Result Evaluate(string data, ref int from)
        {
            string dirname = (data.Length <= from || data[from] == Constants.END_STATEMENT) ?
                                           Directory.GetCurrentDirectory() :
                                           Utils.GetToken(data, ref from, Constants.END_ARG_ARRAY);
    
            try
            {
                var results = Directory.EnumerateFiles(dirname);
                Interpreter.Instance.AppendOutput("----- Files in [" + dirname + "] -----");
                foreach (string filename in results)
                {
                    Interpreter.Instance.AppendOutput(filename);
                }
                Interpreter.Instance.AppendOutput(Utils.GetLine());
            }
            catch (Exception exc)
            {
                throw new ArgumentException("Could not list directory: " + exc.Message);
            }
    
            return new Parser.Result();
        }
    }
    
    class SetVarFunction : ParserFunction
    {
        protected override Parser.Result Evaluate(string data, ref int from)
        {
            string varName = Utils.GetToken(data, ref from, Constants.NEXT_ARG_ARRAY);
            if (from >= data.Length)
            {
                throw new ArgumentException("Couldn't set variable before end of line");
            }
    
            Parser.Result varValue = Utils.GetItem(data, ref from);
    
            // Check if the variable to be set has the form of x(0),
            // meaning that this is an array element.
            int arrayIndex = Utils.ExtractArrayElement(ref varName);
            if (arrayIndex >= 0)
            {
                bool exists = ParserFunction.FunctionExists(varName);
                Parser.Result  currentValue = exists ?
                      ParserFunction.GetFunction(varName).GetValue(data, ref from) :
                      new Parser.Result();
    
                List<Parser.Result> tuple = currentValue.Tuple == null ?
                                              new List<Parser.Result>() :
                                              currentValue.Tuple;
                if (tuple.Count > arrayIndex)
                {
                    tuple[arrayIndex] = varValue;
                }
                else
                {
                    for (int i = tuple.Count; i < arrayIndex; i++)
                    {
                      tuple.Add(new Parser.Result(Double.NaN, string.Empty));
                    }
                    tuple.Add(varValue);
                }
    
                varValue = new Parser.Result(Double.NaN, null, tuple);
            }
    
            ParserFunction.AddFunction(varName, new GetVarFunction(varValue));
    
            return new Parser.Result(Double.NaN, varName);
        }
    }
    
    class GetVarFunction : ParserFunction
    {
        internal GetVarFunction(Parser.Result value)
        {
            m_value = value;
        }
    
        protected override Parser.Result Evaluate(string data, ref int from)
        {
            // First check if this element is part of an array:
            if (from < data.Length && data[from - 1] == Constants.START_ARG)
            {
                // There is an index given - it may be for an element of the tuple.
                if (m_value.Tuple == null || m_value.Tuple.Count == 0)
                {
                    throw new ArgumentException("No tuple exists for the index");
                }
    
                Parser.Result index = Utils.GetItem(data, ref from, true /* expectInt */);
                if (index.Value < 0 || index.Value >= m_value.Tuple.Count)
                {
                    throw new ArgumentException("Incorrect index [" + index.Value +
                                                "] for tuple of size " + m_value.Tuple.Count);
                }
                return m_value.Tuple[(int)index.Value];
            }
    
            return m_value;
        }
    
        private Parser.Result m_value;
    }
    
    class AppendFunction : ParserFunction
    {
        protected override Parser.Result Evaluate(string data, ref int from)
        {
            // 1. Get the name of the variable.
            string varName = Utils.GetToken(data, ref from, Constants.NEXT_ARG_ARRAY);
            if (from >= data.Length)
            {
              throw new ArgumentException("Couldn't append variable");
            }
            
            // 2. Get the current value of the variable.
            ParserFunction func = ParserFunction.GetFunction(varName);
            Parser.Result currentValue = func.GetValue(data, ref from);
            
            // 3. Get the value to be added or appended.
            Parser.Result newValue = Utils.GetItem(data, ref from);
            
            // 4. Take either the string part if it is defined,
            // or the numerical part converted to a string otherwise.
            string arg1 = currentValue.String != null ? currentValue.String :
                                                        currentValue.Value.ToString();
            string arg2 = newValue.String != null ? newValue.String :
                                                    newValue.Value.ToString();
            
            // 5. The variable becomes a string after adding a string to it.
            newValue.Reset();
            newValue.String = arg1 + arg2;
            
            ParserFunction.AddFunction(varName, new GetVarFunction(newValue));
            
            return newValue;
        }
    }
    
    class ToUpperFunction : ParserFunction
    {
        protected override Parser.Result Evaluate(string data, ref int from)
        {
            // 1. Get the name of the variable.
            string varName = Utils.GetToken(data, ref from, Constants.END_ARG_ARRAY);
            if (from >= data.Length)
            {
              throw new ArgumentException("Couldn't get variable");
            }
            
            // 2. Get the current value of the variable.
            ParserFunction func = ParserFunction.GetFunction(varName);
            Parser.Result currentValue = func.GetValue(data, ref from);
            
            // 3. Take either the string part if it is defined,
            // or the numerical part converted to a string otherwise.
            string arg = currentValue.String != null ? currentValue.String :
                                                       currentValue.Value.ToString();
            
            Parser.Result newValue = new Parser.Result(Double.NaN, arg.ToUpper());
            return newValue;
        }
    }
    
    class ToLowerFunction : ParserFunction
    {
        protected override Parser.Result Evaluate(string data, ref int from)
        {
            // 1. Get the name of the variable.
            string varName = Utils.GetToken(data, ref from, Constants.END_ARG_ARRAY);
            if (from >= data.Length)
            {
              throw new ArgumentException("Couldn't get variable");
            }
            
            // 2. Get the current value of the variable.
            ParserFunction func = ParserFunction.GetFunction(varName);
            Parser.Result currentValue = func.GetValue(data, ref from);
            
            // 3. Take either the string part if it is defined,
            // or the numerical part converted to a string otherwise.
            string arg = currentValue.String != null ? currentValue.String :
                                                       currentValue.Value.ToString();
            
            Parser.Result newValue = new Parser.Result(Double.NaN, arg.ToLower());
            return newValue;
        }
    }
    
    class SizeFunction : ParserFunction
    {
        protected override Parser.Result Evaluate(string data, ref int from)
        {
            // 1. Get the name of the variable.
            string varName = Utils.GetToken(data, ref from, Constants.END_ARG_ARRAY);
            if (from >= data.Length)
            {
              throw new ArgumentException("Couldn't get variable");
            }
            
            // 2. Get the current value of the variable.
            ParserFunction func = ParserFunction.GetFunction(varName);
            Parser.Result currentValue = func.GetValue(data, ref from);
            
            // 3. Take either the length of the underlying tuple or
            // string part if it is defined,
            // or the numerical part converted to a string otherwise.
            int size = currentValue.Tuple != null ?  currentValue.Tuple.Count : 
                       currentValue.String != null ? currentValue.String.Length :
                                                     currentValue.Value.ToString().Length;
            
            Parser.Result newValue = new Parser.Result(size);
            return newValue;
        }
    }
    
    class SubstrFunction : ParserFunction
    {
        protected override Parser.Result Evaluate(string data, ref int from)
        {
            string substring;
            
            // 1. Get the name of the variable.
            string varName = Utils.GetToken(data, ref from, Constants.NEXT_ARG_ARRAY);
            if (from >= data.Length)
            {
                throw new ArgumentException("Couldn't get variable");
            }
            
            // 2. Get the current value of the variable.
            ParserFunction func = ParserFunction.GetFunction(varName);
            Parser.Result currentValue = func.GetValue(data, ref from);
            
            // 3. Take either the string part if it is defined,
            // or the numerical part converted to a string otherwise.
            string arg = currentValue.String != null ? currentValue.String :
                                                       currentValue.Value.ToString();
            // 4. Get the initial index of the substring.
            bool lengthAvailable = Utils.SeparatorExists(data, from);
            
            Parser.Result init = Utils.GetItem(data, ref from, true /* expectInt */);
            
            // 5. Get the length of the substring if available.
            if (lengthAvailable)
            {
                Parser.Result length = Utils.GetItem(data, ref from, true /* expectInt */);
                if (init.Value + length.Value > arg.Length)
                {
                  throw new ArgumentException("The total substring length is larger than  [" +
                                               arg + "]");
                }
                substring = arg.Substring((int)init.Value, (int)length.Value);
            }
            else
            {
                substring = arg.Substring((int)init.Value);
            }
            Parser.Result newValue = new Parser.Result(Double.NaN, substring);
            
            return newValue;
        }
    }
    
    class IndexOfFunction : ParserFunction
    {
        protected override Parser.Result Evaluate(string data, ref int from)
        {
            // 1. Get the name of the variable.
            string varName = Utils.GetToken(data, ref from, Constants.NEXT_ARG_ARRAY);
            if (from >= data.Length)
            {
                throw new ArgumentException("Couldn't append variable");
            }
    
            // 2. Get the current value of the variable.
            ParserFunction func = ParserFunction.GetFunction(varName);
            Parser.Result currentValue = func.GetValue(data, ref from);
    
            // 3. Get the value to be looked for.
            Parser.Result searchValue = Utils.GetItem(data, ref from);
    
            // 4. Take either the string part if it is defined,
            // or the numerical part converted to a string otherwise.
            string basePart = currentValue.String != null ? currentValue.String :
                                                            currentValue.Value.ToString();
            string search = searchValue.String != null ? searchValue.String :
                                                         searchValue.Value.ToString();
    
            int result = basePart.IndexOf(search);
            return new Parser.Result(result, null);
        }
    }

    class True : ParserFunction
    {
        protected override Parser.Result Evaluate(string data, ref int from)
        {
            return new Parser.Result(1);
        }
    }
    class False : ParserFunction
    {
        protected override Parser.Result Evaluate(string data, ref int from)
        {
            return new Parser.Result(0);
        }
    }
    class Return : ParserFunction
    {
        protected override Parser.Result Evaluate(string data, ref int from)
        {
            throw new ArgumentException(Constants.RETURN);
            //return new Parser.Result(Double.NaN,Constants.RETURN,null);
        }
    }
    class End : ParserFunction
    {
        protected override Parser.Result Evaluate(string data, ref int from)
        {
            Interpreter.Instance.AppendOutput("End");
            throw new ArgumentException(Constants.END);
            //return new Parser.Result(Double.NaN,Constants.END,null);
        }

        static string Comment = "종료";
        static int    ParaCnt = 0;
    }
    class StopWatch : ParserFunction
    {
        public Stopwatch[] Timer = new Stopwatch[Constants.STOPWATCH_MAX];
        protected override Parser.Result Evaluate(string data, ref int from)
        {
            Timer[0] = new Stopwatch();
            var list = Utils.GetFunctionArgs(data, ref from, ParaCnt);
            uint.TryParse(list[0],out uint   arg1);
            string arg2 = list[1];

                 if(arg2.ToLower().Contains("sw.Start"   )) Timer[arg1].Start  ();
            else if(arg2.ToLower().Contains("sw.Stop"    )) Timer[arg1].Stop   ();
            else if(arg2.ToLower().Contains("sw.Restart" )) Timer[arg1].Restart();

            double ms = Timer[arg1].ElapsedMilliseconds;
            Interpreter.Instance.AppendOutput("StopWatch(" + arg2 + ")" + " = " + ms.ToString());
            return new Parser.Result(ms,null,null);
        }

        static string Comment = "스탑 워치(int,sw)";
        static int    ParaCnt = 2;
    }

    //class Using : ParserFunction
    //{
    //    internal Using(Parser parser) { m_parser = parser; }
    //    protected override Parser.Result Evaluate(string data, ref int from)
    //    {
    //        var list = Utils.GetFunctionArgs(data, ref from, ParaCnt);
    //        string arg1 = list[0];
    //        if (Interpreter.s_Script.TryGetValue(arg1, out string script))
    //        {
    //            ParserFunction.AddFunction(arg1, new GetVarFunction(new Parser.Result(Double.NaN,script)));
    //        }
    //        else
    //        {
    //            throw new ArgumentException("The argument is invalid.");
    //        }
    //        return new Parser.Result();
    //    }
    //    private Parser m_parser;
    //    static string Comment = "사용할 파트,AUTORUN(mi,double)";
    //    static int    ParaCnt = 1;
    //}

    //Motion
    class Motor_Move : ParserFunction
    {

        protected override Parser.Result Evaluate(string data, ref int from)
        {
            var list = Utils.GetFunctionArgs(data, ref from, ParaCnt);
            uint  .TryParse(list[0],out uint   arg1);
            double.TryParse(list[1],out double arg2);
            if(!Constants.CHECK) ML.MT_GoAbsRun(arg1,arg2);
            Interpreter.Instance.AppendOutput("MoveMotr(" + arg1.ToString() + ", " + arg2.ToString()+")");
            return new Parser.Result();
        }
        static string Comment = "모터 이동(mi,double)";
        static int    ParaCnt = 1;
    }
    class Motor_MovePos : ParserFunction
    {

        protected override Parser.Result Evaluate(string data, ref int from)
        {
            var list = Utils.GetFunctionArgs(data, ref from, ParaCnt);
            uint.TryParse(list[0],out uint arg1);
            uint.TryParse(list[1],out uint arg2);
            double dPos = PM.GetValue(arg1,arg2);
            if(!Constants.CHECK) ML.MT_GoAbsRun(arg1,dPos);
            Interpreter.Instance.AppendOutput("MoveMotr(" + arg1.ToString() + ", " + dPos.ToString()+")");
            return new Parser.Result();
        }
        static string Comment = "모터 포지션 이동(mi,pv)";
        static int    ParaCnt = 2;
    }    
    class Motor_CheckStop : ParserFunction
    {
        protected override Parser.Result Evaluate(string data, ref int from)
        {
            var list = Utils.GetFunctionArgs(data, ref from, ParaCnt);
            uint.TryParse(list[0],out uint arg1);
            bool bRet = ML.MT_GetStopInpos(arg1);
            if(bRet) Interpreter.Instance.AppendOutput("Motor_CheckStop(" + arg1.ToString() +") = " + bRet.ToString());
            return new Parser.Result(bRet == true ? 1 : 0,null,null);
        }
        static string Comment = "모터 정지 확인(mi)";
        static int    ParaCnt = 1;
    }
    class Motor_CheckStopPos : ParserFunction
    {
        protected override Parser.Result Evaluate(string data, ref int from)
        {
            var list = Utils.GetFunctionArgs(data, ref from, ParaCnt);
            uint.TryParse(list[0],out uint arg1);
            uint.TryParse(list[1],out uint arg2);
            bool bRet  = ML.MT_GetStopPos((mi)arg1,(pv)arg2);
            if(bRet) Interpreter.Instance.AppendOutput("Motor_CheckStopPos(" + arg1.ToString() + "," + arg2.ToString() + ") = " + bRet.ToString());
            return new Parser.Result(bRet == true ? 1 : 0,null,null);
        }
        static string Comment = "모터 정지 위치 확인(mi,pv)";
        static int    ParaCnt = 2;
    }    
    class Motor_Stop : ParserFunction
    {
        protected override Parser.Result Evaluate(string data, ref int from)
        {
            var list = Utils.GetFunctionArgs(data, ref from, ParaCnt);
            uint.TryParse(list[0],out uint arg1);
            if(!Constants.CHECK) ML.MT_Stop(arg1);
            Interpreter.Instance.AppendOutput("Motor_Stop(" + arg1.ToString() + ")");
            return new Parser.Result();
        }
        static string Comment = "모터 정지(mi)";
        static int    ParaCnt = 1;
    }   
    class Motor_Servo : ParserFunction
    {
        protected override Parser.Result Evaluate(string data, ref int from)
        {
            var list = Utils.GetFunctionArgs(data, ref from, ParaCnt);
            uint.TryParse(list[0],out uint arg1);
            uint.TryParse(list[1],out uint arg2);
            if(!Constants.CHECK) ML.MT_SetServo((mi)arg1,arg2 == 1 ? true:false);
            Interpreter.Instance.AppendOutput("Motor_Servo(" + arg1.ToString() + "," + arg2.ToString() + ")");
            return new Parser.Result();
        }
        static string Comment = "모터 서보 온/오프(mi,bool)";
        static int    ParaCnt = 2;
    }   
    class Motor_Home : ParserFunction
    {
        protected override Parser.Result Evaluate(string data, ref int from)
        {
            var list = Utils.GetFunctionArgs(data, ref from, ParaCnt);
            uint.TryParse(list[0],out uint arg1);
            if(!Constants.CHECK) ML.MT_GoHome((mi)arg1);
            Interpreter.Instance.AppendOutput("Motor_Home(" + arg1.ToString() + ")");
            return new Parser.Result();
        }
        static string Comment = "모터 원점 동작(mi)";
        static int    ParaCnt = 1;
    }   
    class Motor_CheckHome : ParserFunction
    {
        protected override Parser.Result Evaluate(string data, ref int from)
        {
            var list = Utils.GetFunctionArgs(data, ref from, ParaCnt);
            uint.TryParse(list[0],out uint arg1);
            bool bRet  = ML.MT_GetHomeDone((mi)arg1);
            Interpreter.Instance.AppendOutput("Motor_CheckHome(" + arg1.ToString() + ") = " + bRet.ToString());
            return new Parser.Result(bRet == true ? 1 : 0,null,null);
        }
        static string Comment = "모터 원점 확인(mi)";
        static int    ParaCnt = 1;
    }   

    //Cylinder
    class Cylinder_Move : ParserFunction
    {
        protected override Parser.Result Evaluate(string data, ref int from)
        {
            var list = Utils.GetFunctionArgs(data, ref from, ParaCnt);
            uint.TryParse(list[0],out uint arg1);
            uint.TryParse(list[1],out uint arg2);
            if(!Constants.CHECK) ML.CL_Move((ci)arg1,(fb)arg2);
            Interpreter.Instance.AppendOutput("Cylinder_Move(" + arg1.ToString() + "," + arg2.ToString() + ")" );
            return new Parser.Result();
        }
        static string Comment = "실린더 이동(ci,fb)";
        static int    ParaCnt = 2;
    }   
    class Cylinder_CheckStop : ParserFunction
    {
        protected override Parser.Result Evaluate(string data, ref int from)
        {
            var list = Utils.GetFunctionArgs(data, ref from, ParaCnt);
            uint.TryParse(list[0],out uint arg1);
            uint.TryParse(list[1],out uint arg2);
            bool bRet = ML.CL_Complete((ci)arg1,(fb)arg2);
            Interpreter.Instance.AppendOutput("Cylinder_CheckStop(" + arg1.ToString() + "," + arg2.ToString() + ") = " + bRet.ToString() );
            return new Parser.Result(bRet == true ? 1 : 0,null,null);
        }
        static string Comment = "실린더 정지확인(ci,fb)";
        static int    ParaCnt = 2;
    }   

    //IO
    class IO_SetY : ParserFunction
    {
        protected override Parser.Result Evaluate(string data, ref int from)
        {
            var list = Utils.GetFunctionArgs(data, ref from, ParaCnt);
            uint.TryParse(list[0],out uint arg1);
            uint.TryParse(list[1],out uint arg2);
            if(!Constants.CHECK) ML.IO_SetY((yi)arg1,arg2 == 1);

            Interpreter.Instance.AppendOutput("IO_SetY(" + arg1.ToString() + ", " + arg2.ToString() + ")" );
            return new Parser.Result();
        }
        static string Comment = "출력(yi)";
        static int    ParaCnt = 1;
    }   
    class IO_GetY : ParserFunction
    {
        protected override Parser.Result Evaluate(string data, ref int from)
        {
            var list = Utils.GetFunctionArgs(data, ref from, ParaCnt);
            uint.TryParse(list[0],out uint arg1);
            bool bRet = ML.IO_GetY((yi)arg1);
            Interpreter.Instance.AppendOutput("IO_GetY(" + arg1.ToString() + ") = " + bRet.ToString() );
            return new Parser.Result(bRet == true ? 1 : 0,null,null);
        }
        static string Comment = "출력 상태(yi)";
        static int    ParaCnt = 1;
    }   
    class IO_GetX : ParserFunction
    {
        protected override Parser.Result Evaluate(string data, ref int from)
        {
            var list = Utils.GetFunctionArgs(data, ref from, ParaCnt);
            uint.TryParse(list[0],out uint arg1);
            bool bRet = ML.IO_GetX((xi)arg1);
            Interpreter.Instance.AppendOutput("IO_GetX(" + arg1.ToString() + ") = " + bRet.ToString() );
            return new Parser.Result(bRet == true ? 1 : 0,null,null);
        }
        static string Comment = "입력 상태(xi)";
        static int    ParaCnt = 1;
    }   

    //Data
    class Data_Set : ParserFunction
    {
        protected override Parser.Result Evaluate(string data, ref int from)
        {
            var list = Utils.GetFunctionArgs(data, ref from, ParaCnt);
            uint.TryParse(list[0],out uint arg1);
            uint.TryParse(list[1],out uint arg2);
            DM.ARAY[arg1].SetStat((cs)arg2);
            Interpreter.Instance.AppendOutput("Data_Set("+arg1.ToString()+", "+arg2.ToString()+")");
            return new Parser.Result(Double.NaN,null,null);
        }
        static string Comment = "상태셋팅하기(ri,cs)"; 
        static int    ParaCnt = 1;
    }
    class Data_Get : ParserFunction
    {
        protected override Parser.Result Evaluate(string data, ref int from)
        {
            var list = Utils.GetFunctionArgs(data, ref from, ParaCnt);
            uint.TryParse(list[0],out uint arg1);
             int.TryParse(list[1],out  int arg2);
             int.TryParse(list[2],out  int arg3);
            cs status = DM.ARAY[arg1].GetStat(arg2,arg3);
            Interpreter.Instance.AppendOutput("Data_Get("+arg1.ToString()+", "+arg2.ToString()+", "+arg3.ToString()+") = " + status.ToString());
            return new Parser.Result((int)status,null,null);
        }
        static string Comment = "상태받아오기(ri,int col,int row)";
        static int    ParaCnt = 1;
    }
    
    //Option
    class Option_Device: ParserFunction
    {
        protected override Parser.Result Evaluate(string data, ref int from)
        {
            var list = Utils.GetFunctionArgs(data, ref from, ParaCnt);
            uint.TryParse(list[0],out uint arg1);
            double dRst = OM.DevInfo.dInfo[arg1];
            Interpreter.Instance.AppendOutput("Option_Common("+arg1.ToString()+")" + " = " + dRst.ToString());
            return new Parser.Result(dRst);
        }
        static string Comment = "디바이스별 옵션(int : 0~"+Constants.OPTION_DEVICE_MAX.ToString()+")";
        static int    ParaCnt = 1;
    }

    class Option_Common : ParserFunction
    {
        protected override Parser.Result Evaluate(string data, ref int from)
        {
            var list = Utils.GetFunctionArgs(data, ref from, ParaCnt);
            uint.TryParse(list[0],out uint arg1);
            double dRst = OM.CmnOptn.dInfo[arg1];
            Interpreter.Instance.AppendOutput("Option_Common("+arg1.ToString()+")" + " = " + dRst.ToString());
            return new Parser.Result(dRst);
        }
        static string Comment = "공용 옵션(int : 0~"+Constants.OPTION_COMMON_MAX.ToString()+")";
        static int    ParaCnt = 1;
    }

    //Error
    class Error_Set : ParserFunction
    {
        protected override Parser.Result Evaluate(string data, ref int from)
        {
            var list = Utils.GetFunctionArgs(data, ref from, ParaCnt);
            uint.TryParse(list[0],out uint arg1);
            ML.ER_SetErr((ei)arg1);
            Interpreter.Instance.AppendOutput("Error_Set("+arg1.ToString()+")");
            return new Parser.Result();
        }
        static string Comment = "에러(ei)";
        static int    ParaCnt = 1;
    }

    //Position
    class Position_Get : ParserFunction
    {
        protected override Parser.Result Evaluate(string data, ref int from)
        {
            var list = Utils.GetFunctionArgs(data, ref from, ParaCnt);
            uint.TryParse(list[0],out uint arg1);
            uint.TryParse(list[1],out uint arg2);
            double dRst = PM.GetValue(arg1,arg2);

            Interpreter.Instance.AppendOutput("Position("+arg1.ToString() + "," + arg2.ToString() +")" );
            return new Parser.Result(dRst);
        }
        static string Comment = "포지션 받아오기(mi,pv)";
        static int    ParaCnt = 2;
    }

}
