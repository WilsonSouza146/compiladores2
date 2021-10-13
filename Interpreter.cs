using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;

namespace compilador
{
    public class Interpreter
    {
        private List<string> C { get; set; }
        private List<float> D { get; set; }
        private int i { get; set;}
        private int s { get; set; }

        public Interpreter(string path)
        {
            C = File.ReadLines(path).ToList();
            D = new List<float>();
            i = 0;
        }

        public void execute()
        {
            while (C.Count > i)
            {
                // recebe a instrucao e a dividi em funcao e parametro
                string[] term = C[i].Split(' ');
                string func = term[0];

                switch (func)
                {
                    case "CRCT": CRCT(term[1]);
                        break;
                    case "CRVL": CRVL(term[1]);
                        break;
                    case "SOMA": SOMA();
                        break;
                    case "SUBT": SUBT();
                        break;
                    case "MULT": MULT();
                        break;
                    case "DIVI": DIVI();
                        break;
                    case "INVI": INVE();
                        break;
                    case "CONJ": CONJ();
                        break;
                    case "DISJ": DISJ();
                        break;
                    case "NEGA": NEGA();
                        break;
                    case "CPME": CPME();
                        break;
                    case "CPMA": CPMA();
                        break;
                    case "CPIG": CPIG();
                        break;
                    case "CDES": CDES();
                        break;
                    case "CPMI": CPMI();
                        break;
                    case "CMAI": CMAI();
                        break;
                    case "ARMZ": ARMZ(term[1]);
                        break;
                    case "DSVI": DSVI(term[1]);
                        break;
                    case "DSVF": DSVF(term[1]);
                        break;
                    case "LEIT": LEIT();
                        break;
                    case "IMPR": IMPR();
                        break;
                    case "ALME": ALME(term[1]);
                        break;
                    case "INPP": INPP();
                        break;
                    case "PARA": PARA();
                        break;
                }
                // se nao for uma func de condicao avança a posicao
                if (func != "DSVF" && func != "DSVI")
                {
                    i++;
                }
            }
        }
        
        private void CRCT(string k)
        {
            s++; 
            D = D.Append(float.Parse(k, CultureInfo.InvariantCulture)).ToList();
        }

        private void CRVL(string n)
        {
            s++;
            D = D.Append(D[int.Parse(n)]).ToList();
        }
        
        private void SOMA()
        {
            D[s - 1] += D[s];
            D.RemoveAt(s--);
        }
        
        private void SUBT()
        {
            D[s - 1] -= D[s];
            D.RemoveAt(s--);
        }
        
        private void MULT()
        {
            D[s - 1] *= D[s];
            D.RemoveAt(s--);
        }
        
        private void DIVI()
        {
            D[s - 1] /= D[s];
            D.RemoveAt(s--);
        }
        
        private void INVE()
        {
            D[s] *= -1;
        }
        
        private void CONJ()
        {
            if ( (int) D[s-1] == 1 && (int) D[s] == 1)
            {
                D[s - 1] = 1;
            }
            else
            {
                D[s - 1] = 0;
                D.RemoveAt(s--);
            }
        }
        
        private void DISJ()
        {
            if ((int) D[s - 1] == 1 || (int) D[s] == 1)
            {
                D[s - 1] = 1;
            }
            else
            {
                D[s - 1] = 1;
                D.RemoveAt(s--);
            }
        }
        
        private void NEGA()
        {
            D[s] = 1 - D[s];
        }
        
        private void CPME()
        {
            if (D[s - 1] < D[s])
            {
                D[s - 1] = 1;
            }
            else
            {
                D[s - 1] = 0;
                D.RemoveAt(s--);
            }
        }

        private void CPMA()
        {
            if (D[s - 1] > D[s])
            {
                D[s - 1] = 1;
            }
            else
            {
                D[s - 1] = 0;
                D.RemoveAt(s--);
            }
        }

        private void CPIG()
        {
            if (D[s - 1] == D[s])
            {
                D[s - 1] = 1;
            }
            else
            {
                D[s - 1] = 0;
                D.RemoveAt(s--);
            }
        }

        private void CDES()
        {
            if (D[s - 1] != D[s])
            {
                D[s - 1] = 1;
            }
            else
            {
                D[s - 1] = 0;
                D.RemoveAt(s--);
            }
        }

        private void CPMI()
        {
            if (D[s - 1] <= D[s])
            {
                D[s - 1] = 1;
            }
            else
            {
                D[s - 1] = 0;
                D.RemoveAt(s--);
            }
        }

        private void CMAI()
        {
            if (D[s - 1] >= D[s])
            {
                D[s - 1] = 1;
            }
            else
            {
                D[s - 1] = 0;
                D.RemoveAt(s--);
            }
        }

        private void ARMZ(string n)
        {
            D[int.Parse(n)] = D[s]; 
            D.RemoveAt(s--);
        }

        private void DSVI(string p)
        {
            i = int.Parse(p); 
        }

        private void DSVF(string p)
        {
            if (D[s] == 0)
            {
                i = int.Parse(p);
            }
            else
            {
                i++;
            }
            D.RemoveAt(s--);
        }

        private void LEIT()
        {
            s++;
            D = D.Append(float.Parse(Console.ReadLine(), CultureInfo.InvariantCulture)).ToList();
        }

        private void IMPR()
        {
            Console.WriteLine(D[s].ToString(CultureInfo.InvariantCulture));
            D.RemoveAt(s--);
        }

        private void ALME(string m)
        {
            D = D.Append(0).ToList();
            s += int.Parse(m);
        }

        private void INPP()
        {
            s = -1;
        }

        private void PARA()
        {
            
        }
        
    }
}