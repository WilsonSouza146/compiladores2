using System;

namespace compilador
{
     class Program
    {
        static void Main(string[] args)
        {
            int a = 1;
            if (a == 0)
            {
                Sintatico sintatico = new Sintatico("C:/Users/wilso/OneDrive/Documentos/GitHub/compiladores2/input.txt");
                sintatico.analysis();
            }
            else
            {
                LexScanner scan = new LexScanner("C:/Users/wilso/OneDrive/Documentos/GitHub/compiladores2/input.txt");
                Token token = null;
                while (true)
                {
                    token = scan.nextToken();
                    if (token == null)
                    {
                        break;
                    }

                    Console.WriteLine(token);
                }
            }
        }
    }
}