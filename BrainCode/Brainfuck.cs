using System;
using System.Text;
using System.Linq;

namespace BrainCode {
    public class Brainfuck {
        private static readonly int MEMORY_SIZE = 10000;
        private static readonly int STACK_SIZE = 1000;
        private static readonly int RUN_LIMIT = 8000000;

        private string code;
        private int pc; // program counter

        private char[] memory;
        private int mc; // memory counter

        private int[] stack;
        private int sc; // stack counter

        private int rc; // run count

        public delegate void outputCallback(char output);
        private outputCallback handleOutput;

        public delegate char inputCallback();
        private inputCallback handleInput;

        public delegate void breakpointCallback(string traceData);
        private breakpointCallback handleBreakpoint;

        public Brainfuck(string sourceCode, outputCallback outcb, inputCallback incb, breakpointCallback bpcb) {
            code = sourceCode;
            memory = new char[MEMORY_SIZE];
            stack = new int[STACK_SIZE];

            pc = 0;
            mc = 0;
            sc = 0;
            rc = 0;

            handleOutput = outcb;
            handleInput = incb;
            handleBreakpoint = bpcb;
        }

        public bool SanityCheck() {
            int counter = 0;
            foreach (char c in code) {
                if (c == '[') {
                    counter += 1;
                } else if (c == ']') {
                    counter -= 1;
                    if (counter < 0) {
                        return false;
                    }
                }
            }

            return counter == 0;
        }

        public void Run() {
            if (SanityCheck()) {
                while (pc < code.Length) {
                    if (RunOneStatement(code[pc])) {
                        pc += 1;
                    }
                    rc += 1;
                    if (rc > RUN_LIMIT) {
                        throw new ArgumentException("The code ran for too long. You probably have an infinite loop.");
                    }
                }
            } else {
                throw new ArgumentException("Code is invalid, check that all [ and ] match.");
            }
        }

        public bool RunOneStatement(char c) {
            switch (c) {
                case '-':
                    memory[mc] -= (char) 1;
                    break;
                case '+':
                    memory[mc] += (char) 1;
                    break;
                case '<':
                    mc -= 1;
                    break;
                case '>':
                    mc += 1;
                    break;
                case '.':
                    handleOutput(memory[mc]);
                    break;
                case ',':
                    memory[mc] = handleInput();
                    break;
                case '[':
                    OpenLoop();
                    return false;
                case ']':
                    CloseLoop();
                    return false;
                case 'b':
                    handleBreakpoint(Trace());
                    break;
            }

            return true;
        }

        private void OpenLoop() {
            if (memory[mc] == 0) {
                // find loop close
                int closeCounter = pc;
                while (closeCounter < code.Length) {
                    if (code[closeCounter] == ']') {
                        pc = closeCounter + 1;
                        break;
                    }
                    closeCounter += 1;
                }
            } else {
                // push stack
                stack[sc] = pc;
                sc += 1;
                pc += 1;
            }
        }

        private void CloseLoop() {
            // pop stack
            sc -= 1;
            pc = stack[sc];
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
