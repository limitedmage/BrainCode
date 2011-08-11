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
        private char[] tape;
        private int pc; // program counter

        private int[] stack;
        private int sc; // stack counter

        public delegate void outputCallback(char output);
        private outputCallback handleOutput;

        public delegate char inputCallback();
        private inputCallback handleInput;

        public Brainfuck(string sourceCode, outputCallback outcb, inputCallback incb) {
            code = sourceCode;
            tape = new char[MEMORY_SIZE];
            stack = new int[STACK_SIZE];

            pc = 0;
            sc = 0;

            handleOutput = outcb;
            handleInput = incb;
        }
    }
}
