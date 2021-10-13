using System;

namespace compilador
{
     class Program
    {
        static void Main(string[] args)
        {
            Sintatico sintatico = 
                new Sintatico("C:/Users/wilso/OneDrive/Documentos/GitHub/compiladores2/input.txt");
            sintatico.analysis();
            Interpreter interpreter = 
                new Interpreter("C:/Users/wilso/OneDrive/Documentos/GitHub/compiladores2/output.txt");
            interpreter.execute();
        }
    }
}