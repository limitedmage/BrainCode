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
using System.Collections.Generic;

namespace BrainCode {
    public class Brainfuck {
        private static readonly int MEMORY_SIZE = 10000;
        private static readonly int STACK_SIZE = 1000;

        private string code;
        private int pc; // program counter

        private char[] memory;
        private int mc; // memory counter

        private int[] stack;
        private int sc; // stack counter

        public delegate void outputCallback(char output);
        private outputCallback handleOutput;

        public delegate char inputCallback();
        private inputCallback handleInput;

        public Brainfuck(string sourceCode, outputCallback outcb, inputCallback incb) {
            code = sourceCode;
            memory = new char[MEMORY_SIZE];
            stack = new int[STACK_SIZE];

            pc = 0;
            mc = 0;
            sc = 0;

            handleOutput = outcb;
            handleInput = incb;
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
            while (pc < code.Length) {
                RunOneStatement(code[pc]);
            }
        }

        public void RunOneStatement(char c) {
            switch (c) {
                case '-':
                    memory[mc] -= (char)1;
                    pc += 1;
                    break;
                case '+':
                    memory[mc] += (char)1;
                    pc += 1;
                    break;
                case '<':
                    mc -= 1;
                    pc += 1;
                    break;
                case '>':
                    mc += 1;
                    pc += 1;
                    break;
                case '.':
                    handleOutput(memory[mc]);
                    pc += 1;
                    break;
                case ',':
                    memory[mc] = handleInput();
                    pc += 1;    
                    break;
                case '[':
                    OpenLoop();
                    break;
                case ']':
                    CloseLoop();
                    break;
                default:
                    // invalid character, continue
                    pc += 1;
                    break;
            }
        }

        private void OpenLoop() {
            if (memory[mc] == 0) {
                // find loop close
                int closeCounter = pc;
                while (closeCounter < code.Length) {
                    if (code[closeCounter] == ']') {
                        pc = closeCounter + 1;
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
    }
}
