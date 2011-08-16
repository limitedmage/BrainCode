using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace BrainCode {
    public class Lolcode {
        private static readonly int MEMORY_SIZE = 10000;
        private static readonly int STACK_SIZE = 1000;

        private bool running;

        private string[] code;
        private int pc; // program counter

        private char[] memory;
        private int mc; // memory counter

        private int[] stack;
        private int sc; // stack counter

        private delegate void StatementAction(List<string> attrs);
        private Dictionary<Regex, StatementAction> actions;

        public delegate void outputCallback(char output);
        private outputCallback handleOutput;

        public delegate char inputCallback();
        private inputCallback handleInput;

        public delegate void breakpointCallback(string traceData);
        private breakpointCallback handleBreakpoint;

        public Lolcode(string sourceCode, outputCallback outcb, inputCallback incb, breakpointCallback bpcb) {
            GenerateActions();
            
            running = true;
            
            code = ProcessCode(sourceCode);
            memory = new char[MEMORY_SIZE];
            stack = new int[STACK_SIZE];

            pc = 0;
            mc = 0;
            sc = 0;

            handleOutput = outcb;
            handleInput = incb;
            handleBreakpoint = bpcb;
        }

        private string[] ProcessCode(string code) {
            string[] lines = code.Split('\n');
            foreach (string line in lines) {
                line.Trim();
            }
            return lines;
        }

        private void GenerateActions() {
            actions = new Dictionary<Regex, StatementAction>();

            actions.Add(new Regex("^HAI$"), (a) => {
                pc += 1;
            });

            actions.Add(new Regex("^KTHXBYE$"), (a) => {
                if (sc <= 0) {
                    running = false;
                } else {
                    // TODO: search for closing loop
                }
            });

            actions.Add(new Regex("^VISIBLE \"(.+)\"$"), (a) => {
                foreach (char c in a[0]) handleOutput(c);
                pc += 1;
            });
        }

        public bool SanityCheck() {
            // TODO: Implement this
            return false;
        }

        public void Run() {
            while (pc < code.Length && running) {
                RunOneStatement(code[pc]);
            }
        }

        public void RunOneStatement(string statement) {
            foreach (var pair in actions) {
                var match = pair.Key.Match(statement);
                if (match.Success) {
                    var groups = match.Groups;
                    var attrs = new List<string>();
                    foreach (Group group in groups) {
                        if (group.Success && group.Value != statement) {
                            attrs.Add(group.Value);
                        }
                    }
                    pair.Value(attrs);
                    return;
                }
            }

            // unknown statement?
            pc += 1;
        }

        private string Trace() {
            var sb = new StringBuilder();
            sb.AppendLine("pc: " + pc);

            var memtrace = new int[10];
            Array.Copy(memory, memtrace, 10);
            sb.AppendLine("memory: " + string.Join(", ", memtrace));
            sb.AppendLine("mc: " + mc);

            var stacktrace = new int[10];
            Array.Copy(stack, stacktrace, 10);
            sb.AppendLine("stack: " + string.Join(", ", stacktrace));
            sb.AppendLine("sc: " + sc);

            return sb.ToString();
        }
    }
}
